namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Model;
    using MikroPic.EdaTools.v1.Model.Elements;

    public sealed class PolygonVisual: ElementVisual {

        public PolygonVisual(PolygonElement polygon, Part part)
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

                Pen pen = PenCache.Instance.GetPen(layer.Color, Polygon.Thickness, PenLineCap.Round);

                StreamGeometry geometry = new StreamGeometry();
                using (StreamGeometryContext ctx = geometry.Open()) {

                    double x1 = Polygon.Position.X;
                    double y1 = Polygon.Position.Y;

                    ctx.BeginFigure(new Point(x1, y1), Polygon.Thickness == 0, true);
                    foreach (PolygonElement.Segment node in Polygon.Nodes) {
                        if (node.Angle == 0)
                            ctx.LineTo(node.Delta, true, true);
                        else {
                            double co = Math.Sqrt(Math.Pow(node.Delta.X - x1, 2) + Math.Pow(node.Delta.Y - y1, 2)) / 2;
                            double radius = Math.Abs(co / Math.Sin((node.Angle / 2) * Math.PI / 180));
                            ctx.ArcTo(node.Delta, new Size(radius, radius),
                                Math.Abs(node.Angle),
                                true,
                                node.Angle > 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                                true, true);
                        }
                        x1 = node.Delta.X;
                        y1 = node.Delta.Y;
                    }
                }

                if (isMirror)
                    dc.PushTransform(new ScaleTransform(-1, 1));
                dc.DrawGeometry(null, pen, geometry);
                if (isMirror)
                    dc.Pop();

                // Pop de la transformacio global
                //
                if (Part != null)
                    dc.Pop();
            }
        }

        public PolygonElement Polygon {
            get {
                return (PolygonElement) Element;
            }
        }
    }
}