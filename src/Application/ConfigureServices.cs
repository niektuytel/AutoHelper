using System.Reflection;
using AutoHelper.Application.Common.Behaviours;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Services;
using AutoHelper.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        services.AddScoped<IAesEncryptionService, AesEncryptionService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IVehicleTimelineService, VehicleTimelineService>();
        services.AddScoped<IGarageService, GarageService>();
        return services;
    }

}
