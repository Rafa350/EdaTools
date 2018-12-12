namespace MikroPic.EdaTools.v1.PanelEditor {

    using MikroPic.EdaTools.v1.PanelEditor.Services;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Services;
    using MikroPic.NetMVVMToolkit.v1.WindowState;
    using System.Windows;

    public partial class App: Application {

        protected override void OnStartup(StartupEventArgs e) {

            // Crea els serveis per l'aplicacio
            //
            IAppService appService = new AppService(this);
            IDialogService dlgService = DialogService.Instance;

            // Registra els serveix en el localitzador
            //
            ServiceLocator serviceLocator = ServiceLocator.Instance;
            serviceLocator.Register(typeof(IAppService), appService);
            serviceLocator.Register(typeof(IDialogService), dlgService);

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e) {

            WindowStateManager.Instance.Close();

            base.OnExit(e);
        }
    }
}
