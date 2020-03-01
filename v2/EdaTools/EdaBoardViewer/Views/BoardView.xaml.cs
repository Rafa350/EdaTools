namespace EdaBoardViewer.Views {

    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using EdaBoardViewer.Views.Controls;

    public class BoardView : UserControl {

        private enum PointerButton {
            None,
            Left,
            Middle,
            Right
        }

        private const double valueDivisor = 1e6;

        private RulerControl horizontalRuler;
        private RulerControl verticalRuler;
        private DesignControl designer;
        private BoardControl boardView;
        private readonly ViewPoint viewPoint;

        private PointerButton pressedButton = PointerButton.None;
        private Point currentPos;
        private Point startPos;
        private Point endPos;

        private Size boardSize = new Size(70 * valueDivisor, 67.5 * valueDivisor);

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// 
        public BoardView() {

            this.InitializeComponent();

            viewPoint = new ViewPoint();
            viewPoint.Changed += ViewPoint_Changed;
        }

        /// <summary>
        /// Inicialitza el component.
        /// </summary>
        /// 
        private void InitializeComponent() {

            AvaloniaXamlLoader.Load(this);

            horizontalRuler = this.Get<RulerControl>("HorizontalRuler");
            horizontalRuler.ValueDivisor = valueDivisor;
            horizontalRuler.MinValue = 0;
            horizontalRuler.MaxValue = boardSize.Width;

            verticalRuler = this.Get<RulerControl>("VerticalRuler");
            verticalRuler.ValueDivisor = valueDivisor;
            verticalRuler.MinValue = 0;
            verticalRuler.MaxValue = boardSize.Height;

            designer = this.Get<DesignControl>("Designer");
            designer.ValueDivisor = valueDivisor;

            boardView = this.Get<BoardControl>("BoardView");
        }

        private void ViewPoint_Changed(object sender, EventArgs e) {

            Matrix m = viewPoint.Matrix;

            horizontalRuler.ValueMatrix = m;
            verticalRuler.ValueMatrix = m;
            designer.ValueMatrix = m;
            boardView.ValueMatrix = m;
        }

        /// <summary>
        /// Event generat quant canvia alguna propietat.
        /// </summary>
        /// <param name="e">Parametres del event.</param>
        /// 
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) {

            // Detecta el canvi en el tamany del control
            //
            if (e.Property == BoundsProperty)
                ResetViewPoint();

            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Procesa l'event de moviment del punter.
        /// </summary>
        /// <param name="e">Parametres de l'event.</param>
        /// 
        protected override void OnPointerMoved(PointerEventArgs e) {

            // Obte l'estat del punter.
            //
            PointerPoint pp = e.GetCurrentPoint(designer);

            // Obte la posicio del punter.
            //
            currentPos = viewPoint.TransformToWorld(pp.Position);
            endPos = currentPos;

            // Actualitza els controls de diseny
            //
            horizontalRuler.PointerPosition = currentPos;
            verticalRuler.PointerPosition = currentPos;
            designer.PointerPosition = currentPos;

            // Si es prem el boto dret, mostra la seleccio.
            //
            if (pressedButton != PointerButton.None) {
                
                Point start = new Point(Math.Min(startPos.X, endPos.X), Math.Min(startPos.Y, endPos.Y));
                Size size = new Size(Math.Abs(endPos.X - startPos.X), Math.Abs(endPos.Y - startPos.Y));

                horizontalRuler.RegionPosition = start;
                horizontalRuler.RegionSize = size;

                verticalRuler.RegionPosition = start;
                verticalRuler.RegionSize = size;

                designer.RegionPosition = start;
                designer.RegionSize = size;
            }

            base.OnPointerMoved(e);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e) {

            // Obte l'estat del punter.
            //
            PointerPoint pp = e.GetCurrentPoint(designer);

            // Obte la posicio del punter.
            //
            currentPos = viewPoint.TransformToWorld(pp.Position);
            startPos = currentPos;
            endPos = currentPos;

            // Obte el boto premut.
            //
            if (pp.Properties.IsLeftButtonPressed)
                pressedButton = PointerButton.Left;
            else if (pp.Properties.IsMiddleButtonPressed)
                pressedButton = PointerButton.Middle;
            else if (pp.Properties.IsRightButtonPressed)
                pressedButton = PointerButton.Right;
            else
                pressedButton = PointerButton.None;

            switch (pressedButton) {
                case PointerButton.Left:
                    horizontalRuler.PointerPosition = currentPos;
                    horizontalRuler.RegionPosition = currentPos;
                    horizontalRuler.RegionSize = new Size(0, 0);
                    horizontalRuler.ShowRegion = true;

                    verticalRuler.PointerPosition = currentPos;
                    verticalRuler.RegionPosition = currentPos;
                    verticalRuler.RegionSize = new Size(0, 0);
                    verticalRuler.ShowRegion = true;

                    designer.RegionPosition = startPos;
                    designer.RegionSize = new Size(0, 0);
                    designer.ShowPointer = false;
                    designer.ShowRegion = true;
                    designer.ShowRegionHandles = true;

                    break;


            }

            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e) {

            // Actualitza les coordinades
            //
            Point p = e.GetPosition(designer);
            Point currentPos = viewPoint.TransformToWorld(p);
            endPos = currentPos;

            switch (pressedButton) {
                case PointerButton.Middle:
                    viewPoint.Pan(endPos - startPos);
                    break;

                case PointerButton.Right:
                    viewPoint.Rotate(90, currentPos);
                    break;
            }

            pressedButton = PointerButton.None;

            // Actualitza els controls de diseny
            //
            horizontalRuler.PointerPosition = currentPos;
            horizontalRuler.ShowRegion = false;

            verticalRuler.PointerPosition = currentPos;
            verticalRuler.ShowRegion = false;

            designer.PointerPosition = currentPos;
            designer.ShowRegion = false;
            designer.ShowPointer = true;

            base.OnPointerReleased(e);
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e) {

            if (e.Delta.Y > 0)
                viewPoint.ZoomIn(0.25, currentPos);
            else if (e.Delta.Y < 0)
                viewPoint.ZoomOut(0.25, currentPos);

            base.OnPointerWheelChanged(e);
        }

        /// <summary>
        /// Inicialitza el punt de vista.
        /// </summary>
        /// 
        private void ResetViewPoint() {

            Matrix matrix =
                Matrix.CreateTranslation(0, -boardSize.Height) *
                Matrix.CreateScale(1, -1);

            viewPoint.Reset(
                boardView.Bounds.Size,
                new Rect(new Point(0, 0), boardSize),
                matrix);
        }
    }
}
