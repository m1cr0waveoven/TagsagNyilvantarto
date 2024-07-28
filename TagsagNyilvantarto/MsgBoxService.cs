using System.IO;
using System.Windows;

namespace TagsagNyilvantarto
{
    class MsgBoxService : IMsgBoxService
    {
        public void ShowNotification(string message)
        {
            _ = MessageBox.Show(message, "Üzenet", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message)
        {
            _ = MessageBox.Show(message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool AskForConfirmation(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Biztos?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
        public void WriteMessageToFile(string message)
        {
            using (StreamWriter streamWriter = new StreamWriter("log", true))
            {
                streamWriter.WriteLine(message + ";" + System.DateTime.Now);
            }
        }
    }
}
