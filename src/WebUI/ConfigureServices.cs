using App.Authorization;
using App.Requirement;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Infrastructure.Persistence;
using AutoHelper.WebUI.Filters;
using AutoHelper.WebUI.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services, IConfiguration configuration)
    {
        var audience = configuration["OAuth0:AUTH0_AUDIENCE"];
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddControllersWithViews(options =>
            options.Filters.Add<ApiExceptionFilterAttribute>())
                .AddFluentValidation(x => x.AutomaticValidationEnabled = false);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["OAuth0:AUTH0_DOMAIN"];
                options.Audience = audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("read:admin-messages", policy =>
            {
                policy.Requirements.Add(new RbacRequirement("read:admin-messages"));
            });
        });

        services.AddSingleton<IAuthorizationHandler, RbacHandler>();

        services.AddSpaStaticFiles(configuration =>
        {
            // In production, the React files will be served from this directory
            configuration.RootPath = "ClientApp/build";
        });


        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddOpenApiDocument(configure =>
        {
            configure.Title = "AutoHelper API";

            configure.AddSecurity("OAuth2", new[] { audience }, new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2,
                Flow = OpenApiOAuth2Flow.Implicit,
                AuthorizationUrl = $"{configuration["OAuth0:AUTH0_DOMAIN"]}/authorize",
                TokenUrl = $"{configuration["OAuth0:AUTH0_DOMAIN"]}/oauth/token"
            });
        });

        return services;
    }

    public static WebApplication UseWebUIServices(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();

            // Initialise and seed database
            using (var scope = app.Services.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
                initialiser.InitialiseAsync().Wait();
                initialiser.SeedAsync().Wait();
            }
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        if (!app.Environment.IsDevelopment())
        {
            app.UseSpaStaticFiles();
        }

        app.UseOpenApi(configure =>
        {
            configure.DocumentName = "v1";
            configure.Path = "/swagger/v1/swagger.json";
        });

        app.UseSwaggerUi3(settings =>
        {
            settings.OAuth2Client = new OAuth2ClientSettings
            {
                ClientId = app.Configuration["OAuth0:AUTH0_CLIENTID"],
                AppName = "AutoHelper API",
                UsePkceWithAuthorizationCodeGrant = false,
                AdditionalQueryStringParameters =
                {
                    { "audience", app.Configuration["OAuth0:AUTH0_AUDIENCE"] }
                }
            };
            settings.Path = "/swagger";
            settings.DocumentPath = "/swagger/v1/swagger.json";
        });

        app.UseRouting();
        app.UseAuthentication();
        app.UseIdentityServer();
        app.UseAuthorization();
        app.UseHealthChecks("/health");
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp";
            if (app.Environment.IsDevelopment())
            {
                spa.UseReactDevelopmentServer(npmScript: "start");
            }
        });

        return app;
    }
}
