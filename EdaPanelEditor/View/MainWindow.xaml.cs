namespace MikroPic.EdaTools.v1.PanelEditor.View {

    using System.Windows;
    using MikroPic.EdaTools.v1.PanelEditor.ViewModel;
    using MikroPic.NetMVVMToolkit.v1.WindowState;

    public partial class MainWindow: Window {

        private readonly WindowStateAgent sa;

        public MainWindow() {

            InitializeComponent();
            DataContext = new MainViewModel();

            sa = new WindowStateAgent(this);
        }
    }
}
