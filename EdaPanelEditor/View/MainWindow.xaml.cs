﻿namespace MikroPic.EdaTools.v1.PanelEditor.View {

    using System.Windows;
    using MikroPic.EdaTools.v1.PanelEditor.ViewModel;
    using MikroPic.NetMVVMToolkit.v1.WindowState;

    public partial class MainWindow: Window {

        private readonly WindowStateAgent sa;

        public MainWindow() {

            InitializeComponent();

            Loaded += MainWindow_Loaded;

            sa = new WindowStateAgent(this);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {

            MainViewModel vm = (MainViewModel) Resources["ViewModel"];
            vm.Load();
            vm.Initialize();
            DataContext = vm;
        }
    }
}
