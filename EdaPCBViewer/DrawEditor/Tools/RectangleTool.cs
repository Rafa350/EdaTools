namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Tools {

    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Input;

    public class RectangleTool: DesignTool {

        private readonly Brush brush;
        private readonly Pen pen;

        public RectangleTool(DesignSurface surface)
            : base(surface) {

            brush = new SolidColorBrush {
                Color = Colors.Azure,
                Opacity = 0.5
            };

            pen = new Pen {
                Brush = Brushes.White,
                Thickness = 1
            };            
        }

        protected override Cursor GetCursor(Point position) {

            return Cursors.Cross;
        }

        protected override void RenderBox(DrawingContext dc, Point startPosition, Point endPosition) {

            dc.DrawRectangle(brush, pen, new Rect(startPosition, endPosition));
        }
        
        public Rect Rectangle {
            get {
                return new Rect(StartPosition, EndPosition);
            }
        }
    }
}
