namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;
    using Point = MikroPic.EdaTools.v1.Base.Geometry.Point;

    public sealed class RectangleVisual: ElementVisual {

        private readonly Color color;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">La visual pare.</param>
        /// <param name="rectangle">L'element.</param>
        /// <param name="color">El color amb el que es representa.</param>
        /// 
        public RectangleVisual(DrawingVisual parent, RectangleElement rectangle, Color color):
            base(parent, rectangle) {

            this.color = color;
        }

        /// <summary>
        /// Dibuixa la representacio de l'element.
        /// </summary>
        /// <param name="dc">El context de representacio.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            Pen pen = Element.Thickness == 0 ? null : dc.GetPen(color, Element.Thickness, PenLineCap.Flat);
            Brush brush = Element.Filled ? dc.GetBrush(color) : null;

            if (Element.Radius == 0)
                dc.DrawRectangle(brush, pen, Element.Position, Element.Size);
            else
                dc.DrawRoundedRectangle(brush, pen, Element.Position, Element.Size, Element.Radius, Element.Radius);
        }

        /// <summary>
        /// Obte l'element associat al visual.
        /// </summary>
        public new RectangleElement Element {
            get {
                return base.Element as RectangleElement;
            }
        }
    }
}
