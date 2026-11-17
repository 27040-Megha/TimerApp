namespace TimerApplication.ApplicationLayer.Interface;

public interface IUserIO
{
    public void DisplayMessage(string message, MessageType type = MessageType.Default);

    public string ReadInput();

    public string ReadMaskedPassword();

    public void ClearScreen();
}
