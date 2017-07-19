namespace Eda.PCBViewer.DrawEditor.Tools {

    using System.Windows;
    using System.Windows.Media;

    public sealed class PadTool: DesignTool {

        private readonly Brush brush;
        private double width;
        private double height;
        private double radius;

        public PadTool(DesignSurface surface)
            : base(surface) {

            brush = Brushes.Red;

            width = 30;
            height = 60;
            radius = 5;
        }

        protected override void RenderBox(DrawingContext dc, Point startPosition, Point endPosition) {

            Rect rect = new Rect(
                new Point(endPosition.X - (width / 2), endPosition.Y - (height / 2)), 
                new Size(width, height));
            dc.DrawRoundedRectangle(brush, null, rect, radius, radius);
        }
    }
}
