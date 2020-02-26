namespace EdaBoardViewer.Views {

    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Media;
    using Avalonia.Markup.Xaml;
    using EdaBoardViewer.Views.Controls;

    public class BoardView : UserControl {

        private RulerBox horizontalRuler;
        private RulerBox verticalRuler;
        private DesignBox designer;
        private BoardViewControl boardView;
        private readonly ViewPoint viewPoint;

        private bool buttonPressed = false;
        private Point startPos;
        private Point endPos;

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

            horizontalRuler = this.Get<RulerBox>("HorizontalRuler");
            horizontalRuler.MaxValue = 70;

            verticalRuler = this.Get<RulerBox>("VerticalRuler");
            verticalRuler.MaxValue = 67.5;
            
            designer = this.Get<DesignBox>("Designer");
            
            boardView = this.Get<BoardViewControl>("BoardView");
        }

        private void ViewPoint_Changed(object sender, EventArgs e) {

            // Ajusta la vista de la placa
            //
            Matrix m = Matrix.Identity;
            m *= viewPoint.Matrix;
            m *= Matrix.CreateScale(1, -1);
            m *= Matrix.CreateTranslation(0, boardView.Bounds.Height);

            boardView.RenderTransformOrigin = RelativePoint.TopLeft;
            boardView.RenderTransform = new MatrixTransform(m);
            boardView.InvalidateVisual();

            // Ajusta els controls de diseny
            //
            Point offset = viewPoint.Offset;
            Point scale = viewPoint.Scale;

            horizontalRuler.Origin = offset.X;
            horizontalRuler.Scale = scale.X * 1000000;
            verticalRuler.Origin = offset.Y;
            verticalRuler.Scale = scale.Y * 1000000;
        }

        /// <summary>
        /// Event generat quant canvia alguna propietat.
        /// </summary>
        /// <param name="e">Parametres del event.</param>
        /// 
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) {

            // Detecta el canvi en el tamany del control
            //
            if (e.Property == BoundsProperty) {

                // Inicialitza el punt de vista.
                //
                Size vSize = boardView.Bounds.Size;
                Rect wRect = new Rect(0, 0, 70000000, 67500000);
                viewPoint.Reset(vSize, wRect);
            }
            
            base.OnPropertyChanged(e);
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
