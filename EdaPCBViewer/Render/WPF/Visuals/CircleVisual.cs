namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;

    public sealed class CircleVisual: ElementVisual {

        private readonly Color color;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">La visual pare.</param>
        /// <param name="circle">L'element.</param>
        /// <param name="color">El color amb el que es representa.</param>
        /// 
        public CircleVisual(DrawingVisual parent, CircleElement circle, Color color):
            base(parent, circle) {

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

            dc.DrawEllipse(brush, pen, Element.Position, Element.Radius, Element.Radius);
        }

        /// <summary>
        /// Obte l'element associat al visual.
        /// </summary>
        public new CircleElement Element {
            get {
                return base.Element as CircleElement;
            }
        }
    }
}
