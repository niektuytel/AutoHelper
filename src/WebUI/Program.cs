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
using AutoHelper.WebUI;
using LinqKit;
using AutoHelper.WebUI.Services;
using AutoHelper.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using ZymLabs.NSwag.FluentValidation;
using NSwag.AspNetCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorPages();

        // Authentication and Authorization
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAdB2C");
        builder.Services.AddAuthorization(o => Policies.AllPolicies.ForEach(p => o.AddPolicy(p.Key, pb => pb.RequireScope(p.Value))));
        builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

        // Custom API behavior + (Open)API Documentation
        builder.Services.AddControllersWithViews(options => options.Filters.Add<ApiExceptionFilterAttribute>());
        builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        var instance = builder.Configuration["AzureAdB2C:Instance"];
        var domain = builder.Configuration["AzureAdB2C:Domain"];
        var policy = builder.Configuration["AzureAdB2C:SignUpSignInPolicyId"];
        builder.Services.AddOpenApiDocument((configure, serviceProvider) =>
        {
            // Add the fluent validations schema processor
            var scope = serviceProvider.CreateScope();
            var validationRules = scope.ServiceProvider.GetService<IEnumerable<FluentValidationRule>>();
            var loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();
            var fluentSchema = new FluentValidationSchemaProcessor(scope.ServiceProvider, validationRules, loggerFactory);
            configure.SchemaProcessors.Add(fluentSchema);

            configure.Title = "AutoHelper API";
            configure.Version = "v1";
            configure.IgnoreObsoleteProperties = true;
            configure.XmlDocumentationFormatting = Namotion.Reflection.XmlDocsFormattingMode.Markdown;
            configure.GenerateXmlObjects = true;

            configure.AddSecurity("OAuth2", Policies.AllScopes.Values, new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2,
                Flow = OpenApiOAuth2Flow.Implicit,
                AuthorizationUrl = $"{instance}/{domain}/oauth2/v2.0/authorize?p={policy}",
                TokenUrl = $"{instance}/{domain}/oauth2/v2.0/token?p={policy}",
                Scopes = Policies.AllScopes
            });

        });

        // Database and Health Checks
        builder.Services
            .AddDatabaseDeveloperPageExceptionFilter()
            .AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        builder.AddHangfireServices();
        builder.Services.AddMessagingServices(builder.Configuration);
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);


        // [BUILD]
        var app = builder.Build();
        app.MapRazorPages();

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHealthChecks("/health");
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        var clientID = app.Configuration["AzureAdB2C:ClientId"];
        app.UseOpenApi();
        app.UseSwaggerUi3(settings =>
        {
            settings.OAuth2Client = new OAuth2ClientSettings
            {
                ClientId = clientID,
                AppName = "AutoHelper API"
            };
        });

        _ = app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            //endpoints.MapFallbackToFile("index.html");
        });


        // Handle scoped services
        using (var scope = app.Services.CreateScope())
        {
            app.UseHangfireServices(scope);

            // Database initialisation
            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            initialiser.InitialiseAsync().Wait();
            initialiser.SeedAsync().Wait();
        }

        app.Run();
    }
}