namespace MikroPic.EdaTools.v1.PanelEditor.View {

    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.PanelEditor.DrawEditor;
    using MikroPic.EdaTools.v1.PanelEditor.DrawEditor.Tools;
    using MikroPic.EdaTools.v1.PanelEditor.DrawEditor.Controls;
    using MikroPic.EdaTools.v1.PanelEditor.Render;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class PanelEditorView : UserControl {

        public static readonly DependencyProperty ProjectProperty;

        private readonly ViewPoint viewPoint;
        private readonly Scene scene;
        private const double wheelInterval = 150;
        private Point startPos;
        private Point currentPos;
        private DesignTool currentTool;
        private DesignTool selectTool;

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

            // Inicialitza el punt de vista
            //
            viewPoint = new ViewPoint();
            viewPoint.Changed += ViewPoint_Changed;

            // Inicialitza la regla horitzontal
            //
            hRulerBox.TransformMode = TransformationMode.XAxis;
            hRulerBox.MaxValue = 100000000;
            hRulerBox.Visibility = Visibility.Hidden;

            // Inicialitza la regla vertical
            //
            vRulerBox.TransformMode = TransformationMode.YAxis;
            vRulerBox.MaxValue = 100000000;
            vRulerBox.Visibility = Visibility.Hidden;

            // Inicialitza les eines
            //
            selectTool = new VisualDesignTool(contentBox);
            selectTool.Deactive += SelectTool_Deactive;
            selectTool.Activate();
            currentTool = selectTool;

            scene = new Scene();

            SizeChanged += PanelEditorView_SizeChanged;
        }

        private void SelectTool_Deactive(object sender) {
            throw new NotImplementedException();
        }

        private static void Project_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            (o as PanelEditorView).ProjectEditorView_ProjectChanged(o, new EventArgs());
        }

        /// <summary>
        /// Procesa l'event 'OnChanged' de l'objecte 'viewPoint'
        /// </summary>
        /// <param name="sender">L'objecte que genera el missatge.</param>
        /// <param name="e">Parametres del missatge.</param>
        /// 
        private void ViewPoint_Changed(object sender, EventArgs e) {

            if (Project != null) {

                hRulerBox.TransformMatrix = viewPoint.Matrix;
                hRulerBox.MinValue = 0;
                hRulerBox.MaxValue = hRulerBox.TransformMode == TransformationMode.XAxis ? Project.Size.Width : Project.Size.Height;
                hRulerBox.Visibility = Visibility.Visible;

                vRulerBox.TransformMatrix = viewPoint.Matrix;
                vRulerBox.MinValue = 0;
                vRulerBox.MaxValue = vRulerBox.TransformMode == TransformationMode.YAxis ? Project.Size.Height : Project.Size.Width;
                vRulerBox.Visibility = Visibility.Visible;

                //guideBox.ViewTransform = t;
                //contentBox.ViewTransform = t;
                //pointerTool.Matrix = t.Value;

                if (contentBox.Visual != null)
                    contentBox.Visual.Transform = new MatrixTransform(viewPoint.Matrix);
            }
            else {
                hRulerBox.Visibility = Visibility.Hidden;
                vRulerBox.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Procesa el event 'SizeChanged' els canvis de tamany del control.
        /// </summary>
        /// <param name="sender">Objecte que genera el missatge.</param>
        /// <param name="e">Parametres del missatge.</param>
        /// 
        private void PanelEditorView_SizeChanged(object sender, SizeChangedEventArgs e) {

            if (Project != null) {
                viewPoint.Reset(
                    new Size(contentBox.ActualWidth, contentBox.ActualHeight),
                    new Rect(0, 0, Project.Size.Width, Project.Size.Height));
            }
        }

        private void ProjectEditorView_ProjectChanged(object sender, EventArgs e) {

            if (Project != null) {
                viewPoint.Reset(
                    new Size(contentBox.ActualWidth, contentBox.ActualHeight),
                    new Rect(0, 0, Project.Size.Width, Project.Size.Height));
                UpdateView();
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

            // Si es tracta del boto primari, es tracte d'una accio 
            // amb l'eina seleccionada
            //
            if (e.LeftButton == MouseButtonState.Pressed) {
                if (currentTool != null)
                    currentTool.NotifyMouseMove(mousePos);
            }

            // Si es el boto central es tracte d'una accio Pan
            //
            if (e.MiddleButton == MouseButtonState.Pressed) 
                viewPoint.Pan(currentPos.X - startPos.X, currentPos.Y - startPos.Y);

            // Actualitza la posicio del cursor
            //
            hRulerBox.PointerValue = currentPos;
            vRulerBox.PointerValue = currentPos;
        }

        /// <summary>
        /// Procesa l'event dels botons del mouse.
        /// </summary>
        /// <param name="e">Arguments de l'event.</param>
        /// 
        protected override void OnMouseUp(MouseButtonEventArgs e) {

            // Actualtza la posicio del mouse
            //
            Point mousePos = e.GetPosition(contentBox);
            currentPos = viewPoint.TransformToWorld(mousePos);

            // Si es el boto primari, es una accio amb l'eina seleccionada
            //
            if (e.ChangedButton == MouseButton.Left) {
                if (currentTool != null)
                    currentTool.NotifyMouseUp(mousePos);
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
            Point mousePos = e.GetPosition(contentBox);
            currentPos = viewPoint.TransformToWorld(mousePos);
            startPos = currentPos;

            // Si es el boto primari, es una accio amb l'eina seleccionada
            //
            if (e.ChangedButton == MouseButton.Left) {
                if (currentTool != null) 
                    currentTool.NotifyMouseDown(mousePos);
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

            if (Project == null)
                contentBox.Visual = null;

            else {
                scene.Initialize(Project);
                DrawingVisual visual = scene.Visual;
                visual.Transform = new MatrixTransform(viewPoint.Matrix);
                contentBox.Visual = visual;
            }
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
