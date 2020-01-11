namespace EdaBoardViewer.Render {

    using Avalonia.Media;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Utils;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

    public sealed class BoardRenderVisitor: ElementVisitor {

        private readonly Layer layer;
        private readonly VisualLayer visualLayer;
        private readonly DrawingContext context;

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

                var brush = new SolidColorBrush(visualLayer.Color);
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
            }
        }

        public override void Visit(RegionElement region) {

            if (visualLayer.IsVisible(Part, region)) {
             
                var polygon = layer.Function == LayerFunction.Signal ?
                    Board.GetRegionPolygon(region, layer.Name, new Transformation()) :
                    region.GetPolygon(layer.Side);

                var brush = new SolidColorBrush(visualLayer.Color);

                var geometry = polygon.ToGeometry();

                context.DrawGeometry(brush, null, geometry);
            }
        }

        public override void Visit(Part part) {

            Transformation transformation = part.GetLocalTransformation();
            Matrix2D matrix = transformation.Matrix;
            
            var m = new Avalonia.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
            using (context.PushPreTransform(m)) {

                base.Visit(part);
            }
        }
    }
}
