namespace Mkpic.EdaTools.v1.LibraryEditor.Visuals {

    using System.Windows;
    using System.Windows.Shapes;
    using System.Windows.Media;

    public sealed class ThPadShape: FrameworkElement {

        private double drill = 10;
        private double size = 100;

        public ThPadShape() {
        }


        protected override void OnRender(DrawingContext drawingContext) {

            StreamGeometry g = new StreamGeometry();
            using (StreamGeometryContext ctx = g.Open()) {

                Point center = new Point(ActualWidth / 2, ActualHeight / 2);

                double oRadius = size / 2;
                Size oSize = new Size(oRadius, oRadius);

                ctx.BeginFigure(new Point(center.X - oRadius, center.Y), true, true);
                ctx.ArcTo(new Point(center.X + oRadius, center.Y), oSize, 180, true, SweepDirection.Clockwise, true, true);
                ctx.ArcTo(new Point(center.X - oRadius, center.Y), oSize, 180, true, SweepDirection.Clockwise, true, true);

                double iRadius = drill / 2;
                Size iSize = new Size(iRadius, iRadius);

                ctx.BeginFigure(new Point(center.X - iRadius, center.Y), true, true);
                ctx.ArcTo(new Point(center.X + iRadius, center.Y), iSize, 180, true, SweepDirection.Clockwise, true, true);
                ctx.ArcTo(new Point(center.X - iRadius, center.Y), iSize, 180, true, SweepDirection.Clockwise, true, true);
            
            }

            drawingContext.DrawGeometry(Brushes.Green, null, g);
        }
    }
}
