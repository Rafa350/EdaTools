namespace MikroPic.EdaTools.v1.PanelEditor.ViewModel {

    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.PanelEditor.Services;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Services;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;

    public sealed class PanelStructureViewModel: ViewModelBase {

        private readonly IAppService appService;

        public PanelStructureViewModel(ViewModelBase parent):
            base(parent) {

            ServiceLocator serviceLocator = ServiceLocator.Instance;
            appService = serviceLocator.GetService<IAppService>();
        }

        public IEnumerable<Panel> Panel {
            get {
                Panel panel = appService.Project;
                return new Panel[] { panel };
            }
        }
    }
}
