using System.Runtime.CompilerServices;

namespace TagsagNyilvantarto;

internal interface IMsgBoxService
{
    bool AskForConfirmation(string message);
    void ShowError(string message);
    void ShowError(string message, params string[] args);
    void ShowNotification(string message);
    void WriteMessageToFile(string message, [CallerMemberName] string callerName = default);
}