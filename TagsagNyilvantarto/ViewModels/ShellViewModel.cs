using Caliburn.Micro;

namespace TagsagNyilvantarto.ViewModels
{
    class ShellViewModel : Conductor<object>
    {
        IScreen _starupUi;
        public ShellViewModel(Screen startupUi)
        {
            ShowStartupUI();
            _starupUi = startupUi;
        }

        public void ShowStartupUI()
        {
            ActivateItem(_starupUi);
        }
    }
}
