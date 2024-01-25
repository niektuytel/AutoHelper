using System.Globalization;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Infrastructure.Common;
using AutoHelper.Infrastructure.Persistence;
using AutoHelper.Hangfire;
using AutoHelper.Messaging;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Configuration;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Application.Vehicles.Commands.SyncVehicleLookups;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

builder.Services.AddMessagingServices(builder.Configuration);
builder.AddHangfireServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUIServices(builder.Configuration);

var app = builder.Build();

// Use service
using var scope = app.Services.CreateScope();
app.MapRazorPages();

app.UseHangfireServices(scope);
app.UseWebUIServices();

app.Run();
