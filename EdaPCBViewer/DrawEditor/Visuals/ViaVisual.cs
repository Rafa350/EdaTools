namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class ViaVisual: ElementVisual {

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="via">El element que representa.</param>
        public ViaVisual(ViaElement via)
            : base(via, null) {

            RenderVisual();
        }

        /// <summary>
        /// Renderitza el element.
        /// </summary>
        public override void  RenderVisual() {

            using (DrawingContext dc = RenderOpen()) {

                Brush brush = BrushCache.Instance.GetBrush(Colors.Green);

                dc.DrawPolygon(brush, null, Via.Polygon);
            }
        }

        /// <summary>
        /// Obte el element que representa.
        /// </summary>
        public ViaElement Via {
            get {
                return (ViaElement) Element;
            }
        }
    }
}
