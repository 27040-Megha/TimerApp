using System;
using Microsoft.Extensions.DependencyInjection;
using TimerApplication;
using TimerApplication.ApplicationLayer.Domain.Enums;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.PresentationLayer.View;

namespace SDLC;

public class Program
{
    public static void Main(string[] args)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddApplicationServices();
        using ServiceProvider serviceProvider = services.BuildServiceProvider();        
        ViewOperator viewOperator = serviceProvider.GetRequiredService<ViewOperator>();
        IUserIO userIO = serviceProvider.GetRequiredService<IUserIO>();

        AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
        {
            viewOperator.SaveCurrentActiveUser();
        };

        System.Timers.Timer timer = new System.Timers.Timer();
        timer.Interval = 1000;
        timer.Elapsed += (sender, e) =>
        {
            viewOperator.DisplayCurrentUsersTask();
        };
        timer.Start();


        while (true)
        {
            try
            {
                viewOperator.Run();
                break;
                timer.Stop();
            }
            catch (Exception ex)
            {
                userIO.DisplayMessage($"Something went wrong: {ex.Message}", MessageType.Error);
                userIO.DisplayMessage("Returnting to Main menu", MessageType.Warning);
            }
        }

        timer.Stop();
    }
}