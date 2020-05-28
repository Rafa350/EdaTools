namespace MikroPic.EdaTools.v1.PanelEditor.View {

    using System.Windows;
    using MikroPic.EdaTools.v1.PanelEditor.ViewModel;
    using MikroPic.NetMVVMToolkit.v1.WindowState;

    public partial class MainWindow: Window {

        public MainWindow() {

            InitializeComponent();
            WindowStateManager.RegisterWindow(this);

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {

            ViewModel.Load();
        }

        /// <summary>
        /// Obte el ViewModel
        /// </summary>
        /// 
        public MainViewModel ViewModel {
            get {
                return (MainViewModel)Resources["ViewModel"];
            }
        }
    }
}
