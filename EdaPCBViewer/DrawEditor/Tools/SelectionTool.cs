namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Tools {

    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Input;
    
    public sealed class SelectionTool: DesignTool {

        private Brush brush;
        private Pen pen;

        public SelectionTool(DesignSurface surface)
            : base(surface) {

            brush = Brushes.Transparent;

            pen = new Pen {
                Brush = Brushes.White,
                Thickness = 1,
                DashStyle = DashStyles.Dash
            };             
        }

        protected override Cursor GetCursor(Point position) {

            return Cursors.Arrow;
        }

        protected override void RenderBox(DrawingContext dc, Point startPosition, Point endPosition) {

            dc.DrawRectangle(brush, pen, new Rect(startPosition, endPosition));
        }
    }
}
