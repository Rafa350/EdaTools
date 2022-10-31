using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using EdaBoardViewer.Render;
using EdaBoardViewer.Tools;
using EdaBoardViewer.Views.Controls;
using MikroPic.EdaTools.v1.Core.IO;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace EdaBoardViewer.Views {

    public class SceneView: UserControl {

        private enum PointerButton {
            None,
            Left,
            Middle,
            Right
        }

        private class BoardScene: ISceneRenderer {

            private readonly EdaBoard _board;
            private readonly BoardRenderer _boardRenderer;

            public BoardScene() {

                //string fileName = @"C:\Users\Rafael\Documents\Projectes\EDA\DSP04X\dsp04x_panel.xbrd";
                string fileName = @"C:\Users\Rafael\Documents\Projectes\EDA\DSP04X\build\dsp04x.xbrd";
                using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                    var reader = new EdaBoardStreamReader(stream);
                    _board = reader.ReadBoard();
                }

                _boardRenderer = new BoardRenderer(_board);
            }

            public void Render(DrawingContext context) {

                _boardRenderer.Render(context);
            }
        }

        private const double valueDivisor = 1e6;

        private RulerControl horizontalRuler;
        private RulerControl verticalRuler;
        private DesignControl designer;
        private SceneControl sceneView;
        private readonly ViewPoint viewPoint;
        private readonly BoardScene boardScene;

        private readonly DesignTool currentTool = null;
        private readonly DesignTool selectorTool = new DesignTool();

        private PointerButton pressedButton = PointerButton.None;
        private Point currentPos;
        private Point pressedPos;

        private readonly Size boardSize = new Size(70 * valueDivisor, 67.5 * valueDivisor);

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// 
        public SceneView() {

            this.InitializeComponent();

            viewPoint = new ViewPoint();
            viewPoint.Changed += (sender, e) => OnViewPointChanged(e);

            boardScene = new BoardScene();

            selectorTool.PointerReleased += (s, e) => SelectorToolReleased(e);
            currentTool = selectorTool;
            currentTool.Activate();

            //this.PointerPressed += SceneView_PointerPressed;
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
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e) {

            // Detecta el canvi en el tamany del control
            //
            if (e.Property == BoundsProperty)
                ResetViewPoint();

            base.OnPropertyChanged(e);
        }

        private void SelectorToolReleased(DesignTool.PointerEventArgs e) {

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

            if (currentTool != null)
                currentTool.Move(currentPos);

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

            if (currentTool != null)
                currentTool.Press(currentPos);

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

            if (currentTool != null)
                currentTool.Release(currentPos);

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

            /*Matrix matrix =
                Matrix.CreateTranslation(0, 0) *
                Matrix.CreateScale(1, 1);
            ^*/
            viewPoint.Reset(
                sceneView.Bounds.Size,
                new Rect(new Point(0, 0), boardSize),
                matrix);
        }
    }
}
