namespace MikroPic.EdaTools.v1.PanelEditor.View {

    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using MikroPic.EdaTools.v1.PanelEditor.DrawEditor;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using WinPoint = System.Windows.Point;
    using WinRect = System.Windows.Rect;
    using WinSize = System.Windows.Size;

    public partial class PanelEditorView : UserControl {

        public static readonly DependencyProperty ProjectProperty;

        private readonly ViewPoint viewPoint;
        private const double wheelInterval = 150;
        private WinPoint startPos;
        private WinPoint currentPos;

        static PanelEditorView() {

            // Crea la propietat de dependencia 'Project'
            //
            ProjectProperty = DependencyProperty.Register(
                "Project",
                typeof(Project),
                typeof(PanelEditorView),
                new FrameworkPropertyMetadata(
                    null,
                    Project_PropertyChanged));
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// 
        public PanelEditorView() {

            InitializeComponent();

            viewPoint = new ViewPoint();
            viewPoint.Changed += OnViewPointChanged;

            SizeChanged += OnSizeChanged;
        }

        private static void Project_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            (o as PanelEditorView).OnProjectChanged(o, new EventArgs());
        }

        private void OnViewPointChanged(object sender, EventArgs e) {

            if (contentBox.Visual != null)
                UpdateViewPoint();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) {

            if (Project != null) {

                v1.Base.Geometry.Size boardSize = Project.Size;
                viewPoint.Reset(
                    new WinSize(contentBox.ActualWidth, contentBox.ActualHeight),
                    new WinRect(0, 0, boardSize.Width, boardSize.Height));
            }
        }

        private void OnProjectChanged(object sender, EventArgs e) {

            UpdateView();
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
            WinPoint mousePos = e.GetPosition(contentBox);
            currentPos = viewPoint.TransformToWorld(mousePos);

            // Si es tracta del boto primari, es tracte d'una accio 
            // amb l'eina seleccionada
            //
            if (e.LeftButton == MouseButtonState.Pressed) {
            }

            // Si es el boto central es tracte d'una accio Pan
            //
            if (e.MiddleButton == MouseButtonState.Pressed) {
                //  viewPoint.Pan(currentPos.X - startPos.X, currentPos.Y - startPos.Y);
            }
        }

        /// <summary>
        /// Procesa l'event dels botons del mouse.
        /// </summary>
        /// <param name="e">Arguments de l'event.</param>
        /// 
        protected override void OnMouseUp(MouseButtonEventArgs e) {

            // Actualtza la posicio del mouse
            //
            WinPoint mousePos = e.GetPosition(contentBox);
            currentPos = viewPoint.TransformToWorld(mousePos);

            // Si es el boto primari, es una accio amb l'eina seleccionada
            //
            if (e.ChangedButton == MouseButton.Left) {
            }

            // Si es el boto central es tracte d'una accio Pan
            //
            else if (e.ChangedButton == MouseButton.Middle) {
                viewPoint.Pan(currentPos.X - startPos.X, currentPos.Y - startPos.Y);
                currentPos = viewPoint.TransformToWorld(mousePos);
            }
        }

        /// <summary>
        /// Procesa l'event dels botons del mouse.
        /// </summary>
        /// <param name="e">Arguments de l'event.</param>
        /// 
        protected override void OnMouseDown(MouseButtonEventArgs e) {

            // Actualtza la posicio del mouse
            //
            WinPoint mousePos = e.GetPosition(contentBox);
            currentPos = viewPoint.TransformToWorld(mousePos);
            startPos = currentPos;

            // Si es el boto primari, es una accio amb l'eina seleccionada
            //
            if (e.ChangedButton == MouseButton.Left) {

                HitTestResult result = VisualTreeHelper.HitTest(contentBox.Visual, currentPos);
                if (result != null) {
                    DrawingVisual visual = result.VisualHit as DrawingVisual;
                    if (visual != null) {
                        (visual.Parent as DrawingVisual).Children.Remove(visual);
                    }
                }
            }

            else if (e.ChangedButton == MouseButton.Right)
                viewPoint.Rotate(90, currentPos);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {

            // Si el boto central no esta premut, es una accio Zoom
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

        private void UpdateView() {

            if (contentBox.Visual != null)
                contentBox.Visual = null;

            if (Project != null) {

                viewPoint.Reset(
                    new WinSize(contentBox.ActualWidth, contentBox.ActualHeight),
                    new WinRect(0, 0, Project.Size.Width, Project.Size.Height));

                DrawingVisual visual = new DrawingVisual();
                foreach (ProjectItem item in Project.Items) {
                    if (item is CutItem cutItem) {
                        DrawingVisual itemVisual = CreateCutItemVisual(cutItem);
                        visual.Children.Add(itemVisual);
                    }
                    else if (item is PcbItem pcbItem) {
                        DrawingVisual itemVisual = CreatePcbItemVisual(pcbItem);
                        visual.Children.Add(itemVisual);
                    }
                }

                visual.Transform = new MatrixTransform(viewPoint.Matrix);

                contentBox.Visual = visual;
            }
        }

        private void UpdateViewPoint() {

            if (contentBox.Visual != null)
                contentBox.Visual.Transform = new MatrixTransform(viewPoint.Matrix);
        }

        private DrawingVisual CreateCutItemVisual(CutItem item) {

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen()) {

                WinPoint start = new WinPoint(item.StartPosition.X, item.StartPosition.Y);
                WinPoint end = new WinPoint(item.EndPosition.X, item.EndPosition.Y);

                Pen pen = new Pen(new SolidColorBrush(Colors.Cyan), item.Tickness);

                context.DrawLine(pen, start, end);
            }
            return visual;
        }

        private DrawingVisual CreatePcbItemVisual(PcbItem item) {

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen()) {

            }
            return visual;
        }

        public Project Project {
            get {
                return (Project) GetValue(ProjectProperty);
            }
            set {
                SetValue(ProjectProperty, value);
            }
        }
    }
}
