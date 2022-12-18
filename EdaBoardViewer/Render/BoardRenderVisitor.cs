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
        public override void Visit(EdaLineElement element) {

            if (_visualLayer.IsVisible(Part, element)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                var lineCap = element.LineCap == EdaLineCap.Flat ? PenLineCap.Flat : PenLineCap.Round;
                var pen = new Pen(brush, element.Thickness, null, lineCap);

                var start = element.StartPosition.ToPoint();
                var end = element.EndPosition.ToPoint();

                _context.DrawLine(pen, start, end);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaRectangleElement element) {

            if (_visualLayer.IsVisible(Part, element)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                var polygon = _polygonCache.GetPolygon(element, _layer.Id);
                var geometry = polygon.ToGeometry();

                _context.DrawGeometry(brush, null, geometry);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaArcElement element) {

            if (_visualLayer.IsVisible(Part, element)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                var polygon = _polygonCache.GetPolygon(element, _layer.Id);
                var geometry = polygon.ToGeometry();

                _context.DrawGeometry(brush, null, geometry);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaCircleElement element) {

            if (_visualLayer.IsVisible(Part, element)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                var polygon = _polygonCache.GetPolygon(element, _layer.Id);
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
        public override void Visit(EdaViaElement element) {

            if (_visualLayer.IsVisible(Part, element)) {

                switch (_visualLayer.VisualMode) {
                    case VisualMode.Element: {
                            var brush = new SolidColorBrush(_visualLayer.Color);
                            var polygon = _polygonCache.GetPolygon(element, _layer.Id);
                            var geometry = polygon.ToGeometry();
                            _context.DrawGeometry(brush, null, geometry);
                        }
                        break;

                    case VisualMode.Drill: {
                            var brush = new SolidColorBrush(_background);
                            var polygon = _polygonCache.GetDrillPolygon(element);
                            var geometry = polygon.ToGeometry();
                            _context.DrawGeometry(brush, null, geometry);
                        }
                        break;
                }
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaThtPadElement element) {

            if (_visualLayer.IsVisible(Part, element)) {

                switch (_visualLayer.VisualMode) {
                    case VisualMode.Element: {
                            var brush = new SolidColorBrush(_visualLayer.Color);
                            var polygon = _polygonCache.GetPolygon(element, _layer.Id);
                            var geometry = polygon.ToGeometry();
                            _context.DrawGeometry(brush, null, geometry);
                        }
                        break;

                    case VisualMode.Drill: {
                            var brush = new SolidColorBrush(_background);
                            var polygon = _polygonCache.GetDrillPolygon(element);
                            var geometry = polygon.ToGeometry();
                            _context.DrawGeometry(brush, null, geometry);
                        }
                        break;
                }
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaSmtPadElement element) {

            if (_visualLayer.IsVisible(Part, element)) {

                var brush = new SolidColorBrush(_visualLayer.Color);

                EdaPolygon polygon;
                if ((_layer.Id == EdaLayerId.TopCream) || (_layer.Id == EdaLayerId.BottomCream)) {
                    var spacing = Math.Max(element.Size.Width, element.Size.Height) * element.PasteReductionRatio / 2;
                    polygon = _polygonCache.GetOutlinePolygon(element, _layer.Id, -spacing);
                }
                else
                    polygon = _polygonCache.GetPolygon(element, _layer.Id);

                var geometry = polygon.ToGeometry();
                _context.DrawGeometry(brush, null, geometry);
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaTextElement element) {

            if (_visualLayer.IsVisible(Part, element)) {

                var paa = new PartAttributeAdapter(Part, element);
                if (paa.IsVisible) {

                    if (_font == null)
                        _font = Font.Load("font.xml");

                    var td = new TextDrawer(_font);
                    IEnumerable<GlyphTrace> glyphTraces = td.Draw(paa.Value, new EdaPoint(0, 0), paa.HorizontalAlign, paa.VerticalAlign, paa.Height);

                    var t = new EdaTransformation();
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

                        var pen = new Pen(new SolidColorBrush(_visualLayer.Color), element.Thickness, null, PenLineCap.Round, PenLineJoin.Round);

                        _context.DrawGeometry(null, pen, geometry);
                    }
                }
            }
        }

        /// <inheritdoc/>
        /// 
        public override void Visit(EdaRegionElement element) {

            if (_visualLayer.IsVisible(Part, element)) {

                var polygons = _layer.Function == LayerFunction.Signal ?
                    _polygonCache.GetPolygons(Board, element, _layer.Id) :
                    new List<EdaPolygon>() { _polygonCache.GetPolygon(element, _layer.Id) };

                var pen = new Pen(new SolidColorBrush(_visualLayer.Color), element.Thickness, null, PenLineCap.Round, PenLineJoin.Round);
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
