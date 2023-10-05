using System.Globalization;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.SyncGarageLookups;
using AutoHelper.Infrastructure.Common;
using AutoHelper.Infrastructure.Persistence;
using AutoHelper.Hangfire;
using Hangfire;
using MediatR;
using WebUI.Extensions;
using Microsoft.Extensions.Configuration;
using AutoHelper.Hangfire.MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHangfireServices(builder.Configuration);
builder.Services.AddWebUIServices(builder.Configuration);

var app = builder.Build();

// Use service
using var scope = app.Services.CreateScope();
app.UseHangfireServices(scope);
app.UseWebUIServices();

// Initialize (+ Run) Recurrence jobs
var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
var enableRecurringjobs = app.Configuration["Hangfire:EnableRecurringJobs"];
_ = bool.TryParse(enableRecurringjobs, out var isRecurring);

if (mediator != null)
{
    mediator.RecurringJobWeekly($"{nameof(SyncGarageLookupsCommand)}", new SyncGarageLookupsCommand(), isRecurring);
}


app.Run();
