namespace MikroPic.EdaTools.v1.Panel.Model.Elements {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Panel.Model;

    public sealed class MillingElement: PanelElement {

        private Point startPosition;
        private Point endPosition;
        private int thickness;
        private int spacing;
        private int margin;
        private int cuts;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="startPosition">Posicio inicial.</param>
        /// <param name="endPosition">Posicio final.</param>
        /// <param name="thickness">Amplada del tall.</param>
        /// <param name="spacing">Espai entre talls.</param>
        /// <param name="margin">Marge inicial i final.</param>
        /// <param name="cuts">Nombre de talls.</param>
        /// 
        public MillingElement(Point startPosition, Point endPosition, int thickness, int spacing, int margin, int cuts) {

            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.thickness = thickness;
            this.spacing = spacing;
            this.margin = margin;
            this.cuts = cuts;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna la posicio inicial.
        /// </summary>
        /// 
        public Point StartPosition {
            get {
                return startPosition;
            }
            set {
                startPosition = value;
            }
        }

        /// <summary>
        /// Obte o asigna la posicio final.
        /// </summary>
        /// 
        public Point EndPosition {
            get {
                return endPosition;
            }
            set {
                endPosition = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'amplada del talls.
        /// </summary>
        /// 
        public int Tickness {
            get {
                return thickness;
            }
            set {
                thickness = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'espaiat entre talls.
        /// </summary>
        /// 
        public int Spacing {
            get {
                return spacing;
            }
            set {
                spacing = value;
            }
        }

        /// <summary>
        /// Obte o asigna el marge inicial i final.
        /// </summary>
        /// 
        public int Margin {
            get {
                return margin;
            }
            set {
                margin = value;
            }
        }

        /// <summary>
        /// Obte o asigna el numero de talls.
        /// </summary>
        /// 
        public int Cuts {
            get {
                return cuts;
            }
            set {
                cuts = value;
            }
        }
    }
}
