using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EdaBoardViewer.Views {

    public class BoardView : UserControl {

        public BoardView() {

            this.InitializeComponent();
        }

        private void InitializeComponent() {

            AvaloniaXamlLoader.Load(this);
        }
    }
}
