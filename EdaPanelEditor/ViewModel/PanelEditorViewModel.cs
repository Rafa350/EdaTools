namespace MikroPic.EdaTools.v1.PanelEditor.ViewModel {

    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.PanelEditor.Render;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;

    public sealed class PanelEditorViewModel: ViewModelBase {

        private Panel project;

        public PanelEditorViewModel():
            base(null) {
        }

        public Panel Project {
            get {
                return project;
            }
            set {
                if (SetProperty<Panel>(ref project, value, "Project"))
                    OnMultiplePropertyChanged("Visual", "ContentVisibility");
            }
        }

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

        public Visibility ContentVisibility {
            get {
                return project == null ? Visibility.Hidden : Visibility.Visible;
            }
        } 
    }
}
