namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System.Windows;
    using System.Windows.Media;

    public sealed class GridVisual: DrawingVisual {

        public GridVisual(double thickness) {

            using (DrawingContext dc = RenderOpen()) {

                thickness = 0.05;

                Pen pen = new Pen();
                pen.Brush = Brushes.White;
                pen.Thickness = thickness;
                pen.DashStyle = new DashStyle(new double[] { 2 / thickness, 8 / thickness }, 0);
                pen.Freeze();

                for (double x = 0; x < 80; x++)
                    dc.DrawLine(pen, new Point(x, 0), new Point(x, 80));

                for (double y = 0; y < 80; y++)
                    dc.DrawLine(pen, new Point(0, y), new Point(80, y));
            }
        }
    }
}
