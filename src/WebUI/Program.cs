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
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();


// This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
// By default, the claims mapping will map claim names in the old format to accommodate older SAML applications.
// For instance, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' instead of 'roles' claim.
// This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Adds Microsoft Identity platform (AAD v2.0) support to protect this Api
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
        {
            builder.Configuration.Bind("AzureAdB2C", options);
        },
        options => { builder.Configuration.Bind("AzureAdB2C", options); }
    );

builder.Services.AddAuthorization TODO: https://stackoverflow.com/questions/64193349/idw10201-neither-scope-or-roles-claim-was-found-in-the-bearer-token(options =>
{
    options.AddPolicy("GarageReadWritePolicy", policy =>
        policy.RequireScope("Garage.ReadWrite"));

    options.AddPolicy("UserReadWritePolicy", policy =>
        policy.RequireScope("User.ReadWrite"));
});

//builder.Services.AddControllers();

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
