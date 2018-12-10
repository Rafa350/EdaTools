namespace MikroPic.EdaTools.v1.Panel.Model.Items {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Panel.Model;

    public sealed class CutItem: ProjectItem {

        private Point startPosition;
        private Point endPosition;
        private int thickness;
        private int margin;
        private int cuts;
        private int cutSpacing;
        private int holes;
        private int holeDiameter;
        private int holeSpacing;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="startPosition">Posicio inicial.</param>
        /// <param name="endPosition">Posicio final.</param>
        /// <param name="thickness">Amplada del tall.</param>
        /// <param name="margin">Marge inicial i final.</param>
        /// <param name="cuts">Nombre de talls.</param>
        /// <param name="cutSpacing">Espai entre talls.</param>
        /// <param name="holes">Nombre de forats.</param>
        /// <param name="holeDiameter">Diametre dels forats.</param>
        /// <param name="holeSpacing">Espai entre forats.</param>
        /// 
        public CutItem(Point startPosition, Point endPosition, int thickness, int margin, int cuts, int cutSpacing, int holes, int holeDiameter, int holeSpacing) {

            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.thickness = thickness;
            this.margin = margin;
            this.cuts = cuts;
            this.cutSpacing = cutSpacing;
            this.holes = holes;
            this.holeDiameter = holeDiameter;
            this.holeSpacing = holeSpacing;
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
        /// Obte o asigna el nombre de forats.
        /// </summary>
        /// 
        public int Holes {
            get {
                return holes;
            }
            set {
                holes = value;
            }
        }

        /// <summary>
        /// Obte o asigna el diabetre dels forats.
        /// </summary>
        /// 
        public int HoleDiameter {
            get {
                return holeDiameter;
            }
            set {
                holeDiameter = value;
            }
        }

        /// <summary>
        /// Obte o asigna la distancia entre forats.
        /// </summary>
        /// 
        public int HoleSpacing {
            get {
                return holeSpacing;
            }
            set {
                holeSpacing = value;
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

        /// <summary>
        /// Obte o asigna l'espaiat entre talls.
        /// </summary>
        /// 
        public int CutSpacing {
            get {
                return cutSpacing;
            }
            set {
                cutSpacing = value;
            }
        }
    }
}
