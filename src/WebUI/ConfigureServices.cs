using System.Reflection;
using App.Authorization;
using App.Requirement;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Hangfire;
using AutoHelper.Hangfire.MediatR;
using AutoHelper.Infrastructure.Common.Interfaces;
using AutoHelper.Infrastructure.Persistence;
using AutoHelper.Infrastructure.Services;
using AutoHelper.WebUI.Filters;
using AutoHelper.WebUI.Services;
using FluentValidation.AspNetCore;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using ZymLabs.NSwag.FluentValidation;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseDeveloperPageExceptionFilter()
                .AddHttpContextAccessor()
                .AddAuthenticationServices(configuration)
                .AddAuthorizationServices()
                .AddControllerServices(configuration)
                .AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();

        services.AddAuthentication()
            .AddIdentityServerJwt();

        return services;
    }

    private static IServiceCollection AddControllerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        services.AddControllersWithViews(options =>
            options.Filters.Add<ApiExceptionFilterAttribute>())
                .AddFluentValidation(x => x.AutomaticValidationEnabled = false);

        services.AddScoped<FluentValidationSchemaProcessor>(provider =>
        {
            var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
            var loggerFactory = provider.GetService<ILoggerFactory>();

            return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
        });

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddOpenApiDocument((configure, serviceProvider) =>
        {
            var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<FluentValidationSchemaProcessor>();

            // Add the fluent validations schema processor
            configure.SchemaProcessors.Add(fluentValidationSchemaProcessor);

            configure.Title = "AutoHelper API";
            configure.Version = "v1";
            configure.XmlDocumentationFormatting = Namotion.Reflection.XmlDocsFormattingMode.Markdown;
            configure.IgnoreObsoleteProperties = true;
            configure.GenerateXmlObjects = true;
            configure.GenerateExamples = true;

            var audience = configuration["OAuth0:Audience"];
            configure.AddSecurity("OAuth2", new[] { audience }, new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2,
                Flow = OpenApiOAuth2Flow.Implicit,
                AuthorizationUrl = $"{configuration["OAuth0:Domain"]}/authorize",
                TokenUrl = $"{configuration["OAuth0:Domain"]}/oauth/token"
            });
        });

        return services;
    }

    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var audience = configuration["OAuth0:Audience"];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["OAuth0:Domain"];
                    options.Audience = audience;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true
                    };
                });

        return services;
    }

    private static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("read:admin-messages", policy =>
            {
                policy.Requirements.Add(new RbacRequirement("read:admin-messages"));
            });

            // Add a new policy for the Garage role
            options.AddPolicy("AdminRole", policy => policy.RequireRole("Admin"));
            options.AddPolicy("GarageRole", policy => policy.RequireRole("Admin", "Garage"));
        });

        services.AddSingleton<IAuthorizationHandler, RbacHandler>();

        return services;
    }

    public static WebApplication UseWebUIServices(this WebApplication app)
    {
        // API is always accessible via HTTPS
        app.UseMigrationsEndPoint();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            //app.UseHsts();
        }

        // Initialise and seed database
        using (var scope = app.Services.CreateScope())
        {
            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            initialiser.InitialiseAsync().Wait();
            initialiser.SeedAsync().Wait();
            //initialiser.StartSyncTasksWhenEmpty().Wait();
        }

        app.UseHealthChecks("/health");
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseOpenApi(configure =>
        {
            configure.DocumentName = "v1";
            configure.Path = "/swagger/v1/swagger.json";
        });

        app.UseSwaggerUi3(settings =>
        {
            settings.OAuth2Client = new OAuth2ClientSettings
            {
                ClientId = app.Configuration["OAuth0:ClientID"],
                AppName = "AutoHelper API",
                UsePkceWithAuthorizationCodeGrant = false,
                AdditionalQueryStringParameters =
                {
                    { "audience", app.Configuration["OAuth0:Audience"] },
                }
            };
            settings.Path = "/swagger";
            settings.DocumentPath = "/swagger/v1/swagger.json";
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });

        return app;
    }

}

