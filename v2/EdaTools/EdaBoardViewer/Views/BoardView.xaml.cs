namespace EdaBoardViewer.Views {

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using EdaBoardViewer.Views.Controls;

    public class BoardView : UserControl {

        private RulerBox horizontalRuler;
        private RulerBox verticalRuler;
        private BoardViewControl boardView;

        public BoardView() {

            this.InitializeComponent();
        }

        private void InitializeComponent() {

            AvaloniaXamlLoader.Load(this);

            horizontalRuler = this.Get<RulerBox>("HorizontalRuler");
            verticalRuler = this.Get<RulerBox>("VerticalRuler");
            boardView = this.Get<BoardViewControl>("BoardView");
        }

        protected override void OnPointerMoved(PointerEventArgs e) {
            
            Point p = e.GetPosition(boardView);
            
            if (horizontalRuler != null)
                horizontalRuler.PointerValue = p.X;
            if (verticalRuler != null)
                verticalRuler.PointerValue = p.Y;
            
            base.OnPointerMoved(e);
        }
    }
}
