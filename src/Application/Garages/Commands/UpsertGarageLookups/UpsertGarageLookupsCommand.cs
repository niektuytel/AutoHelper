using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleTimelines;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using Hangfire.Server;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Application.Garages.Commands.UpsertGarageLookups;


public record UpsertGarageLookupsCommand : IQueueRequest
{
    public const int InsertAll = -1;
    public const int UpdateAll = -1;
    public const int DefaultStartingRowIndex = 0;
    public const int DefaultEndingRowIndex = -1;

    public UpsertGarageLookupsCommand(
        int startRowIndex = DefaultStartingRowIndex,
        int endRowIndex = DefaultEndingRowIndex,
        int maxInsertAmount = InsertAll,
        int maxUpdateAmount = UpdateAll,
        int batchSize = 10000
    )
    {
        StartRowIndex = startRowIndex;
        EndRowIndex = endRowIndex;
        MaxInsertAmount = maxInsertAmount;
        MaxUpdateAmount = maxUpdateAmount;
        BatchSize = batchSize;
    }

    public int StartRowIndex { get; init; }
    public int EndRowIndex { get; set; }
    public int MaxInsertAmount { get; init; }
    public int MaxUpdateAmount { get; init; }
    public int BatchSize { get; set; }

    public IQueueService QueueService { get; set; }
}

