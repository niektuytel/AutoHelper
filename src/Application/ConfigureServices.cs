using System.Reflection;
using AutoHelper.Application.Common.Behaviours;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Infrastructure.Services;
using FluentValidation;
using MediatR;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        services.AddTransient<IVehicleService, VehicleService>();
        services.AddTransient<IVehicleTimelineService, VehicleTimelineService>();
        services.AddTransient<IGarageService, GarageService>();
        return services;
    }

}
