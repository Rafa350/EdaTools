namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;

    public sealed class ViaVisual : ElementVisual {

        private readonly Layer layer;
        private readonly Color color;

        /// <summary>
        /// Constructir de l'objecte
        /// </summary>
        /// <param name="parent">La visual pare.</param>
        /// <param name="element">El element</param>
        /// <param name="layer">La capa en la que es representa.</param>
        /// <param name="color">El color amb el que es representa.</param>
        /// 
        public ViaVisual(DrawingVisual parent, ViaElement element, Layer layer, Color color) :
            base(parent, element) {

            this.layer = layer;
            this.color = color;
        }

        /// <summary>
        /// Dibuixa la representacio del element.
        /// </summary>
        /// <param name="dc">El contexte de representacio.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            if (layer.Function == LayerFunction.Mechanical) {

                Pen pen = dc.GetPen(color, 50000, PenLineCap.Flat);
                dc.DrawEllipse(Brushes.Black, pen, Element.Position, Element.Drill / 2, Element.Drill / 2);
            }

            else {
                if (Element.Shape == ViaElement.ViaShape.Circle) {

                    int size = layer.Id.Side == BoardSide.Inner ? Element.InnerSize : Element.OuterSize;
                    int radius = (size + Element.Drill) / 4;

                    Pen pen = dc.GetPen(color, (size - Element.Drill) / 2, PenLineCap.Flat);
                    dc.DrawEllipse(Brushes.Black, pen, Element.Position, radius, radius);
                }

                else {

                    Polygon polygon = Element.GetPolygon(layer.Id.Side);

                    Brush brush = dc.GetBrush(color);
                    dc.DrawPolygon(brush, null, polygon);
                    dc.DrawEllipse(Brushes.Black, null, Element.Position, Element.Drill / 2, Element.Drill / 2);
                }
            }
        }

        /// <summary>
        /// Obte l'element associat al visual.
        /// </summary>
        /// 
        public new ViaElement Element {
            get {
                return base.Element as ViaElement;
            }
        }
    }
}