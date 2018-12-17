namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;

    public sealed class LineVisual: ElementVisual {

        private readonly Color color;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">La visual pare.</param>
        /// <param name="line">L'element.</param>
        /// <param name="color">El color amb el que es representa.</param>
        /// 
        public LineVisual(DrawingVisual parent, LineElement line, Color color):
            base(parent, line) {

            this.color = color;
        }

        /// <summary>
        /// Dibuixa la representacio de l'element.
        /// </summary>
        /// <param name="dc">El context de representacio.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            Pen pen = dc.GetPen(color, Element.Thickness, Element.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);

            dc.DrawLine(pen, Element.StartPosition, Element.EndPosition);
        }

        /// <summary>
        /// Obte l'element associat al visual.
        /// </summary>
        public new LineElement Element {
            get {
                return base.Element as LineElement;
            }
        }
    }
}
