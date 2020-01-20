namespace EdaBoardViewer.Views {

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using EdaBoardViewer.Views.Controls;

    public class BoardView : UserControl {

        private RulerBox horizontalRuler;
        private RulerBox verticalRuler;
        private DesignBox designer;
        private BoardViewControl boardView;

        public BoardView() {

            this.InitializeComponent();
        }

        private void InitializeComponent() {

            AvaloniaXamlLoader.Load(this);

            horizontalRuler = this.Get<RulerBox>("HorizontalRuler");
            verticalRuler = this.Get<RulerBox>("VerticalRuler");
            designer = this.Get<DesignBox>("Designer");
            //boardView = this.Get<BoardViewControl>("BoardView");
        }

        protected override void OnPointerMoved(PointerEventArgs e) {

            Point p = e.GetPosition(designer);

            var x = (p.X / 10) - 0;
            var y = (p.Y / 10) - 0;

            if (horizontalRuler != null)
                horizontalRuler.PointerPosition = x;
            if (verticalRuler != null)
                verticalRuler.PointerPosition = y;
            if (designer != null)
                designer.PointerPosition = new Point(x, y);

            base.OnPointerMoved(e);
        }
    }
}
