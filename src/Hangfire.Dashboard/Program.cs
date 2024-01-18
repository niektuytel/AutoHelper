using AutoHelper.Messaging;
using AutoHelper.Hangfire;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Application.Vehicles.Commands.SyncVehicleLookups;
using AutoHelper.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using AutoHelper.Infrastructure.Persistence;
using AutoHelper.Infrastructure.Common.Interfaces;
using AutoHelper.Hangfire.Dashboard;

internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorPages();

        builder.Services.AddMessagingServices(builder.Configuration);
        builder.AddHangfireServices();
        builder.AddHangfireServerInstance();
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

        var app = builder.Build();
        app.MapRazorPages();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseRouting();

        app.UseEndpoints(endpoints => endpoints.MapRazorPages());

        using var scope = app.Services.CreateScope();
        app.UseHangfireServices(scope);
        app.UseHangfireDashboardInstance(matchPath: "");

        app.Run();
    }

}