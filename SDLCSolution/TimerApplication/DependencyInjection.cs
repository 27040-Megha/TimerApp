using Microsoft.Extensions.DependencyInjection;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.InfrastructureLayer;
using TimerApplication.PresentationLayer.IO;
using TimerApplication.PresentationLayer.View;

namespace TimerApplication;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddTransient<UserService>();
        services.AddSingleton<IUserIO, ConsoleUserIO>();
        services.AddTransient<ViewOperator>();
        return services;
    }
}
