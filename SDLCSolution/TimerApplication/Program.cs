using System;
using Microsoft.Extensions.DependencyInjection;
using TimerApplication.ApplicationLayer.Domain.Enums;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.InfrastructureLayer;
using TimerApplication.PresentationLayer.IO;
using TimerApplication.PresentationLayer.View;

namespace SDLC;

public class Program
{
    public static void Main(string[] args)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddTransient<UserService>();
        services.AddSingleton<IUserIO, ConsoleUserIO>();
        services.AddTransient<ViewOperator>();
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        ViewOperator viewOperator = serviceProvider.GetRequiredService<ViewOperator>();
        IUserIO userIO = serviceProvider.GetRequiredService<IUserIO>();
        while (true)
        {
            try
            {
                viewOperator.Run();
                break;
            }
            catch (Exception ex)
            {
                userIO.DisplayMessage($"Something went wrong: {ex.Message}", MessageType.Error);
                userIO.DisplayMessage("Returnting to Main menu", MessageType.Warning);
            }
        }
    }
}