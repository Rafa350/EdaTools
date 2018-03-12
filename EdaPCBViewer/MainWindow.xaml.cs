namespace MikroPic.EdaTools.v1.Designer {

    using MikroPic.EdaTools.v1.Designer.DrawEditor;
    using MikroPic.EdaTools.v1.Designer.DrawEditor.Tools;
    using MikroPic.EdaTools.v1.Pcb.Import;
    using MikroPic.EdaTools.v1.Pcb.Import.Eagle;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;
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
        private const string path = @"..\..\..\Data";
        private const string inImportFileName = @"board3.brd";
        private const string fileName = @"board3.xml";

        private Point prevContentMousePos;

        public MainWindow() {

            InitializeComponent();

            selectionTool = new SelectionTool(content);
            padTool = new PadTool(content);
            lineTool = new LineTool(content);
            rectangleTool = new RectangleTool(content);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

            Board board;

            // Importa el fitxer
            //
            Importer importer = new EagleImporter();
            board = importer.Read(Path.Combine(path, inImportFileName));
            
            // Guarda el fitxer
            //
            using (Stream outStream = new FileStream(Path.Combine(path, fileName), FileMode.Create, FileAccess.Write, FileShare.None)) {
                XmlBoardWriter writer = new XmlBoardWriter(outStream);
                writer.Write(board);
            }
            
            // Carrega el fitxer
            //
            using (Stream inStream = new FileStream(Path.Combine(path, fileName), FileMode.Open, FileAccess.Read, FileShare.None)) {
                XmlBoardReader reader = new XmlBoardReader(inStream);
                board = reader.Read();
            }
            
            content.ClearVisual();
            VisualGenerator vg = new VisualGenerator(board);
            content.AddVisual(vg.CreateVisuals());
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
        /// 
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
                    DrawingVisual visual = result.VisualHit as DrawingVisual;
                    if (visual != null) {
                        /*visual.IsSelected = !visual.IsSelected;
                        foreach (ElementVisual child in content.Visuals) {
                            if (child.Part == visual.Part)
                                child.IsSelected = visual.IsSelected;
                        }*/
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
 
                zoomControl.ScaleCenterX += deltaX;
                zoomControl.ScaleCenterY += deltaY;
                zoomControl.Scale = newScale;

                e.Handled = true;
            }
        }
    }
}
