using System;
using System.Collections.Generic;
using Avalonia.Media;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Geometry.Utils;
using MikroPic.EdaTools.v1.Core.Infrastructure;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace EdaBoardViewer.Render {

    public sealed class BoardRenderVisitor: EdaElementVisitor {

        private readonly Color _background = Color.FromRgb(0x30, 0x30, 0x30);
        private readonly EdaLayer _layer;
        private readonly VisualLayer _visualLayer;
        private readonly DrawingContext _context;
        private readonly PolygonCache _polygonCache;
        private Font _font;

        public BoardRenderVisitor(EdaLayer layer, VisualLayer visualLayer, DrawingContext context, PolygonCache polygonCache) {

            _layer = layer;
            _visualLayer = visualLayer;
            _context = context;
            _polygonCache = polygonCache;
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaBoard board) {

            base.Visit(board);
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaLineElement line) {

            if (_visualLayer.IsVisible(Part, line)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                var lineCap = line.LineCap == EdaLineCap.Flat ? PenLineCap.Flat : PenLineCap.Round;
                var pen = new Pen(brush, line.Thickness, null, lineCap);

                var start = line.StartPosition.ToPoint();
                var end = line.EndPosition.ToPoint();

                _context.DrawLine(pen, start, end);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaRectangleElement rectangle) {

            if (_visualLayer.IsVisible(Part, rectangle)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                var polygon = _polygonCache.GetPolygon(rectangle, _layer.Id);
                var geometry = polygon.ToGeometry();

                _context.DrawGeometry(brush, null, geometry);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaArcElement arc) {

            if (_visualLayer.IsVisible(Part, arc)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                var polygon = _polygonCache.GetPolygon(arc, _layer.Id);
                var geometry = polygon.ToGeometry();

                _context.DrawGeometry(brush, null, geometry);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaCircleElement circle) {

            if (_visualLayer.IsVisible(Part, circle)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                var polygon = _polygonCache.GetPolygon(circle, _layer.Id);
                var geometry = polygon.ToGeometry();

                _context.DrawGeometry(brush, null, geometry);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaCircularHoleElement element) {

            if (_visualLayer.IsVisible(Part, element)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                var polygon = _polygonCache.GetPolygon(element, _layer.Id);
                var geometry = polygon.ToGeometry();

                _context.DrawGeometry(brush, null, geometry);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaViaElement via) {

            if (_visualLayer.IsVisible(Part, via)) {

                switch (_visualLayer.VisualMode) {
                    case VisualMode.Element: {
                            var brush = new SolidColorBrush(_visualLayer.Color);
                            var polygon = _polygonCache.GetPolygon(via, _layer.Id);
                            var geometry = polygon.ToGeometry();
                            _context.DrawGeometry(brush, null, geometry);
                        }
                        break;

                    case VisualMode.Drill: {
                            var brush = new SolidColorBrush(_background);
                            var polygon = _polygonCache.GetDrillPolygon(via);
                            var geometry = polygon.ToGeometry();
                            _context.DrawGeometry(brush, null, geometry);
                        }
                        break;
                }
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaThtPadElement pad) {

            if (_visualLayer.IsVisible(Part, pad)) {

                switch (_visualLayer.VisualMode) {
                    case VisualMode.Element: {
                            var brush = new SolidColorBrush(_visualLayer.Color);
                            var polygon = _polygonCache.GetPolygon(pad, _layer.Id);
                            var geometry = polygon.ToGeometry();
                            _context.DrawGeometry(brush, null, geometry);
                        }
                        break;

                    case VisualMode.Drill: {
                            var brush = new SolidColorBrush(_background);
                            var polygon = _polygonCache.GetDrillPolygon(pad);
                            var geometry = polygon.ToGeometry();
                            _context.DrawGeometry(brush, null, geometry);
                        }
                        break;
                }
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaSmtPadElement pad) {

            if (_visualLayer.IsVisible(Part, pad)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                EdaPolygon polygon;
                if ((_layer.Id == EdaLayerId.TopCream) || (_layer.Id == EdaLayerId.BottomCream)) {
                    var spacing = Math.Max(pad.Size.Width, pad.Size.Height) * pad.PasteReductionRatio / 2;
                    polygon = _polygonCache.GetOutlinePolygon(pad, _layer.Id, -spacing);
                }
                else
                    polygon = _polygonCache.GetPolygon(pad, _layer.Id);

                var geometry = polygon.ToGeometry();
                _context.DrawGeometry(brush, null, geometry);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaTextElement text) {

            if (_visualLayer.IsVisible(Part, text)) {

                var paa = new PartAttributeAdapter(Part, text);
                if (paa.IsVisible) {

                    if (_font == null)
                        _font = Font.Load("font.xml");

                    var td = new TextDrawer(_font);
                    IEnumerable<GlyphTrace> glyphTraces = td.Draw(paa.Value, new EdaPoint(0, 0), paa.HorizontalAlign, paa.VerticalAlign, paa.Height);

                    var t = new EdaTransformation();
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

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaRegionElement region) {

            if (_visualLayer.IsVisible(Part, region)) {

                var polygons = _layer.Function == LayerFunction.Signal ?
                    _polygonCache.GetPolygons(Board, region, _layer.Id) :
                    new List<EdaPolygon>() { _polygonCache.GetPolygon(region, _layer.Id) };

                var pen = new Pen(new SolidColorBrush(_visualLayer.Color), region.Thickness, null, PenLineCap.Round, PenLineJoin.Round);
                var brush = new SolidColorBrush(_visualLayer.Color);

                var geometry = polygons.ToGeometry();

                _context.DrawGeometry(brush, pen, geometry);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaPart part) {

            EdaTransformation transformation = part.GetLocalTransformation();
            Matrix2D matrix = transformation.Matrix;

            var m = new Avalonia.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.Tx, matrix.Ty);
            using (_context.PushPreTransform(m))
                base.Visit(part);
        }
    }
}
