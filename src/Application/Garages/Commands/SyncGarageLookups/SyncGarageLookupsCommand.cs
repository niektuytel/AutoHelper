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
using AutoHelper.Application.Vehicles.Commands.SyncVehicleTimelines;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using Hangfire.Server;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Application.Garages.Commands.UpsertGarageLookups;


public record SyncGarageLookupsCommand : IQueueRequest
{
    public const int InsertAll = -1;
    public const int UpdateAll = -1;
    public const int DefaultStartingRowIndex = 0;
    public const int DefaultEndingRowIndex = -1;

    public SyncGarageLookupsCommand(
        int startRowIndex = DefaultStartingRowIndex,
        int endRowIndex = DefaultEndingRowIndex,
        int maxInsertAmount = InsertAll,
        int maxUpdateAmount = UpdateAll,
        int batchSize = 10
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

public class UpsertGarageLookupsCommandHandler : IRequestHandler<SyncGarageLookupsCommand>
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

    public async Task<Unit> Handle(SyncGarageLookupsCommand request, CancellationToken cancellationToken)
    {
        int totalRecords = await CalculateTotalRecords(request, cancellationToken);
        SetMaxInsertAndUpdateAmounts(request, totalRecords);

        _allRDWServices = await _garageService.GetRDWServices();
        LogInformationBasedOnAmount(request);

        int numberOfBatches = CalculateNumberOfBatches(request.BatchSize, totalRecords);
        for (int i = 0; i < numberOfBatches; i++)
        {
            var start = request.StartRowIndex + (i * request.BatchSize);
            if (ShouldStopProcessing(start, request, cancellationToken))
            {
                break;
            }

            var batch = await _garageService.GetRDWCompanies(i, request.BatchSize);

            var (garageItemsToInsert, garageItemsToUpdate, garageServicesToInsert, garageServicesToRemove) = await ProcessGarageBatchAsync(batch, request, cancellationToken);

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

            if (garageServicesToRemove.Any())
            {
                await _dbContext.BulkRemoveAsync(garageServicesToRemove, cancellationToken);
            }

            var line = $"[{(start + request.BatchSize)}/{request.EndRowIndex}] insert: {garageItemsToInsert.Count} | update: {garageItemsToUpdate.Count} items";
            request.QueueService.LogInformation(line, inProgressBar: true);
        }

        var message = $"Task finished.";
        request.QueueService.LogInformation(message);
        return Unit.Value;

    }

    private async Task<int> CalculateTotalRecords(SyncGarageLookupsCommand request, CancellationToken cancellationToken)
    {
        int totalRecords = await _garageService.GetRDWCompaniesCount();

        // get only omount from start to end row index
        if (request.EndRowIndex > 0)
        {
            totalRecords = request.EndRowIndex - request.StartRowIndex;
        }
        else
        {
            request.EndRowIndex = totalRecords;
        }

        return totalRecords;
    }

    private void SetMaxInsertAndUpdateAmounts(SyncGarageLookupsCommand request, int totalRecords)
    {
        _maxInsertAmount = DetermineMaxAmount(request.MaxInsertAmount, totalRecords);
        _maxUpdateAmount = DetermineMaxAmount(request.MaxUpdateAmount, totalRecords);
    }

    private int DetermineMaxAmount(int requestedAmount, int totalRecords)
    {
        return requestedAmount == SyncGarageLookupsCommand.InsertAll ? totalRecords : requestedAmount;
    }

    private void LogInformationBasedOnAmount(SyncGarageLookupsCommand request)
    {
        request.QueueService.LogInformation($"Start upsert rows from {request.StartRowIndex} to {request.EndRowIndex}");

        if (request.MaxInsertAmount == SyncGarageLookupsCommand.InsertAll)
        {
            request.QueueService.LogInformation($"Insert all available garages");
        }
        else
        {
            request.QueueService.LogInformation($"Insert {_maxInsertAmount} garages");
        }

        if (request.MaxUpdateAmount == SyncGarageLookupsCommand.UpdateAll)
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

    private async Task<(List<GarageLookupItem> InsertLookupItems, List<GarageLookupItem> UpdateLookupItems, List<GarageLookupServiceItem> InsertServiceItems, List<GarageLookupServiceItem> RemoveServiceItems)> ProcessGarageBatchAsync(IEnumerable<RDWCompany> batch, SyncGarageLookupsCommand request, CancellationToken cancellationToken)
    {
        var storedGarages = await _dbContext.GarageLookups
            .AsNoTracking()
            .Include(x => x.Services)
            .Where(x => batch.Select(y => y.Volgnummer.ToString()).Contains(x.Identifier))
            .ToListAsync(cancellationToken);

        var garagesToInsert = new List<GarageLookupItem>();
        var garagesToUpdate = new List<GarageLookupItem>();
        var garageServicesToInsert = new List<GarageLookupServiceItem>();
        var garageServicesToRemove = new List<GarageLookupServiceItem>();
        foreach (var company in batch)
        {
            try
            {
                var identifier = company.Volgnummer.ToString();
                var storedGarage = storedGarages.FirstOrDefault(x => x.Identifier == identifier);

                var (itemToInsert, itemToUpdate) = await _garageService.UpsertLookup(storedGarage, company);
                if (itemToInsert != null || itemToUpdate != null)
                {
                    if (itemToInsert != null)
                    {
                        garagesToInsert.Add(itemToInsert);
                        _maxInsertAmount--;
                    }

                    if (itemToUpdate != null)
                    {
                        garagesToUpdate.Add(itemToUpdate);
                        _maxUpdateAmount--;
                    }

                    var rdwServices = _allRDWServices
                        .Where(y => y.Volgnummer.ToString() == identifier)
                        .SelectMany(y => y.RelatedServiceItems);

                    // NOTE: we use them as well, we do an sort on the services amount and they will been ignored
                    // If they like to be more accessable by the user, they have to give information about htere services.
                    //if (rdwServices?.Any() != true) continue;

                    // Always check the services as they can change very often
                    var (itemsToInsert, itemsToRemove) = await _garageService.UpsertLookupServices(storedGarage?.Services, rdwServices, identifier);
                    if (itemsToInsert?.Any() == true)
                    {
                        garageServicesToInsert.AddRange(itemsToInsert);
                    }

                    if (itemsToRemove?.Any() == true)
                    {
                        garageServicesToRemove.AddRange(itemsToRemove);
                    }
                }

                if (_maxInsertAmount <= 0 && _maxUpdateAmount <= 0)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                request.QueueService.LogError($"[{company.Naambedrijf}]:{ex.Message}");
            }
        }

        return (garagesToInsert, garagesToUpdate, garageServicesToInsert, garageServicesToRemove);
    }

    private bool ShouldStopProcessing(int startIndex, SyncGarageLookupsCommand request, CancellationToken cancellationToken)
    {
        if (startIndex >= request.EndRowIndex)
        {
            return true;
        }

        return (HasReachedInsertLimit(request) && HasReachedUpdateLimit(request)) || cancellationToken.IsCancellationRequested;
    }

    private bool HasReachedInsertLimit(SyncGarageLookupsCommand request)
    {
        return (request.MaxInsertAmount > 0 && _maxInsertAmount <= 0) || (request.MaxInsertAmount == -1 && _maxInsertAmount == 0);
    }

    private bool HasReachedUpdateLimit(SyncGarageLookupsCommand request)
    {
        return (request.MaxUpdateAmount > 0 && _maxUpdateAmount <= 0) || (request.MaxUpdateAmount == -1 && _maxUpdateAmount == 0);
    }

}
