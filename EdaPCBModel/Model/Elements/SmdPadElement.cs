namespace MikroPic.EdaTools.v1.Model.Elements {

    using System.Windows;

    public sealed class SmdPadElement: PadElement {

        private Size size;
        private double roundnes;

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna el tamany del pad.
        /// </summary>
        public Size Size {
            get {
                return size;
            }
            set {
                size = value;
            }
        }

        /// <summary>
        /// Obte o asigna el factor d'arrodoniment de les cantonades del pad.
        /// </summary>
        public double Roundnes {
            get {
                return roundnes;
            }
            set {
                if (value < 0)
                    roundnes = 0;
                else if (value > 1)
                    roundnes = 1;
                else
                    roundnes = value;
            }
        }
    }
}
