﻿using AsyncAwaitBestPractices;
using Caliburn.Micro;
using System;

namespace TagsagNyilvantarto.ViewModels
{
    internal sealed class ShellViewModel : Conductor<object>
    {
        private readonly IScreen _starupUi;

        public static ShellViewModel CreateShellViewModel(Screen startupUi)
        {
            var shellViewModel = new ShellViewModel(startupUi);
            // Aditional creation logic
            return shellViewModel;
        }
        private ShellViewModel(Screen startupUi) => _starupUi = startupUi;


        public void ShowStartupUI()
        {
            ActivateItemAsync(_starupUi)
                .SafeFireAndForget(ex =>
                    IoC.Get<IMsgBoxService>().ShowError("Hiba történt a kezdő ablak megnyitása során! {newLine}Error: {exMessage}", Environment.NewLine, ex.Message), true);
        }
    }
}
