namespace MikroPic.EdaTools.v1.Designer.View {

    using MikroPic.EdaTools.v1.Designer.ViewModel;
    using System.Windows;
    using MikroPic.NetMVVMToolkit.v1.WindowState;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class MainWindow: Window {

        private bool showDrag = true;
        private readonly double scaleFactor = 1.1;
        private const string path = @"..\..\..\Data";
        private const string inImportFileName = @"board3.brd";
        private const string fileName = @"board3.xml";
        private readonly WindowStateAgent sa;

        private Point prevContentMousePos;

        public MainWindow() {

            InitializeComponent();
            DataContext = new MainViewModel();

            sa = new WindowStateAgent(this);
        }

        private void UpdatePositionIndicator(Point mousePos) {

            /*ctrlPosition.Content = String.Format("X:{0} Y:{1}",
                Math.Round(mousePos.X, 2),
                Math.Round(mousePos.Y, 2));*/
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
