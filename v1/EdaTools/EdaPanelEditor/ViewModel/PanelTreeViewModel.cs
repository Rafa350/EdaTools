namespace MikroPic.EdaTools.v1.PanelEditor.ViewModel {

    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;

    public sealed class PanelTreeViewModel: ViewModelBase {

        private Panel project;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// 
        public PanelTreeViewModel():
            base(null) {
        }

        /// <summary>
        /// Obte o asigna la propietat Project.
        /// </summary>
        /// 
        public Panel Project {
            get {
                return project;
            }
            set {
                if (SetProperty<Panel>(ref project, value, "Project"))
                    OnPropertyChanged("Panel");
            }
        }

        public IEnumerable<Panel> Panel {
            get {
                return new Panel[] { project };
            }
        }
    }
}
