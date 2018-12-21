namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;

    public sealed class HoleVisual: ElementVisual {

        private readonly Color color;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">La visual pare.</param>
        /// <param name="circle">L'element.</param>
        /// <param name="color">El color amb el que es representa.</param>
        /// 
        public HoleVisual(DrawingVisual parent, HoleElement hole, Color color):
            base(parent, hole) {

            this.color = color;
        }

        /// <summary>
        /// Dibuixa la representacio de l'element.
        /// </summary>
        /// <param name="dc">El context de representacio.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            Pen pen = dc.GetPen(color, 50000, PenLineCap.Flat);

            dc.DrawEllipse(Brushes.Black, pen, Element.Position, Element.Drill / 2, Element.Drill / 2);
        }

        /// <summary>
        /// Obte l'element associat al visual.
        /// </summary>
        public new HoleElement Element {
            get {
                return base.Element as HoleElement;
            }
        }
    }
}
