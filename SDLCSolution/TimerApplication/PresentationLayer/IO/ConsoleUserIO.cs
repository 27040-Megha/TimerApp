using System;
using TimerApplication.ApplicationLayer.Domain.Enums;
using TimerApplication.ApplicationLayer.Interface;

namespace TimerApplication.PresentationLayer.IO;

public class ConsoleUserIO : IUserIO
{
    /// <summary>
    /// Clears the currently visible screen using Console.Clear() and using \x1b[3J, erase all lines in the scrollback buffer
    /// </summary>
    public void ClearScreen()
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        Console.Clear();
    }

    public void DisplayMessage(string message, MessageType type = MessageType.Default)
    {
        ApplyColor(type);
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void ApplyColor(MessageType type)
    {
        Console.ForegroundColor = type switch
        {
            MessageType.Info => ConsoleColor.Cyan,
            MessageType.Success => ConsoleColor.Green,
            MessageType.Warning => ConsoleColor.Yellow,
            MessageType.Error => ConsoleColor.Red,
            _ => ConsoleColor.White
        };
    }

    public string ReadInput()
    {
        return Console.ReadLine();
    }

    public string ReadMaskedPassword()
    {
        string password = "";
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }

            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b\b");
            }
            else if(!char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        }

        Console.WriteLine();
        return password;
    }
}
