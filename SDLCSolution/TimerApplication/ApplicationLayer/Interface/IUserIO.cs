using TimerApplication.Domain.Enums;

namespace TimerApplication.ApplicationLayer.Interface;

public interface IUserIO
{
    public int WindowWidth { get; }

    public int WindowHeight { get; }

    public void DisplayMessage(string message, MessageType messageType = MessageType.Default);

    public void DisplayPrompt(string message, MessageType messageType = MessageType.Default);

    void DisplayPanel(DisplayRegion region, string title, IReadOnlyList<string> items);

    public string? ReadInput();

    public string ReadMaskedPassword();

    public void ClearOutput();
}
