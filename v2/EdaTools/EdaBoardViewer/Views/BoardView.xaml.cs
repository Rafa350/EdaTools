﻿namespace EdaBoardViewer.Views {

    using System;
    using System.IO;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using EdaBoardViewer.Views.Controls;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.IO;
    using EdaBoardViewer.Render;

    public class BoardView : UserControl {

        private enum PointerButton {
            None,
            Left,
            Middle,
            Right
        }

        private class BoardScene: ISceneRenderer {

            private Board board;

            public BoardScene() {

                using (Stream stream = new FileStream("board3.xbrd", FileMode.Open, FileAccess.Read, FileShare.None)) {
                    BoardStreamReader reader = new BoardStreamReader(stream);
                    board = reader.Read();
                }
            }

            public void Render(DrawingContext context) {

                var boardRenderer = new BoardRenderer(context);
                boardRenderer.Render(board);
            }
        }

        private const double valueDivisor = 1e6;

        private RulerControl horizontalRuler;
        private RulerControl verticalRuler;
        private DesignControl designer;
        private SceneControl sceneView;
        private readonly ViewPoint viewPoint;
        private readonly BoardScene boardScene;

        private PointerButton pressedButton = PointerButton.None;
        private Point currentPos;
        private Point pressedPos;

        private Size boardSize = new Size(70 * valueDivisor, 67.5 * valueDivisor);

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// 
        public BoardView() {

            this.InitializeComponent();

            viewPoint = new ViewPoint();
            viewPoint.Changed += (sender, e) => OnViewPointChanged(e);

            boardScene = new BoardScene();
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

            sceneView = this.Get<SceneControl>("BoardView");
        }

        /// <summary>
        /// Es crida quant hi ha un canvi en el punt de vista de l'escena.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        protected virtual void OnViewPointChanged(EventArgs e) {

            Matrix m = viewPoint.Matrix;

            horizontalRuler.ValueMatrix = m;
            verticalRuler.ValueMatrix = m;
            designer.ValueMatrix = m;

            sceneView.ValueMatrix = m;
            sceneView.SceneRenderer = boardScene;
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

            // Actualitza les coordinades.
            //
            Point p = e.GetPosition(designer);
            currentPos = viewPoint.TransformToWorld(p);

            // Actualitza els controls de diseny
            //
            horizontalRuler.PointerPosition = currentPos;
            verticalRuler.PointerPosition = currentPos;
            designer.PointerPosition = currentPos;

            // Si es prem el boto dret, mostra la seleccio.
            //
            if (pressedButton != PointerButton.None) {
                
                Point start = new Point(Math.Min(pressedPos.X, currentPos.X), Math.Min(pressedPos.Y, currentPos.Y));
                Size size = new Size(Math.Abs(currentPos.X - pressedPos.X), Math.Abs(currentPos.Y - pressedPos.Y));

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
            pressedPos = currentPos;

            // Obte el boto premut.
            //
            if (pp.Properties.IsLeftButtonPressed)
                pressedButton = PointerButton.Left;
            else if (pp.Properties.IsMiddleButtonPressed)
                pressedButton = PointerButton.Middle;
            else 
                pressedButton = PointerButton.Right;

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

                    designer.RegionPosition = pressedPos;
                    designer.RegionSize = new Size(0, 0);
                    designer.ShowPointer = false;
                    designer.ShowRegion = true;
                    designer.ShowRegionHandles = false;

                    break;

            }

            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e) {

            // Actualitza les coordinades
            //
            Point p = e.GetPosition(designer);
            Point currentPos = viewPoint.TransformToWorld(p);

            switch (pressedButton) {
                case PointerButton.Middle:
                    viewPoint.Pan(currentPos - pressedPos);
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
                sceneView.Bounds.Size,
                new Rect(new Point(0, 0), boardSize),
                matrix);
        }
    }
}