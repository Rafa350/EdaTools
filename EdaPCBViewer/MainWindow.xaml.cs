namespace Eda.PCBViewer {

    using Eda.PCBViewer.DrawEditor;
    using Eda.PCBViewer.DrawEditor.Tools;
    using Eda.PCBViewer.DrawEditor.Visuals;
    using MikroPic.EdaTools.v1.Model;
    using MikroPic.EdaTools.v1.Model.IO;
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class MainWindow: Window {

        private DesignTool tool;
        private SelectionTool selectionTool;
        private LineTool lineTool;
        private RectangleTool rectangleTool;
        private PadTool padTool;

        private bool showDrag = true;
        private readonly double scaleFactor = 1.1;
        private const string inTestFileName = @"c:\temp\board.brd";
        private const string outTestFileName = @"c:\temp\board.xml";

        private Point prevContentMousePos;

        public MainWindow() {

            InitializeComponent();

            selectionTool = new SelectionTool(content);
            padTool = new PadTool(content);
            lineTool = new LineTool(content);
            rectangleTool = new RectangleTool(content);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

            BoardLoader loader = new BoardLoader();
            Board board = loader.Load(inTestFileName);

            XmlBoardWriter writer = new XmlBoardWriter(
                new FileStream(outTestFileName, FileMode.Create, FileAccess.Write, FileShare.None));
            writer.Write(board);

            content.ClearVisual();
            VisualGenerator vg = new VisualGenerator(board);
            foreach (Visual visual in vg.CreateVisuals())
                content.AddVisual(visual);
            //content.AddVisual(new GridVisual(0.1));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {

            if (tool != null)
                tool.Deactivate();

            tool = padTool;
            tool.Activate();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {

            if (tool != null)
                tool.Deactivate();

            tool = lineTool;
            tool.Activate();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) {

            if (tool != null)
                tool.Deactivate();

            tool = rectangleTool;
            tool.Activate();

        }

        private void Button_Click_4(object sender, RoutedEventArgs e) {

            if (tool != null)
                tool.Deactivate();

            tool = selectionTool;
            tool.Activate();
        }

        private void UpdatePositionIndicator(Point mousePos) {

            ctrlPosition.Content = String.Format("X:{0} Y:{1}",
                Math.Round(mousePos.X, 2),
                Math.Round(mousePos.Y, 2));
        }

        /// <summary>
        /// Procesa l'event de moviment del mouse
        /// </summary>
        /// <param name="sender">El control que genera l'event.</param>
        /// <param name="e">Arguments de l'event.</param>
        private void zoomControl_MouseMove(object sender, MouseEventArgs e) {

            Point curContentMousePos = e.GetPosition(content);

            UpdatePositionIndicator(curContentMousePos);

            if (e.MiddleButton == MouseButtonState.Pressed) {
                if (showDrag) {
                    Vector delta = curContentMousePos - prevContentMousePos;
                    zoomControl.OffsetX += delta.X;
                    zoomControl.OffsetY += delta.Y;
                }

                e.Handled = true;
            }

            prevContentMousePos = e.GetPosition(content);
        }

        private void zoomControl_MouseUp(object sender, MouseButtonEventArgs e) {

            if (e.ChangedButton == MouseButton.Middle) {
                if (!showDrag) {
                    Point curContentMousePos = e.GetPosition(content);
                    Vector delta = curContentMousePos - prevContentMousePos;
                    if ((delta.X != 0) || (delta.Y != 0)) {
                        zoomControl.OffsetX += delta.X;
                        zoomControl.OffsetY += delta.Y;
                    }
                }
                zoomControl.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void zoomControl_MouseDown(object sender, MouseButtonEventArgs e) {

            prevContentMousePos = e.GetPosition(content);

            if (e.ChangedButton == MouseButton.Middle) {
                zoomControl.CaptureMouse();
                e.Handled = true;
            }

            else if (e.ChangedButton == MouseButton.Left) {

                HitTestResult result = VisualTreeHelper.HitTest(content, e.GetPosition(content));
                if (result.VisualHit != null) {
                    ElementVisual visual = result.VisualHit as ElementVisual;
                    if (visual != null) {
                        visual.IsSelected = !visual.IsSelected;
                        foreach (ElementVisual child in content.Visuals) {
                            if (child.Part == visual.Part)
                                child.IsSelected = visual.IsSelected;
                        }
                    }
                }
            }
        }

        private void zoomControl_MouseWheel(object sender, MouseWheelEventArgs e) {

            if (e.Delta != 0) {

                double curScale = zoomControl.Scale;
                double newScale = e.Delta > 0 ? curScale * scaleFactor : curScale / scaleFactor;

                Point curMousePos = e.GetPosition(zoomControl);
                double deltaX = (curMousePos.X - zoomControl.ScaleCenterX) / curScale;
                double deltaY = (curMousePos.Y - zoomControl.ScaleCenterY) / curScale;
 
                zoomControl.Scale = newScale;
                zoomControl.ScaleCenterX += deltaX;
                zoomControl.ScaleCenterY += deltaY;

                e.Handled = true;
            }
        }
    }
}
