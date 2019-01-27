namespace MikroPic.EdaTools.v1.PanelEditor.View {

    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.PanelEditor.DrawEditor;
    using MikroPic.EdaTools.v1.PanelEditor.Render;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Win = System.Windows;

    public partial class PanelEditorView : UserControl {

        public static readonly DependencyProperty ProjectProperty;

        private readonly ViewPoint viewPoint;
        private const double wheelInterval = 150;
        private Win.Point startPos;
        private Win.Point currentPos;

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
                    new Win.Size(contentBox.ActualWidth, contentBox.ActualHeight),
                    new Win.Rect(0, 0, boardSize.Width, boardSize.Height));
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
            Win.Point mousePos = e.GetPosition(contentBox);
            currentPos = viewPoint.TransformToWorld(mousePos);

            // Si es tracta del boto primari, es tracte d'una accio 
            // amb l'eina seleccionada
            //
            if (e.LeftButton == MouseButtonState.Pressed) {
            }

            // Si es el boto central es tracte d'una accio Pan
            //
            if (e.MiddleButton == MouseButtonState.Pressed) 
                viewPoint.Pan(currentPos.X - startPos.X, currentPos.Y - startPos.Y);
        }

        /// <summary>
        /// Procesa l'event dels botons del mouse.
        /// </summary>
        /// <param name="e">Arguments de l'event.</param>
        /// 
        protected override void OnMouseUp(MouseButtonEventArgs e) {

            // Actualtza la posicio del mouse
            //
            Win.Point mousePos = e.GetPosition(contentBox);
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
            Win.Point mousePos = e.GetPosition(contentBox);
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

        /// <summary>
        /// Procesa els events de la roda del mouse.
        /// </summary>
        /// <param name="e">Arguments de l'event.</param>
        /// 
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

        /// <summary>
        /// Actualitza l'escena.
        /// </summary>
        /// 
        private void UpdateView() {

            if (contentBox.Visual != null)
                contentBox.Visual = null;

            if (Project != null) {

                viewPoint.Reset(
                    new Win.Size(contentBox.ActualWidth, contentBox.ActualHeight),
                    new Win.Rect(0, 0, Project.Size.Width, Project.Size.Height));

                DrawingVisual visual = new DrawingVisual();
                RenderVisitor visitor = new RenderVisitor(visual);
                Project.AcceptVisitor(visitor);

                visual.Transform = new MatrixTransform(viewPoint.Matrix);

                contentBox.Visual = visual;
            }
        }

        /// <summary>
        /// Actualitza el punt de vista de l'escena.
        /// </summary>
        /// 
        private void UpdateViewPoint() {

            if (contentBox.Visual != null)
                contentBox.Visual.Transform = new MatrixTransform(viewPoint.Matrix);
        }

        /// <summary>
        /// Obte o asigna el projecte.
        /// </summary>
        /// 
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
