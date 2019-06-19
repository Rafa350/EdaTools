namespace MikroPic.EdaTools.v1.PanelEditor.ViewModel {

    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.PanelEditor.Render;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;

    public sealed class PanelEditorViewModel: ViewModelBase {

        private Panel project;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// 
        public PanelEditorViewModel():
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
                    OnMultiplePropertyChanged("Visual", "Visibility");
            }
        }

        /// <summary>
        /// Obte o asigna la propietat Visual
        /// </summary>
        /// 
        public DrawingVisual Visual {
            get {
                if (project == null)
                    return null;
                else {
                    Scene scene = new Scene();
                    scene.Initialize(project);
                    return scene.Visual;
                }
            }
        }

        /// <summary>
        /// Obte o asigna la propietat ContentVisubility
        /// </summary>
        /// 
        public Visibility Visibility {
            get {
                return project == null ? Visibility.Hidden : Visibility.Visible;
            }
        } 
    }
}
