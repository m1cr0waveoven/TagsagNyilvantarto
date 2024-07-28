using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TagsagNyilvantarto;

internal sealed class MsgBoxService : IMsgBoxService
{
    public void ShowNotification(string message)
    {
        _ = MessageBox.Show(message, "Üzenet", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowError(string message) => _ = MessageBox.Show(message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);

    public void ShowError(string message, params string[] args)
    {
        message = string.Format(message, args);
        ShowError(message);
    }
    public bool AskForConfirmation(string message) =>
         MessageBox.Show(message, "Biztos?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    public void WriteMessageToFile(string message, [CallerMemberName] string callerName = default)
    {
        using StreamWriter streamWriter = new("log.txt", true);
        streamWriter.WriteLine($"[{callerName}] - {message} - {System.DateTime.Now}");
    }
}
