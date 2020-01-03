namespace MikroPic.EdaTools.v1.PanelEditor.View {

    using System.Windows;
    using System.Windows.Controls;
    using MikroPic.EdaTools.v1.PanelEditor.ViewModel;

    public partial class PanelTreeView: UserControl {

        public static readonly DependencyProperty ProjectProperty;

        /// <summary>
        /// Constructor estatic. Crea les propietats de dependencia.
        /// </summary>
        /// 
        static PanelTreeView() {

            // Crea la propietat de dependencia 'Project'
            //
            ProjectProperty = DependencyProperty.Register(
                "Project",
                typeof(v1.Panel.Model.Panel),
                typeof(PanelTreeView),
                new FrameworkPropertyMetadata(
                    null,
                    Project_PropertyChanged));
        }

        /// <summary>
        /// Contructor de l'objecte.
        /// </summary>
        /// 
        public PanelTreeView() {

            InitializeComponent();

            Loaded += PanelTreeView_Loaded;
        }

        private static void Project_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            (o as PanelTreeView).ViewModel.Project = (v1.Panel.Model.Panel)e.NewValue;
        }

        private void PanelTreeView_Loaded(object sender, RoutedEventArgs e) {

            ViewModel.Load();
        }

        /// <summary>
        /// Obte el ViewModel associat.
        /// </summary>
        /// 
        public PanelTreeViewModel ViewModel {
            get {
                return (PanelTreeViewModel)Resources["ViewModel"];
            }
        }

        /// <summary>
        /// Obte o asigna la propietat 'Project'.
        /// </summary>
        /// 
        public MikroPic.EdaTools.v1.Panel.Model.Panel Project {
            get {
                return (v1.Panel.Model.Panel)GetValue(ProjectProperty);
            }
            set {
                SetValue(ProjectProperty, value);
            }
        }
    }
}