public class UpsertGarageLookupsCommandHandler : IRequestHandler<UpsertGarageLookupsCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IGarageService _garageService;
    private IEnumerable<RDWCompanyService> _allRDWServices;
    private int _maxInsertAmount;
    private int _maxUpdateAmount;

    public UpsertGarageLookupsCommandHandler(IApplicationDbContext dbContext, IGarageService garageService)
    {
        _dbContext = dbContext;
        _garageService = garageService;
    }

    public async Task<Unit> Handle(UpsertGarageLookupsCommand request, CancellationToken cancellationToken)
    {
        int totalRecords = await CalculateTotalRecords(request, cancellationToken);
        SetMaxInsertAndUpdateAmounts(request, totalRecords);

        _allRDWServices = await _garageService.GetRDWServices();
        LogInformationBasedOnAmount(request);

        int numberOfBatches = CalculateNumberOfBatches(request.BatchSize, totalRecords);
        await ProcessBatchesAsync(request, numberOfBatches, cancellationToken);

        var message = $"Operation finished. Inserted: {request.MaxInsertAmount - _maxInsertAmount}, Updated: {request.MaxUpdateAmount - _maxUpdateAmount}";
        request.QueueService.LogInformation(message);
        return Unit.Value;

    }

    private async Task<int> CalculateTotalRecords(UpsertGarageLookupsCommand request, CancellationToken cancellationToken)
    {
        int totalRecords = await _garageService.GetRDWCompaniesCount();
        if (request.EndRowIndex > 0)
        {
            totalRecords = request.EndRowIndex - request.StartRowIndex;
            request.EndRowIndex = totalRecords;
        }
        return totalRecords;
    }

    private void SetMaxInsertAndUpdateAmounts(UpsertGarageLookupsCommand request, int totalRecords)
    {
        _maxInsertAmount = DetermineMaxAmount(request.MaxInsertAmount, totalRecords);
        _maxUpdateAmount = DetermineMaxAmount(request.MaxUpdateAmount, totalRecords);
    }

    private int DetermineMaxAmount(int requestedAmount, int totalRecords)
    {
        return requestedAmount == UpsertGarageLookupsCommand.InsertAll ? totalRecords : requestedAmount;
    }

    private void LogInformationBasedOnAmount(UpsertGarageLookupsCommand request)
    {
        request.QueueService.LogInformation($"Start upsert rows from {request.StartRowIndex} to {request.EndRowIndex}");

        if (request.MaxInsertAmount == UpsertGarageLookupsCommand.InsertAll)
        {
            request.QueueService.LogInformation($"Insert all available garages");
        }
        else
        {
            request.QueueService.LogInformation($"Insert {_maxInsertAmount} garages");
        }

        if (request.MaxUpdateAmount == UpsertGarageLookupsCommand.UpdateAll)
        {
            request.QueueService.LogInformation($"Update all available garages");
        }
        else
        {
            request.QueueService.LogInformation($"Update {_maxUpdateAmount} garages");
        }
    }

    private int CalculateNumberOfBatches(int batchSize, int totalRecords)
    {
        return (totalRecords / batchSize) + (totalRecords % batchSize > 0 ? 1 : 0);
    }

    private async Task ProcessBatchesAsync(UpsertGarageLookupsCommand request, int numberOfBatches, CancellationToken cancellationToken)
    {
        for (int i = 0; i < numberOfBatches; i++)
        {
            if (ShouldStopProcessing(request, cancellationToken))
            {
                break;
            }

            var start = request.StartRowIndex + (i * request.BatchSize);
            var batch = await _garageService.GetRDWCompanies(start, request.BatchSize);

            var (garageItemsToInsert, garageItemsToUpdate, garageServicesToInsert, garageServicesToUpdate, garageServicesToRemove) = await ProcessGarageBatchAsync(batch, request, cancellationToken);

            if (garageItemsToInsert.Any())
            {
                await _dbContext.BulkInsertAsync(garageItemsToInsert, cancellationToken);
            }

            if (garageItemsToUpdate.Any())
            {
                await _dbContext.BulkUpdateAsync(garageItemsToUpdate, cancellationToken);
            }

            if (garageServicesToInsert.Any())
            {
                await _dbContext.BulkInsertAsync(garageServicesToInsert, cancellationToken);
            }

            if (garageServicesToUpdate.Any())
            {
                await _dbContext.BulkUpdateAsync(garageServicesToUpdate, cancellationToken);
            }

            if (garageServicesToRemove.Any())
            {
                await _dbContext.BulkRemoveAsync(garageServicesToRemove, cancellationToken);
            }

            request.QueueService.LogInformation(
                $"[{(start + request.BatchSize)}/{request.EndRowIndex}] insert: {garageItemsToInsert.Count} | update: {garageItemsToUpdate.Count} items"
            );
        }

    }

    private async Task<(List<GarageLookupItem> InsertLookupItems, List<GarageLookupItem> UpdateLookupItems, List<GarageLookupServiceItem> InsertServiceItems, List<GarageLookupServiceItem> UpdateServiceItems, List<GarageLookupServiceItem> RemoveServiceItems)> ProcessGarageBatchAsync(IEnumerable<RDWCompany> batch, UpsertGarageLookupsCommand request, CancellationToken cancellationToken)
    {
        var storedGarages = await _dbContext.GarageLookups
            .AsNoTracking()
            .Include(x => x.Services)
            .Where(x => batch.Select(y => y.Volgnummer.ToString()).Contains(x.Identifier))
            .ToListAsync(cancellationToken);

        var garagesToInsert = new List<GarageLookupItem>();
        var garagesToUpdate = new List<GarageLookupItem>();
        var garageServicesToInsert = new List<GarageLookupServiceItem>();
        var garageServicesToUpdate = new List<GarageLookupServiceItem>();
        var garageServicesToRemove = new List<GarageLookupServiceItem>();
        foreach (var garage in batch)
        {
            try
            {
                var rdwServices = _allRDWServices
                    .Where(y => y.Volgnummer == garage.Volgnummer)
                    .SelectMany(y => y.RelatedServiceItems);

                // garage is useless if it doesn't provide any service
                if (rdwServices?.Any() != true)
                {
                    continue;
                }

                var storedGarage = storedGarages.FirstOrDefault(x => x.Identifier == garage.Volgnummer.ToString());
                if (storedGarage == null)
                {
                    storedGarage = await _garageService.CreateLookup(garage);
                    garagesToInsert.Add(storedGarage);
                    _maxInsertAmount--;
                }
                else
                {
                    var needToUpdate = await _garageService.NeedToUpdate(garage, storedGarage);
                    if (needToUpdate)
                    {
                        storedGarage = await _garageService.UpdateLookup(garage, storedGarage);
                        garagesToUpdate.Add(storedGarage);
                        _maxUpdateAmount--;
                    }
                }

                // Always check the services as they can change very often
                var (itemsToInsert, itemsToUpdate, itemsToRemove) = await _garageService.UpsertLookupServices(storedGarage.Services, rdwServices, storedGarage.Identifier);
                if (itemsToInsert?.Any() == true)
                {
                    garageServicesToInsert.AddRange(itemsToInsert);
                }
                
                if (itemsToUpdate?.Any() == true)
                {
                    garageServicesToUpdate.AddRange(itemsToUpdate);
                }

                if (itemsToRemove?.Any() == true)
                {
                    garageServicesToRemove.AddRange(itemsToRemove);
                }

                if (_maxInsertAmount <= 0 && _maxUpdateAmount <= 0)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                request.QueueService.LogError($"[{garage.Naambedrijf}]:{ex.Message}");
            }
        }

        return (garagesToInsert, garagesToUpdate, garageServicesToInsert, garageServicesToUpdate, garageServicesToRemove);
    }

    private bool ShouldStopProcessing(UpsertGarageLookupsCommand request, CancellationToken cancellationToken)
    {
        return (HasReachedInsertLimit(request) && HasReachedUpdateLimit(request)) || cancellationToken.IsCancellationRequested;
    }

    private bool HasReachedInsertLimit(UpsertGarageLookupsCommand request)
    {
        return (request.MaxInsertAmount > 0 && _maxInsertAmount <= 0) || (request.MaxInsertAmount == -1 && _maxInsertAmount == 0);
    }

    private bool HasReachedUpdateLimit(UpsertGarageLookupsCommand request)
        {
            return (request.MaxUpdateAmount > 0 && _maxUpdateAmount <= 0) || (request.MaxUpdateAmount == -1 && _maxUpdateAmount == 0);
        }

}
