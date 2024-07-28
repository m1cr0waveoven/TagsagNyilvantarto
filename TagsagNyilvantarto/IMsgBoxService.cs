namespace TagsagNyilvantarto
{
    internal interface IMsgBoxService
    {
        bool AskForConfirmation(string message);
        void ShowError(string message);
        void ShowNotification(string message);
        void WriteMessageToFile(string message);
    }
}