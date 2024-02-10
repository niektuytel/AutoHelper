using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Hangfire;
using AutoHelper.Hangfire.Dashboard;
using AutoHelper.Messaging;

internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var inDev = builder.Environment.IsDevelopment();
        builder.Services.AddRazorPages();

        builder.Services.AddMessagingServices(builder.Configuration);
        builder.Services.AddHangfireServices(builder.Configuration, inDev);
        builder.Services.AddHangfireServerInstance();
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

        var app = builder.Build();
        app.MapRazorPages();

        if (inDev)
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