using System.IO;
using System.Windows;

namespace TagsagNyilvantarto
{
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
        public void WriteMessageToFile(string message)
        {
            using (StreamWriter streamWriter = new StreamWriter("log", true))
            {
                streamWriter.WriteLine(message + ";" + System.DateTime.Now);
            }
        }
    }
}
