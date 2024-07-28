using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows;
using TagsagNyilvantarto.ViewModels;
using AsyncAwaitBestPractices;

namespace TagsagNyilvantarto
{
    internal class TagsagNyilvantartoBootStrap : BootstrapperBase
    {
        public TagsagNyilvantartoBootStrap()
        {
            Initialize();
        }

        private readonly SimpleContainer _container = new SimpleContainer();
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void Configure()
        {
            _container.Singleton<MsgBoxService>();
            _container.Singleton<IMsgBoxService, MsgBoxService>();
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<DataAccess>();
            _container.Singleton<TagokViewModel>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            Screen startupUi = _container.GetInstance<TagokViewModel>();
            ShellViewModel shellViewModel = ShellViewModel.CreateShellViewModel(startupUi);
            _container.RegisterInstance(shellViewModel.GetType(), "shellvm", shellViewModel);
            IWindowManager windowManager = _container.GetInstance<IWindowManager>();
            windowManager.ShowWindowAsync(shellViewModel).SafeFireAndForget(ex => IoC.Get<IMsgBoxService>().ShowError("Hiba az ablak megnyitása során! {newLine}Error: {exMessage}", Environment.NewLine, ex.Message), continueOnCapturedContext: true);
            shellViewModel.ShowStartupUI();
            //DisplayRootViewFor<ShellViewModel>();
        }
    }
}
