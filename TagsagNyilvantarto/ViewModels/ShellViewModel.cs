using Caliburn.Micro;

namespace TagsagNyilvantarto.ViewModels
{
    internal class ShellViewModel : Conductor<object>
    {
        private IScreen _starupUi;
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
