namespace MikroPic.EdaTools.v1.PanelEditor.View {

    using MikroPic.EdaTools.v1.PanelEditor.ViewModel;
    using MikroPic.NetMVVMToolkit.v1.WindowState;
    using System.Windows;

    public partial class MainWindow: Window {

        private readonly WindowStateAgent sa;

        public MainWindow() {

            InitializeComponent();
            DataContext = new MainViewModel();

            sa = new WindowStateAgent(this);
        }
    }
}
