using System.Collections.Generic;
using TimerApplication.ApplicationLayer.Domain.Enums;
using TimerApplication.Domain.Enums;

namespace TimerApplication.ApplicationLayer.Interface;

public interface IUserIO
{
    public void DisplayMessage(string message, MessageType type = MessageType.Default);

    void DisplayPanel(DisplayRegion region, string title, IReadOnlyList<string> items);

    public string ReadInput();

    public string ReadMaskedPassword();

    public void ClearOutput();
}
