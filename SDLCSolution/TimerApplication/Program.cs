using System;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.InfrastructureLayer;
using TimerApplication.PresentationLayer.View;

namespace SDLC;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var userRepository = new UserRepository();
            var userService = new UserService(userRepository);
            var consoleOperator = new ConsoleOperator(userService);
            consoleOperator.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception Caught: {ex.Message}");
        }
    }
}