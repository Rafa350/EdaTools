namespace EdaBoardViewer.Render {

    using System.Collections.Generic;
    using Avalonia.Media;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Base.Geometry.Utils;
    using MikroPic.EdaTools.v1.Core.Infrastructure;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

    public sealed class BoardRenderVisitor : ElementVisitor {

        private readonly Layer layer;
        private readonly VisualLayer visualLayer;
        private readonly DrawingContext context;
        private Font font;

        public BoardRenderVisitor(Layer layer, VisualLayer visualLayer, DrawingContext context) {

            this.layer = layer;
            this.visualLayer = visualLayer;
            this.context = context;
        }

        public override void Visit(Board board) {

            base.Visit(board);
        }

        public override void Visit(LineElement line) {

            if (visualLayer.IsVisible(Part, line)) {

                var brush = new SolidColorBrush(visualLayer.Color);

                var lineCap = line.LineCap == LineElement.CapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round;
                var pen = new Pen(brush, line.Thickness, null, lineCap);

                var start = line.StartPosition.ToPoint();
                var end = line.EndPosition.ToPoint();

                context.DrawLine(pen, start, end);
            }
        }

        public override void Visit(RectangleElement rectangle) {

            if (visualLayer.IsVisible(Part, rectangle)) {

                var brush = new SolidColorBrush(visualLayer.Color);
                var geometry = rectangle.GetPolygon(layer.Side).ToGeometry();

                context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(ArcElement arc) {

            if (visualLayer.IsVisible(Part, arc)) {

                var brush = new SolidColorBrush(visualLayer.Color);
                var geometry = arc.GetPolygon(layer.Side).ToGeometry();

                context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(CircleElement circle) {

            if (visualLayer.IsVisible(Part, circle)) {

                var brush = new SolidColorBrush(visualLayer.Color);
                var geometry = circle.GetPolygon(layer.Side).ToGeometry();

                context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(ViaElement via) {

            if (visualLayer.IsVisible(Part, via)) {

                var brush = new SolidColorBrush(visualLayer.Color);
                var geometry = via.GetPolygon(layer.Side).ToGeometry();

                context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(ThPadElement pad) {

            if (visualLayer.IsVisible(Part, pad)) {

                var brush = new SolidColorBrush(visualLayer.Color);
                var geometry = pad.GetPolygon(layer.Side).ToGeometry();

                context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(SmdPadElement pad) {

            if (visualLayer.IsVisible(Part, pad)) {

                var brush = new SolidColorBrush(pad.Name == "1" ? Colors.AliceBlue : visualLayer.Color);
                var geometry = pad.GetPolygon(layer.Side).ToGeometry();

                context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(HoleElement hole) {

            if (visualLayer.IsVisible(Part, hole)) {

                var brush = new SolidColorBrush(visualLayer.Color);
                var geometry = hole.GetPolygon(layer.Side).ToGeometry();

                context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(TextElement text) {

            if (visualLayer.IsVisible(Part, text)) {

                var paa = new PartAttributeAdapter(Part, text);
                if (paa.IsVisible) {

                    if (font == null)
                        font = Font.Load("font.xml");

                    var td = new TextDrawer(font);
                    IEnumerable<GlyphTrace> glyphTraces = td.Draw(paa.Value, new Point(0, 0), paa.HorizontalAlign, paa.VerticalAlign, paa.Height);

                    var t = new Transformation();
                    //t.Scale(1, -1);
                    t.Translate(paa.Position);
                    t.Rotate(paa.Position, paa.Rotation);

                    Matrix2D matrix = t.Matrix;
                    var m = new Avalonia.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.Tx, matrix.Ty);
                    using (context.PushPreTransform(m)) {

                        bool first = true;
                        var geometry = new StreamGeometry();
                        using (var gc = geometry.Open()) {
                            foreach (var glyphTrace in glyphTraces) {
                                var p = new Avalonia.Point(glyphTrace.Position.X, glyphTrace.Position.Y);
                                if (first || !glyphTrace.Stroke) {
                                    gc.BeginFigure(p, false);
                                    first = false;
                                }
                                else
                                    gc.LineTo(p);
                            }
                        }

                        var pen = new Pen(new SolidColorBrush(visualLayer.Color), text.Thickness, null, PenLineCap.Round, PenLineJoin.Round);

                        context.DrawGeometry(null, pen, geometry);
                    }
                }
            }
        }

        public override void Visit(RegionElement region) {

            if (visualLayer.IsVisible(Part, region)) {

                var polygon = layer.Function == LayerFunction.Signal ?
                    Board.GetRegionPolygon(region, layer, new Transformation()) :
                    region.GetPolygon(layer.Side);

                var pen = new Pen(new SolidColorBrush(visualLayer.Color), region.Thickness, null, PenLineCap.Round, PenLineJoin.Round);
                var brush = new SolidColorBrush(visualLayer.Color);

                var geometry = polygon.ToGeometry();

                context.DrawGeometry(brush, pen, geometry);
            }
        }

        public override void Visit(Part part) {

            Transformation transformation = part.GetLocalTransformation();
            Matrix2D matrix = transformation.Matrix;

            var m = new Avalonia.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.Tx, matrix.Ty);
            using (context.PushPreTransform(m))
                base.Visit(part);
        }
    }
}
