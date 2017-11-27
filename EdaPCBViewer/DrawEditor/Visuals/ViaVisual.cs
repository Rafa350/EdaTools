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

                // Dibuixa el anell de la via
                //
                Brush brush = BrushCache.Instance.GetBrush(Via.Layer.Color);
                switch (Via.Shape) {
                    case ViaElement.ViaShape.Circular:
                        dc.DrawCircularRing(brush, null, Via.Position, Via.Size, Via.Drill);
                        break;

                    case ViaElement.ViaShape.Square:
                        dc.DrawSquareRing(brush, null, Via.Position, Via.Size, Via.Drill);
                        break;

                    case ViaElement.ViaShape.Octogonal:
                        dc.DrawOctogonalRing(brush, null, Via.Position, Via.Size, Via.Drill);
                        break;
                }
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
