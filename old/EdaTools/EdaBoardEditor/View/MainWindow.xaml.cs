namespace MikroPic.EdaTools.v1.BoardEditor.View {

    using MikroPic.EdaTools.v1.BoardEditor.ViewModel;
    using MikroPic.NetMVVMToolkit.v1.WindowState;
    using System.Windows;

    public partial class MainWindow: Window {

        private const string path = @"..\..\..\Data";
        private const string inImportFileName = @"board3.brd";
        private const string fileName = @"board3.xml";

        public MainWindow() {

            InitializeComponent();
            WindowStateManager.RegisterWindow(this);

            DataContext = new MainViewModel();
        }
    }
}
