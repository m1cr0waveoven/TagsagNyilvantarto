using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace TagsagNyilvantarto
{
    internal sealed class MsgBoxService : IMsgBoxService
    {
        //[DllImport("User32.dll", BestFitMapping = true, CharSet = CharSet.Unicode)]
        //public static extern int MessageBox(IntPtr h, string m, string c, int type);

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetActiveWindow();
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
