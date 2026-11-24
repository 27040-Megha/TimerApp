using Microsoft.Extensions.DependencyInjection;
using TimerApplication;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;
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