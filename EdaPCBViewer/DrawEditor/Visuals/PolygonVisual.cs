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
                    transform.Children.Add(new RotateTransform(Part.Rotation, Part.Position.X, Part.Position.Y));
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

                    double x1 = 0;
                    double y1 = 0;
                    double angle = 0;

                    bool first = true;
                    foreach (RegionElement.Segment segment in Polygon.Segments) {
                        if (first) {
                            first = false;
                            ctx.BeginFigure(segment.Position, Polygon.Thickness == 0, true);
                        }
                        else {
                            if (angle == 0)
                                ctx.LineTo(segment.Position, true, true);
                            else {
                                double co = Math.Sqrt(Math.Pow(segment.Position.X - x1, 2) + Math.Pow(segment.Position.Y - y1, 2)) / 2;
                                double radius = Math.Abs(co / Math.Sin((angle / 2) * Math.PI / 180));
                                ctx.ArcTo(segment.Position, new Size(radius, radius),
                                    Math.Abs(angle),
                                    true,
                                    angle > 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                                    true, true);
                            }
                        }
                        x1 = segment.Position.X;
                        y1 = segment.Position.Y;
                        angle = segment.Angle;
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