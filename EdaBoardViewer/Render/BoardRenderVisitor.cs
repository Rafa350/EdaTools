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

    public sealed class BoardRenderVisitor : EdaElementVisitor {

        private readonly Color _background = Color.FromRgb(0x30, 0x30, 0x30);
        private readonly EdaLayer _layer;
        private readonly VisualLayer _visualLayer;
        private readonly DrawingContext _context;
        private Font font;

        public BoardRenderVisitor(EdaLayer layer, VisualLayer visualLayer, DrawingContext context) {

            _layer = layer;
            _visualLayer = visualLayer;
            _context = context;
        }

        public override void Visit(EdaBoard board) {

            base.Visit(board);
        }

        public override void Visit(LineElement line) {

            if (_visualLayer.IsVisible(Part, line)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                var lineCap = line.LineCap == LineElement.CapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round;
                var pen = new Pen(brush, line.Thickness, null, lineCap);

                var start = line.StartPosition.ToPoint();
                var end = line.EndPosition.ToPoint();

                _context.DrawLine(pen, start, end);
            }
        }

        public override void Visit(RectangleElement rectangle) {

            if (_visualLayer.IsVisible(Part, rectangle)) {

                var brush = new SolidColorBrush(_visualLayer.Color);
                var geometry = rectangle.GetPolygon(_layer.Side).ToGeometry();

                _context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(ArcElement arc) {

            if (_visualLayer.IsVisible(Part, arc)) {

                var brush = new SolidColorBrush(_visualLayer.Color);
                var geometry = arc.GetPolygon(_layer.Side).ToGeometry();

                _context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(CircleElement circle) {

            if (_visualLayer.IsVisible(Part, circle)) {

                var brush = new SolidColorBrush(_visualLayer.Color);
                var geometry = circle.GetPolygon(_layer.Side).ToGeometry();

                _context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(ViaElement via) {

            if (_visualLayer.IsVisible(Part, via)) {

                switch (_visualLayer.VisualMode) {
                    case VisualMode.Element: {
                        var brush = new SolidColorBrush(_visualLayer.Color);
                        var geometry = via.GetPolygon(_layer.Side).ToGeometry();
                        _context.DrawGeometry(brush, null, geometry);
                    }
                    break;

                    case VisualMode.Drill: {
                        var drillBrush = new SolidColorBrush(_background);
                        var drillGeometry = via.GetDrillPolygon().ToGeometry();
                        _context.DrawGeometry(drillBrush, null, drillGeometry);
                    }
                    break;
                }
            }
        }

        public override void Visit(ThPadElement pad) {

            if (_visualLayer.IsVisible(Part, pad)) {

                switch (_visualLayer.VisualMode) {
                    case VisualMode.Element: {
                        var brush = new SolidColorBrush(_visualLayer.Color);
                        var geometry = pad.GetPolygon(_layer.Side).ToGeometry();
                        _context.DrawGeometry(brush, null, geometry);
                    }
                    break;

                    case VisualMode.Drill: {
                        var drillBrush = new SolidColorBrush(_background);
                        var drillGeometry = pad.GetDrillPolygon().ToGeometry();
                        _context.DrawGeometry(drillBrush, null, drillGeometry);
                    }
                    break;
                }
            }
        }

        public override void Visit(SmdPadElement pad) {

            if (_visualLayer.IsVisible(Part, pad)) {

                var brush = new SolidColorBrush(_visualLayer.Color);
                var geometry = pad.GetPolygon(_layer.Side).ToGeometry();

                _context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(HoleElement hole) {

            if (_visualLayer.IsVisible(Part, hole)) {

                var brush = new SolidColorBrush(_visualLayer.Color);
                var geometry = hole.GetPolygon(_layer.Side).ToGeometry();

                _context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(TextElement text) {

            if (_visualLayer.IsVisible(Part, text)) {

                var paa = new PartAttributeAdapter(Part, text);
                if (paa.IsVisible) {

                    if (font == null)
                        font = Font.Load("font.xml");

                    var td = new TextDrawer(font);
                    IEnumerable<GlyphTrace> glyphTraces = td.Draw(paa.Value, new EdaPoint(0, 0), paa.HorizontalAlign, paa.VerticalAlign, paa.Height);

                    var t = new Transformation();
                    //t.Scale(1, -1);
                    t.Translate(paa.Position);
                    t.Rotate(paa.Position, paa.Rotation);

                    Matrix2D matrix = t.Matrix;
                    var m = new Avalonia.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.Tx, matrix.Ty);
                    using (_context.PushPreTransform(m)) {

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

                        var pen = new Pen(new SolidColorBrush(_visualLayer.Color), text.Thickness, null, PenLineCap.Round, PenLineJoin.Round);

                        _context.DrawGeometry(null, pen, geometry);
                    }
                }
            }
        }

        public override void Visit(RegionElement region) {

            if (_visualLayer.IsVisible(Part, region)) {

                var polygon = _layer.Function == LayerFunction.Signal ?
                    Board.GetRegionPolygon(region, _layer.Id, new Transformation()) :
                    region.GetPolygon(_layer.Side);

                var pen = new Pen(new SolidColorBrush(_visualLayer.Color), region.Thickness, null, PenLineCap.Round, PenLineJoin.Round);
                var brush = new SolidColorBrush(_visualLayer.Color);

                var geometry = polygon.ToGeometry();

                _context.DrawGeometry(brush, pen, geometry);
            }
        }

        public override void Visit(EdaPart part) {

            Transformation transformation = part.GetLocalTransformation();
            Matrix2D matrix = transformation.Matrix;

            var m = new Avalonia.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.Tx, matrix.Ty);
            using (_context.PushPreTransform(m))
                base.Visit(part);
        }
    }
}
