﻿namespace MikroPic.EdaTools.v1.BoardEditor.View {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.BoardEditor.DrawEditor;
    using MikroPic.EdaTools.v1.BoardEditor.Render.WPF;
    using MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals;
    using MikroPic.EdaTools.v1.Core.Model.Connectivity;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Point = System.Windows.Point;

    public partial class BoardEditorView : UserControl {

        public static readonly DependencyProperty BoardProperty;

        private readonly ViewPoint viewPoint;
        private readonly DispatcherTimer wheelTimer;
        private const double wheelInterval = 150;
        private int wheelCount = 0;
        private Point startPos;
        private Point currentPos;
        private SceneGraph sceneGraph;
        private ConnectivityMap map;

        static BoardEditorView() {

            // Crea la propietat de dependencia 'Board'
            //
            BoardProperty = DependencyProperty.Register(
                "Board",
                typeof(Board),
                typeof(BoardEditorView),
                new FrameworkPropertyMetadata(
                    null,
                    Board_PropertyChanged));
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// 
        public BoardEditorView() {

            InitializeComponent();

            wheelTimer = new DispatcherTimer();
            wheelTimer.Interval = TimeSpan.FromMilliseconds(wheelInterval);
            wheelTimer.Tick += OnWheelTimerTick;

            viewPoint = new ViewPoint();
            viewPoint.Changed += OnViewPointChanged;

            SizeChanged += OnSizeChanged;
        }

        private static void Board_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            (o as BoardEditorView).UpdateBoard();
        }

        private void OnWheelTimerTick(object sender, EventArgs e) {

            if (wheelCount > 0) 
                viewPoint.ZoomIn(0.25 * wheelCount, currentPos);

            else if (wheelCount < 0) 
                viewPoint.ZoomOut(0.25 * (-wheelCount), currentPos);

            wheelCount = 0;
            wheelTimer.Stop();
        }

        private void OnViewPointChanged(object sender, EventArgs e) {

            if (contentBox.Visual != null)
                UpdateViewPoint();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) {

            if (Board != null) {

                v1.Base.Geometry.Size boardSize = Board.Size;
                viewPoint.Reset(
                    new System.Windows.Size(contentBox.ActualWidth, contentBox.ActualHeight),
                    new System.Windows.Rect(0, 0, boardSize.Width, boardSize.Height));
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
            Point mousePos = e.GetPosition(contentBox);
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
            Point mousePos = e.GetPosition(contentBox);
            currentPos = viewPoint.TransformToWorld(mousePos);
            startPos = currentPos;

            // Si es el boto primari, es una accio amb l'eina seleccionada
            //
            if (e.ChangedButton == MouseButton.Left) {

                if (sceneGraph != null) {
                    VisualBase visual = sceneGraph.GetVisual((int)currentPos.X, (int)currentPos.Y);
                    if (visual != null) {
                        if (visual is ElementVisual elementVisual) {
                            Element element = elementVisual.Element;
                            if (element is ViaElement via) {
                                Base.Geometry.Point position = via.Position;
                                IEnumerable<Element> elements = map.GetChainedElements(position);
                                foreach (var eu in elements) {
                                    ElementVisual ev = sceneGraph.GetVisual(eu);
                                    if (ev != null)
                                        (ev.Parent as DrawingVisual).Children.Remove(ev);
                                }
                            }
                        }
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
                if (e.Delta > 0) {
                    wheelCount++;
                    wheelTimer.Stop();
                    wheelTimer.Start();
                    //viewPoint.ZoomIn(0.25, currentPos);
                }

                // En cas contrari allunya
                //
                else if (e.Delta < 0) {
                    wheelCount--;
                    wheelTimer.Stop();
                    wheelTimer.Start();
                    //viewPoint.ZoomOut(0.25, currentPos);
                }
            }
        }

        private void UpdateBoard() {

            if (contentBox.Visual != null)
                contentBox.Visual = null;

            if (Board != null) {

                viewPoint.Reset(
                    new System.Windows.Size(contentBox.ActualWidth, contentBox.ActualHeight),
                    new System.Windows.Rect(0, 0, Board.Size.Width, Board.Size.Height));

                RenderContext renderContext = new RenderContext();
                sceneGraph = renderContext.Render(Board) as SceneGraph;
                DrawingVisual visual = sceneGraph.Visual;

                visual.Transform = new MatrixTransform(viewPoint.Matrix);

                contentBox.Visual = visual;

                //map = new ConnectivityMap();
                //map.Add(Board);
            }
        }

        private void UpdateViewPoint() {

            if (contentBox.Visual != null)
                contentBox.Visual.Transform = new MatrixTransform(viewPoint.Matrix);
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