﻿namespace EdaBoardViewer.Views {

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;

    public class MainWindow : Window {

        public MainWindow() {

            InitializeComponent();
#if DEBUG
         //   this.AttachDevTools();
#endif
        }

        private void InitializeComponent() {

            AvaloniaXamlLoader.Load(this);
        }
    }
}
