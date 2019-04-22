namespace MikroPic.EdaTools.v1.PanelEditor.ViewModel {

    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.PanelEditor.Services;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Services;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;

    public sealed class PanelEditorViewModel: ViewModelBase {

        private readonly IAppService appService;

        public PanelEditorViewModel(ViewModelBase parent):
            base(parent) {

            ServiceLocator serviceLocator = ServiceLocator.Instance;
            appService = serviceLocator.GetService<IAppService>();
        }

        public Panel Project {
            get {
                return appService.Project;
            }
        }
    }
}
