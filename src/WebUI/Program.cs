using System.Globalization;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Infrastructure.Common;
using AutoHelper.Infrastructure.Persistence;
using AutoHelper.Hangfire;
using AutoHelper.Whatsapp;
using Hangfire;
using MediatR;
using WebUI.Extensions;
using Microsoft.Extensions.Configuration;
using AutoHelper.Hangfire.MediatR;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMessagingServices(builder.Configuration);
builder.Services.AddHangfireServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUIServices(builder.Configuration);

var app = builder.Build();

// Use service
using var scope = app.Services.CreateScope();
app.UseHangfireServices(scope);
app.UseWebUIServices();

app.Run();
