namespace MikroPic.EdaTools.v1.Panel.Model.Items {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Panel.Model;

    public sealed class CutItem : EdaPanelItem {

        private EdaPoint _startPosition;
        private EdaPoint _endPosition;
        private int _thickness;
        private int _margin;
        private int _cuts;
        private int _cutSpacing;
        private int _holes;
        private int _holeDiameter;
        private int _holeSpacing;

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
        public CutItem(EdaPoint startPosition, EdaPoint endPosition, int thickness, int margin, int cuts, int cutSpacing, int holes, int holeDiameter, int holeSpacing) {

            _startPosition = startPosition;
            _endPosition = endPosition;
            _thickness = thickness;
            _margin = margin;
            _cuts = cuts;
            _cutSpacing = cutSpacing;
            _holes = holes;
            _holeDiameter = holeDiameter;
            _holeSpacing = holeSpacing;
        }

        public override void AcceptVisitor(IEdaPanelVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna la posicio inicial.
        /// </summary>
        /// 
        public EdaPoint StartPosition {
            get {
                return _startPosition;
            }
            set {
                _startPosition = value;
            }
        }

        /// <summary>
        /// Obte o asigna la posicio final.
        /// </summary>
        /// 
        public EdaPoint EndPosition {
            get {
                return _endPosition;
            }
            set {
                _endPosition = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'amplada del talls.
        /// </summary>
        /// 
        public int Thickness {
            get {
                return _thickness;
            }
            set {
                _thickness = value;
            }
        }

        /// <summary>
        /// Obte o asigna el marge inicial i final.
        /// </summary>
        /// 
        public int Margin {
            get {
                return _margin;
            }
            set {
                _margin = value;
            }
        }

        /// <summary>
        /// Obte o asigna el nombre de forats.
        /// </summary>
        /// 
        public int Holes {
            get {
                return _holes;
            }
            set {
                _holes = value;
            }
        }

        /// <summary>
        /// Obte o asigna el diabetre dels forats.
        /// </summary>
        /// 
        public int HoleDiameter {
            get {
                return _holeDiameter;
            }
            set {
                _holeDiameter = value;
            }
        }

        /// <summary>
        /// Obte o asigna la distancia entre forats.
        /// </summary>
        /// 
        public int HoleSpacing {
            get {
                return _holeSpacing;
            }
            set {
                _holeSpacing = value;
            }
        }

        /// <summary>
        /// Obte o asigna el numero de talls.
        /// </summary>
        /// 
        public int Cuts {
            get {
                return _cuts;
            }
            set {
                _cuts = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'espaiat entre talls.
        /// </summary>
        /// 
        public int CutSpacing {
            get {
                return _cutSpacing;
            }
            set {
                _cutSpacing = value;
            }
        }
    }
}
