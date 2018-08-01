namespace MikroPic.EdaTools.v1.Designer.View {

    using MikroPic.EdaTools.v1.Designer.DrawEditor;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Geometry;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class BoardEditorView : UserControl {

        public static readonly DependencyProperty BoardProperty;

        private readonly ViewPoint viewPoint;
        private Point startPos;
        private Point currentPos;

        static BoardEditorView() {

            // Crea la propietat de dependencia 'Board'
            //
            BoardProperty = DependencyProperty.Register(
                "Board",
                typeof(Board),
                typeof(BoardEditorView),
                new FrameworkPropertyMetadata(
                    new Board(),
                    Board_PropertyChanged));
        }

        public BoardEditorView() {

            InitializeComponent();

            viewPoint = new ViewPoint();
            viewPoint.Changed += ViewPoint_Changed;

            SizeChanged += OnSizeChanged;
        }

        private static void Board_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            (o as BoardEditorView).UpdateBoard();
        }

        private void ViewPoint_Changed(object sender, System.EventArgs e) {

            if (contentBox.Visual != null)
                UpdateViewPoint();
        }

        private void UpdateBoard() {

            if (contentBox.Visual != null)
                contentBox.Visual = null;

            if (Board != null) {

                SizeInt boardSize = Board.Size;
                viewPoint.Reset(
                    new Size(contentBox.ActualWidth, contentBox.ActualHeight),
                    new Rect(0, 0, boardSize.Width / 1000000.0, boardSize.Height / 1000000.0));

                VisualGenerator vg = new VisualGenerator(Board);
                DrawingVisual visual = vg.CreateVisual();
                visual.Transform = new MatrixTransform(viewPoint.Matrix);
                contentBox.Visual = visual;
            }
        }

        private void UpdateViewPoint() {

            if (contentBox.Visual != null)
                contentBox.Visual.Transform = new MatrixTransform(viewPoint.Matrix);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) {

            if (Board != null) {

                SizeInt boardSize = Board.Size;
                viewPoint.Reset(
                    new Size(contentBox.ActualWidth, contentBox.ActualHeight),
                    new Rect(0, 0, boardSize.Width / 1000000.0, boardSize.Height / 1000000.0));
            }
        }

        /// <summary>
        /// Procesa l'event de moviment del mouse
        /// </summary>
        /// <param name="sender">El control que genera l'event.</param>
        /// <param name="e">Arguments de l'event.</param>
        /// 
        protected override void OnMouseMove(MouseEventArgs e) {

            // Actualtza la posicio del mouse
            //
            Point mousePos = e.GetPosition(contentBox);
            currentPos = viewPoint.TransformToWorld(mousePos);

            // Si es el boto central es tracte d'una accio Pan
            //
            if (e.MiddleButton == MouseButtonState.Pressed)
                viewPoint.Pan(currentPos.X - startPos.X, currentPos.Y - startPos.Y);

            // Si es tracta del boto primari, es tracte d'una accio 
            // amb l'eina seleccionada
            //
            else if (e.LeftButton == MouseButtonState.Pressed) {
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {

            // Actualtza la posicio del mouse
            //
            Point mousePos = e.GetPosition(contentBox);
            currentPos = viewPoint.TransformToWorld(mousePos);

            // Si es el boto primari, es una accio amb l'eina seleccionada
            //
            if (e.LeftButton == MouseButtonState.Released) {
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {

            // Actualtza la posicio del mouse
            //
            Point mousePos = e.GetPosition(contentBox);
            currentPos = viewPoint.TransformToWorld(mousePos);
            startPos = currentPos;

            // Si es el boto primari, es una accio amb l'eina seleccionada
            //
            if (e.LeftButton == MouseButtonState.Pressed) {
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {

            // Si el boto central esta premut, es una accio Zoom
            //
            if (e.MiddleButton == MouseButtonState.Released) {

                // Si el moviment es positiu, acosta
                //
                if (e.Delta > 0)
                    viewPoint.ZoomIn(0.25, currentPos);

                // En cas contrari allunya
                //
                else if (e.Delta < 0)
                    viewPoint.ZoomOut(0.25, currentPos);
            }
        }

        public Board Board {
            get {
                return (Board) GetValue(BoardProperty);
            }
            set {
                SetValue(BoardProperty, value);
            }
        }
    }
}
