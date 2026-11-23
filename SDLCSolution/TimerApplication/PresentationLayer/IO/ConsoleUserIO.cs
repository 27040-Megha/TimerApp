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

    /// <summary>
    /// Displays message at specified position in the Console using SetCursorPosition
    /// </summary>
    /// <param name="left">Left-position of cursor</param>
    /// <param name="top">Top-position of cursor</param>
    /// <param name="message">Message to be displayed</param>
    /// <param name="messageType">Message Type to decide the color of the text</param>
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

    /// <summary>
    /// Divides the Console into 3 parts - Main Menu, Side Bar1 (Primary Sidebar), Side Bar2 (Secondary Sidebar)
    /// Primary bar - Displays all tasks added by the user
    /// Secondary bar - Displays current active task logs
    /// </summary>
    /// <param name="region">Primary or Secondary Side bar</param>
    /// <param name="title">Title for the Side bar</param>
    /// <param name="items">Items to be displayed in the region</param>
    public void DisplayPanel(DisplayRegion region, string title, IReadOnlyList<string> items)
    {
        // Set column starting position based on the Console Window size
        int col2Left = Math.Max(45, (int)(WindowWidth * 0.2));
        int col3Left = Math.Max(82, (int)(WindowWidth * 0.5));

        switch (region)
        {
            case DisplayRegion.PrimarySidebar:
                {
                    // Calculate width of Primary Side bar based on Console Window width
                    int width = Math.Min(col3Left, WindowWidth) - col2Left - 2;
                    if (width > 10)
                    {
                        DivideColumn(col2Left, WindowTop, width, title, items);
                    }
                    break;
                }
            case DisplayRegion.SecondarySidebar:
                {
                    // Calculate width of Secondary Side bar based on Console Window width
                    int width = WindowWidth - col3Left - 1;
                    if (width > 10)
                    {
                        DivideColumn(col3Left, WindowTop, width, title, items);
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// Displays messages within specified boundaries
    /// </summary>
    private void DivideColumn(int left, int top, int width, string title, IReadOnlyList<string> items)
    {
        lock (_consoleLock)
        {
            DisplayMessageAt(left, top, title.PadRight(width), MessageType.Info);

            for (int i = 0; i < items.Count; i++)
            {
                string text = items[i].Length > width ? items[i].Substring(0, width) : items[i];
                DisplayMessageAt(left, top + i + 1, text.PadRight(width));
            }

            DisplayMessageAt(left, top + items.Count + 1, new string(' ', width));
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
