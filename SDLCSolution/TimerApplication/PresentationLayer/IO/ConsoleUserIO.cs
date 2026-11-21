using System;
using System.Collections.Generic;
using TimerApplication.ApplicationLayer.Domain.Enums;
using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;

namespace TimerApplication.PresentationLayer.IO;

public class ConsoleUserIO : IUserIO
{
    private readonly object _consoleLock = new();

    private int WindowWidth => Console.WindowWidth;

    private int WindowTop => Console.WindowTop;

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

    public void DisplayMessage(string message, MessageType messageType = MessageType.Default)
    {
        lock (_consoleLock)
        {
            ApplyColor(messageType);
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    public void DisplayMessageAtSameLine(string message, MessageType messageType = MessageType.Default)
    {
        lock (_consoleLock)
        {
            ApplyColor(messageType);
            Console.Write(message);
            Console.ResetColor();
        }
    }

    private void DisplayMessageAt(int left, int top, string message, MessageType messageType = MessageType.Default)
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

    public void DisplayPanel(DisplayRegion region, string title, IReadOnlyList<string> items)
    {
        int col2Left = Math.Max(45, (int)(WindowWidth * 0.28));
        int col3Left = Math.Max(82, (int)(WindowWidth * 0.52));

        switch (region)
        {
            case DisplayRegion.PrimarySidebar:
                {
                    int width = Math.Min(col3Left, WindowWidth) - col2Left - 2;
                    if (width > 10)
                    {
                        DivideColumn(col2Left, topOffset: WindowTop, width, title, items);
                    }
                    break;
                }
            case DisplayRegion.SecondarySidebar:
                {
                    int width = WindowWidth - col3Left - 1;
                    if (width > 10)
                    {
                        DivideColumn(col3Left, topOffset: WindowTop, width, title, items);
                    }
                    break;
                }
        }
    }

    private void DivideColumn(int left, int topOffset, int width, string title, IReadOnlyList<string> items)
    {
        lock (_consoleLock)
        {
            DisplayMessageAt(left, topOffset, title.PadRight(width), MessageType.Info);

            for (int i = 0; i < items.Count; i++)
            {
                string text = items[i].Length > width ? items[i].Substring(0, width) : items[i];
                DisplayMessageAt(left, topOffset + i + 1, text.PadRight(width));
            }

            DisplayMessageAt(left, topOffset + items.Count + 1, new string(' ', width));
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
                lock (_consoleLock)
                {
                    Console.Write("\b \b");
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
