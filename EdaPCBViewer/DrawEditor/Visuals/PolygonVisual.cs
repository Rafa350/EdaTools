namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class PolygonVisual: ElementVisual {

        public PolygonVisual(RegionElement polygon, Part part)
            : base(polygon, part) {

            RenderVisual();
        }

        public override void RenderVisual() {

            using (DrawingContext dc = RenderOpen()) {

                // Push de la transformacio global
                //
                if (Part != null) {
                    TransformGroup transform = new TransformGroup();
                    transform.Children.Add(new TranslateTransform(Part.Position.X, Part.Position.Y));
                    transform.Children.Add(new RotateTransform(Part.Rotate, Part.Position.X, Part.Position.Y));
                    dc.PushTransform(transform);
                }
                bool isMirror = Part == null ? false : Part.IsMirror;

                Layer layer = isMirror ? Polygon.MirrorLayer : Polygon.Layer;

                Pen pen = Polygon.Thickness == 0 ?
                    null :
                    PenCache.Instance.GetPen(layer.Color, Polygon.Thickness, PenLineCap.Round);

                Brush brush = Polygon.Thickness == 0 ?
                    new SolidColorBrush(layer.Color) :
                    null;

                StreamGeometry geometry = new StreamGeometry();
                using (StreamGeometryContext ctx = geometry.Open()) {

                    double x1 = Polygon.Position.X;
                    double y1 = Polygon.Position.Y;
                    double angle = 0;

                    ctx.BeginFigure(new Point(x1, y1), Polygon.Thickness == 0, true);
                    foreach (RegionElement.Segment node in Polygon.Segments) {
                        if (angle == 0)
                            ctx.LineTo(node.Position, true, true);
                        else {
                            double co = Math.Sqrt(Math.Pow(node.Position.X - x1, 2) + Math.Pow(node.Position.Y - y1, 2)) / 2;
                            double radius = Math.Abs(co / Math.Sin((angle / 2) * Math.PI / 180));
                            ctx.ArcTo(node.Position, new Size(radius, radius),
                                Math.Abs(angle),
                                true,
                                angle > 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                                true, true);
                        }
                        x1 = node.Position.X;
                        y1 = node.Position.Y;
                        angle = node.Angle;
                    }
                }

                if (isMirror)
                    dc.PushTransform(new ScaleTransform(-1, 1));
                dc.DrawGeometry(brush, pen, geometry);
                if (isMirror)
                    dc.Pop();

                // Pop de la transformacio global
                //
                if (Part != null)
                    dc.Pop();
            }
        }

        public RegionElement Polygon {
            get {
                return (RegionElement) Element;
            }
        }
    }
}