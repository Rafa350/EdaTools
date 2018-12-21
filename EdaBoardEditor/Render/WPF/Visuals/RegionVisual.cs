namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;

    public sealed class RegionVisual: ElementVisual {

        private readonly Board board;
        private readonly Layer layer;
        private readonly Color color;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">La visual pare.</param>
        /// <param name="region">L'element.</param>
        /// <param name="color">El color amb el que es representa.</param>
        /// 
        public RegionVisual(DrawingVisual parent, RegionElement region, Board board, Layer layer, Color color):
            base(parent, region) {

            this.board = board;
            this.layer = layer;
            this.color = color;
        }

        /// <summary>
        /// Dibuixa la representacio de l'element.
        /// </summary>
        /// <param name="dc">El context de representacio.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            Polygon polygon = layer.Function == LayerFunction.Signal ?
                board.GetRegionPolygon(Element, layer, new Transformation()) :
                Element.GetPolygon(layer.Id.Side);

            Pen pen = Element.Thickness > 0 ? dc.GetPen(color, Element.Thickness, PenLineCap.Round) : null;
            Brush brush = Element.Filled ? dc.GetBrush(color) : null;
            dc.DrawPolygon(brush, pen, polygon);
        }

        /// <summary>
        /// Obte l'element associat al visual.
        /// </summary>
        public new RegionElement Element {
            get {
                return base.Element as RegionElement;
            }
        }
    }
}
