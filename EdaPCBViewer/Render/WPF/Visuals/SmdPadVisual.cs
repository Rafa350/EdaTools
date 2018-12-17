namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;
    using Rect = MikroPic.EdaTools.v1.Base.Geometry.Rect;

    public sealed class SmdPadVisual: ElementVisual {

        private readonly Color color;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">La visual pare.</param>
        /// <param name="pad">L'element.</param>
        /// <param name="color">El color amb el que es representa.</param>
        /// 
        public SmdPadVisual(DrawingVisual parent, SmdPadElement pad, Color color):
            base(parent, pad) {

            this.color = color;
        }

        /// <summary>
        /// Dibuixa la representacio de l'element.
        /// </summary>
        /// <param name="dc">El context de representacio.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            Rect rect = new Rect(
                Element.Position.X - (Element.Size.Width / 2),
                Element.Position.Y - (Element.Size.Height / 2),
                Element.Size.Width,
                Element.Size.Height);

            Brush brush = dc.GetBrush(color);
            if (Element.Radius == 0)
                dc.DrawRectangle(brush, null, rect);
            else
                dc.DrawRoundedRectangle(brush, null, rect, Element.Radius, Element.Radius);
        }

        /// <summary>
        /// Obte l'element associat al visual.
        /// </summary>
        public new SmdPadElement Element {
            get {
                return base.Element as SmdPadElement;
            }
        }
    }
}
