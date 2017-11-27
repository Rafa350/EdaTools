namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;

    public sealed class ThPadElement: PadElement {

        public enum ThPadShape {
            Square,
            Octogonal,
            Circular,
            Oval
        }

        private static double autosizefactor = 1.3;

        private ThPadShape shape = ThPadShape.Circular;
        private double size;
        private bool autoSize = false;
        private double drill;

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public ThPadShape Shape {
            get {
                return shape;
            }
            set {
                shape = value;
            }
        }

        /// <summary>
        /// Obte o asigna el diametre del forat.
        /// </summary>
        public double Drill {
            get {
                return drill;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Drill");
                drill = value;
            }
        }


        /// <summary>
        /// Obte o asigna el tamany del pad.
        /// </summary>
        public double Size {
            get {
                return autoSize ? size * autosizefactor : size;
            }
            set {
                size = value;
                autoSize = false;
            }
        }

        /// <summary>
        /// Asigna o obte el indicador de tamany automatic.
        /// </summary>
        public bool AutoSize {
            get {
                return autoSize;
            }
            set {
                autoSize = value;
            }
        }
    }
}
