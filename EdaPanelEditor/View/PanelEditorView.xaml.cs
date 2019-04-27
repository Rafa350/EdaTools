namespace MikroPic.EdaTools.v1.PanelEditor.View {

    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MikroPic.EdaTools.v1.PanelEditor.ViewModel;
    using MikroPic.EdaTools.v1.PanelEditor.VisualEditor;
    using MikroPic.EdaTools.v1.PanelEditor.VisualEditor.Controls;
    using MikroPic.EdaTools.v1.PanelEditor.VisualEditor.Tools;

    public partial class PanelEditorView : UserControl {

        public static readonly DependencyProperty ProjectProperty;

        private readonly ViewPoint viewPoint;
        private const double wheelInterval = 150;
        private Point startPos;
        private Point currentPos;
        private ToolBase currentTool;
        private SelectionTool selectionTool;

        static PanelEditorView() {

            // Crea la propietat de dependencia 'Project'
            //
            ProjectProperty = DependencyProperty.Register(
                "Project",
                typeof(v1.Panel.Model.Panel),
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

            Loaded += PanelEditorView_Loaded;

            // Inicialitza el punt de vista
            //
            viewPoint = new ViewPoint();
            viewPoint.Changed += ViewPoint_Changed;

            // Inicialitza les eines
            //
            selectionTool = new SelectionTool(contentBox);
            selectionTool.Activate();
            selectionTool.MouseUp += SelectionTool_MouseUp;
            currentTool = selectionTool;

            SizeChanged += PanelEditorView_SizeChanged;
        }

        private void PanelEditorView_Loaded(object sender, RoutedEventArgs e) {

            ViewModel.Load();
        }

        private static void Project_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            (o as PanelEditorView).ViewModel.Project = (v1.Panel.Model.Panel)e.NewValue;

            (o as PanelEditorView).ProjectEditorView_ProjectChanged(o, new EventArgs());
        }

        /// <summary>
        /// Procesa l'event 'MouseUp' del objecte 'selectionTool'
        /// </summary>
        /// <param name="tool">L'objecte que ha regnerat l'event.</param>
        /// 
        private void SelectionTool_MouseUp(object sender) {

            Rect selectionRect = viewPoint.TransformToWorld(selectionTool.Selection);
            contentBox.GetVisuals(selectionRect);
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
                hRulerBox.MaxValue = hRulerBox.TransformMode == TransformationMode.XAxis ? Project.Size.Width : Project.Size.Height;

                vRulerBox.TransformMatrix = viewPoint.Matrix;
                vRulerBox.MaxValue = vRulerBox.TransformMode == TransformationMode.YAxis ? Project.Size.Height : Project.Size.Width;

                contentBox.TransformMatrix = viewPoint.Matrix;
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

                contentBox.Visibility = Visibility.Visible;
            }
            else {
                contentBox.Visibility = Visibility.Hidden;
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

            // Si es tracta del boto primari, es tracta d'una accio 
            // amb l'eina seleccionada
            //
            if (e.LeftButton == MouseButtonState.Pressed) {
                if (currentTool != null)
                    currentTool.NotifyMouseMove(mousePos);
            }

            // Si es el boto central es tracta d'una accio Pan
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
        /// Obte el ViewModel associat
        /// </summary>
        /// 
        public PanelEditorViewModel ViewModel {
            get {
                return (PanelEditorViewModel)Resources["ViewModel"];
            }
        }

        /// <summary>
        /// Obte o asigna la propietat 'Project'.
        /// </summary>
        /// 
        public MikroPic.EdaTools.v1.Panel.Model.Panel Project {
            get {
                return (v1.Panel.Model.Panel)GetValue(ProjectProperty);
            }
            set {
                SetValue(ProjectProperty, value);
            }
        }
    }
}
