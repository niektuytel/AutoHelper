using System.IdentityModel.Tokens.Jwt;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Hangfire;
using AutoHelper.Infrastructure.Persistence;
using AutoHelper.Messaging;
using AutoHelper.WebUI;
using AutoHelper.WebUI.Filters;
using AutoHelper.WebUI.Services;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using NSwag;
using NSwag.AspNetCore;
using ZymLabs.NSwag.FluentValidation;

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

        builder.Services.AddHealthChecks();
        builder.Services.AddHangfireServices(builder.Configuration, builder.Environment.IsDevelopment());
        builder.Services.AddMessagingServices(builder.Configuration);
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);


        // [BUILD]
        var app = builder.Build();
        app.MapRazorPages();

        if (app.Environment.IsDevelopment())
        {
            //app.UseMigrationsEndPoint();
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
            endpoints.MapFallbackToFile("index.html");
        });

        app.UseHangfireServices();
        app.Services.UseInfrastructureServices();

        app.Run();
    }
}