namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System.Windows.Media;

    public sealed class ArcVisual: ElementVisual {

        private readonly Color color;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">La visual pare.</param>
        /// <param name="arc">L'element.</param>
        /// <param name="color">El color amb el que es representa.</param>
        /// 
        public ArcVisual(DrawingVisual parent, ArcElement arc, Color color):
            base(parent, arc) {

            this.color = color;
        }

        /// <summary>
        /// Dibuixa la representacio de l'element.
        /// </summary>
        /// <param name="dc">El context de representacio.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            Pen pen = dc.GetPen(color, Element.Thickness, Element.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);
            dc.DrawArc(pen, Element.StartPosition, Element.EndPosition, Element.Radius, Element.Angle);
        }

        /// <summary>
        /// Obte l'element associat al visual.
        /// </summary>
        public new ArcElement Element {
            get {
                return base.Element as ArcElement;
            }
        }
    }
}
