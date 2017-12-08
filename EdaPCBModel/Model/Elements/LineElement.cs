namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public class LineElement: SingleLayerElement {

        public enum LineCapStyle {
            Round,
            Flat
        }

        private Point endPosition;
        private double thickness;
        private LineCapStyle lineCap = LineCapStyle.Round;

        /// <summary>
        /// Constructor per defecte de l'objecte.
        /// </summary>
        /// 
        public LineElement():
            base() {

        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// <param name="layer">La capa.</param>
        /// <param name="endPosition">La posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="lineCap">Forma dels extrems de linia.</param>
        /// 
        public LineElement(Point position, Layer layer, Point endPosition, double thickness, LineCapStyle lineCap) :
            base(position, layer) {

            this.endPosition = endPosition;
            this.thickness = thickness;
            this.lineCap = lineCap;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public Point StartPosition {
            get {
                return Position;
            }
            set {
                Position = value;
            }
        }

        public Point EndPosition {
            get {
                return endPosition;
            }
            set {
                endPosition = value;
            }
        }

        public double Thickness {
            get {
                return thickness;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Thickness");
                thickness = value;
            }
        }

        public LineCapStyle LineCap {
            get {
                return lineCap;
            }
            set {
                lineCap = value;
            }
        }
    }
}
