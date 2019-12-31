namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using System;
    using System.Windows.Media;

    public sealed class PartVisual: VisualBase {

        private readonly Part part;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">La visual pare.</param>
        /// <param name="part">El part.</param>
        /// 
        public PartVisual(DrawingVisual parent, Part part):
            base(parent) {

            if (part == null)
                throw new ArgumentNullException("part");

            this.part = part;
        }

        /// <summary>
        /// Dibuixa la representacio del part.
        /// </summary>
        /// <param name="dc">El context de representacio.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            dc.DrawRectangle(Brushes.Blue, null, new Point(0, 0), new Size(3000000, 3000000));
        }

        /// <summary>
        /// Obte el part associat al visual.
        /// </summary>
        /// 
        public Part Part {
            get {
                return part;
            }
        }
    }
}
