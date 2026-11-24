using TimerApplication.Domain.Enums;

namespace TimerApplication.ApplicationLayer.Interface;

public interface IUserIO
{
    public int WindowWidth { get; }

    public void DisplayMessage(string message, MessageType type = MessageType.Default);

    public void DisplayMessageAt(int left, int top, string message, MessageType type = MessageType.Default);

    public string? ReadInput();

    public string ReadMaskedPassword();

    public void ClearOutput();
}
