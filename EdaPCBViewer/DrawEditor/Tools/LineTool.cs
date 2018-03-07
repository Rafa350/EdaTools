namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Tools {

    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public sealed class LineTool: DesignTool {

        private Pen pen;
        private List<Point> points;

        public LineTool(DesignSurface surface)
            : base(surface) {

            pen = new Pen {
                Brush = Brushes.White,
                Thickness = 1
            };

            points = new List<Point>();
        }

        protected override void RenderBox(DrawingContext dc, Point startPosition, Point endPosition) {

            dc.DrawLine(pen, startPosition, endPosition);
        }

        public override bool Activate() {

            points.Clear();

            return base.Activate();
        }

        public override void MouseButtonUp(Point position) {
            
            base.MouseButtonUp(position);

            points.Add(position);
        }

        public Point[] Points {
            get {
                return points.ToArray();
            }
        }
    }
}
