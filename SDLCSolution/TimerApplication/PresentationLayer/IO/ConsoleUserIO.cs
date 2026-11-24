using TimerApplication.ApplicationLayer.Interface;
using TimerApplication.Domain.Enums;

namespace TimerApplication.PresentationLayer.IO;

public class ConsoleUserIO : IUserIO
{
    private const int MinPrimarySidebarLeft = 45;
    private const double PrimarySidebarLeftRatio = 0.2;
    private const int MinSecondarySidebarLeft = 82;
    private const double SecondarySidebarLeftRatio = 0.5;
    private const int MinSidebarWidth = 10;

    private readonly object _consoleLock = new();

    public int WindowWidth => Console.WindowWidth;

    public int WindowHeight => Console.WindowHeight;

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
    /// Divides the Console into 3 parts - Main Menu, Primary Sidebar (all tasks), and Secondary Sidebar (active task logs)
    /// </summary>
    public void DisplayPanel(DisplayRegion region, string title, IReadOnlyList<string> items)
    {
        int primarySidebarLeft = Math.Max(MinPrimarySidebarLeft, (int)(WindowWidth * PrimarySidebarLeftRatio));
        int secondarySidebarLeft = Math.Max(MinSecondarySidebarLeft, (int)(WindowWidth * SecondarySidebarLeftRatio));

        switch (region)
        {
            case DisplayRegion.PrimarySidebar:
                {
                    int width = Math.Min(secondarySidebarLeft, WindowWidth) - primarySidebarLeft - 2;
                    if (width > MinSidebarWidth)
                    {
                        RenderColumn(primarySidebarLeft, WindowTop, width, title, items);
                    }
                    break;
                }
            case DisplayRegion.SecondarySidebar:
                {
                    int width = WindowWidth - secondarySidebarLeft - 1;
                    if (width > MinSidebarWidth)
                    {
                        RenderColumn(secondarySidebarLeft, WindowTop, width, title, items);
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// Renders a sidebar column within the specified boundaries
    /// </summary>
    private void RenderColumn(int left, int top, int width, string title, IReadOnlyList<string> items)
    {
        lock (_consoleLock)
        {
            DisplayMessageAt(left, top, title.PadRight(width), MessageType.Info);

            for (int i = 0; i < items.Count; i++)
            {
                string text = items[i].Length > width ? items[i][..width] : items[i];
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

    public string? ReadInput()
    {
        return Console.ReadLine();
    }

    public string ReadMaskedPassword()
    {
        string password = "";
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }

            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                lock (_consoleLock)
                {
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(key.KeyChar))
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
