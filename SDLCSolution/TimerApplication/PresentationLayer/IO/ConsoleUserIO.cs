using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;

namespace TimerApplication.PresentationLayer.IO;

public class ConsoleUserIO : IUserIO
{
    private readonly object _consoleLock = new();

    public int WindowWidth => Console.WindowWidth;

    /// <summary>
    /// Clears the currently visible screen using Console.Clear() and using \x1b[3J, erase all lines in the scrollback buffer
    /// </summary>
    public void ClearOutput()
    {
        lock (_consoleLock)
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            Console.Clear();
        }
    }

    public void DisplayMessage(string message, MessageType type = MessageType.Default)
    {
        lock (_consoleLock)
        {
            ApplyColor(type);
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    public void DisplayMessageAt(int left, int top, string message, MessageType messageType)
    {
        lock (_consoleLock)
        {
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;
            Console.SetCursorPosition(left, top);
            this.DisplayMessage(message, messageType);
            Console.SetCursorPosition(currentLeft, currentTop);
        }
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

    public string? ReadInput()
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
                lock (_consoleLock)
                {
                    Console.Write("\b\b");
                }
            }
            else if(!char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
                lock (_consoleLock)
                {
                    Console.Write("*");
                }
            }
        }

        DisplayMessage(string.Empty);
        return password;
    }
}
