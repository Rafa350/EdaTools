namespace EdaBoardViewer.Views {

    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using EdaBoardViewer.Views.Controls;

    public class BoardView : UserControl {

        private RulerBox horizontalRuler;
        private RulerBox verticalRuler;
        private DesignBox designer;
        //private BoardViewControl boardView;

        private bool buttonPressed = false;
        private Point startPos;
        private Point endPos;

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

            // Obte la posicio del punter
            //
            Point p = e.GetPosition(designer);
            Point currentPos = new Point((p.X / 10) - 0, (p.Y / 10) - 0);

            // Actualitza els controls de diseny
            //
            horizontalRuler.PointerPosition = currentPos.X;
            verticalRuler.PointerPosition = currentPos.Y;
            
            if (buttonPressed) {
                
                endPos = currentPos;

                horizontalRuler.RegionPosition = Math.Min(startPos.X, endPos.X);
                horizontalRuler.RegionSize = Math.Abs(endPos.X - startPos.X);

                verticalRuler.RegionSize = Math.Abs(endPos.Y - startPos.Y);
                verticalRuler.RegionPosition = Math.Min(startPos.Y, endPos.Y);

                designer.RegionPosition = new Point(Math.Min(startPos.X, endPos.X), Math.Min(startPos.Y, endPos.Y));
                designer.RegionSize = new Size(Math.Abs(endPos.X - startPos.X), Math.Abs(endPos.Y - startPos.Y));
            }
            else
                designer.PointerPosition = currentPos;

            base.OnPointerMoved(e);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e) {

            // Obte les dades del punter
            //
            PointerPoint pointer = e.GetCurrentPoint(designer);

            // Comprova si s'ha premut el boto
            //
            if (pointer.Properties.IsLeftButtonPressed) {

                buttonPressed = true;

                // Actualitza les coordinades
                //
                Point currentPos = new Point((pointer.Position.X / 10) - 0, (pointer.Position.Y / 10) - 0);
                startPos = currentPos;
                endPos = currentPos;

                // Actualitza els controls de diseny
                //
                horizontalRuler.PointerPosition = currentPos.X;
                horizontalRuler.RegionPosition = currentPos.X;
                horizontalRuler.RegionSize = 0;
                horizontalRuler.ShowRegion = true;

                verticalRuler.PointerPosition = currentPos.Y;
                verticalRuler.RegionPosition = currentPos.Y;
                verticalRuler.RegionSize = 0;
                verticalRuler.ShowRegion = true;
                
                designer.RegionPosition = startPos;
                designer.RegionSize = new Size(0, 0);
                designer.ShowPointer = false;
                designer.ShowRegion = true;
            }

            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e) {

            if (buttonPressed) {

                buttonPressed = false;

                // Actualitza les coordinades
                //
                Point p = e.GetPosition(designer);
                Point currentPos = new Point((p.X / 10) - 0, (p.Y / 10) - 0);
                endPos = currentPos;

                // Actualitza els controls de diseny
                //
                horizontalRuler.PointerPosition = currentPos.X;
                horizontalRuler.ShowRegion = false;
                
                verticalRuler.PointerPosition = currentPos.Y;
                verticalRuler.ShowRegion = false;
                
                designer.PointerPosition = currentPos;
                designer.ShowRegion = false;
                designer.ShowPointer = true;
            }

            base.OnPointerReleased(e);
        }
    }
}
