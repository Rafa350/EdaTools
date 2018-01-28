namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System.Windows.Media;
    
    public enum LayerFunction {
        Unknown,
        Signal,
        Design,
        Mechanical,
    }

    public sealed class Layer: IVisitable {

        public static readonly string TopName = "Top";
        public static readonly string Inner1Name = "Inner1";
        public static readonly string Inner2Name = "Inner2";
        public static readonly string Inner3Name = "Inner3";
        public static readonly string Inner4Name = "Inner4";
        public static readonly string Inner5Name = "Inner5";
        public static readonly string Inner6Name = "Inner6";
        public static readonly string Inner7Name = "Inner7";
        public static readonly string Inner8Name = "Inner8";
        public static readonly string Inner9Name = "Inner9";
        public static readonly string Inner10Name = "Inner10";
        public static readonly string Inner11Name = "Inner11";
        public static readonly string Inner12Name = "Inner12";
        public static readonly string Inner13Name = "Inner13";
        public static readonly string Inner14Name = "Inner14";
        public static readonly string BottomName = "Bottom";
        public static readonly string TopStopName = "TopStop";
        public static readonly string BottomStopName = "BottomStop";
        public static readonly string TopCreamName = "TopCream";
        public static readonly string BottomCreamName = "BottomCream";
        public static readonly string TopGlueName = "TopGlue";
        public static readonly string BottomGlueName = "BottomGlue";
        public static readonly string TopPlaceName = "TopPlace";
        public static readonly string BottomPlaceName = "BottomPlace";
        public static readonly string TopDocumentName = "TopDocument";
        public static readonly string BottomDocumentName = "BottomDocument";
        public static readonly string TopNamesName = "TopNames";
        public static readonly string BottomNamesName = "BottomNames";
        public static readonly string TopValuesName = "TopValues";
        public static readonly string BottomValuesName = "BottomValues";
        public static readonly string TopRestrictName = "TopRestrict";
        public static readonly string BottomRestrictName = "BottomRestrict";
        public static readonly string ViaRestrictName = "ViaRestrict";
        public static readonly string TopKeepoutName = "TopKeepout";
        public static readonly string BottomKeepoutName = "BottomKeepout";
        public static readonly string DrillsName = "Drills";
        public static readonly string HolesName = "Holes";
        public static readonly string PadsName = "Pads";
        public static readonly string ViasName = "Vias";
        public static readonly string UnroutedName = "Unrouted";
        public static readonly string ProfileName = "Profile";
        public static readonly string UnknownName = "Unknown";

        private readonly BoardSide side = BoardSide.Unknown;
        private readonly LayerFunction function = LayerFunction.Unknown;
        private string name;
        private Color color;
        private bool isVisible = true;

        /// <summary>
        /// Constructor per defecte.
        /// </summary>
        /// 
        public Layer() {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Nom de la capa.</param>
        /// <param name="side">Cara on es troba la capa.</param>
        /// <param name="function">Functio de la capa.</param>
        /// <param name="color">Color dels elements.</param>
        /// <param name="isVisible">Indica si la capa es visible.</param>
        /// 
        public Layer(string name, BoardSide side, LayerFunction function, Color color, bool isVisible = true) {

            this.name = name;
            this.side = side;
            this.function = function;
            this.color = color;
            this.isVisible = isVisible;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte la funcio de la capa
        /// </summary>
        /// 
        public LayerFunction Function {
            get {
                return function;
            }
        }

        /// <summary>
        /// Obte el cara on es troba la capa de la capa.
        /// </summary>
        /// 
        public BoardSide Side {
            get {
                return side;
            }
        }

        /// <summary>
        /// Obte o asigna el nom de la capa.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        /// <summary>
        /// Obte o asigna el color de la capa.
        /// </summary>
        /// 
        public Color Color {
            get {
                return color;
            }
            set {
                color = value;
            }
        }

        public bool IsVisible {
            get {
                return isVisible;
            }
            set {
                isVisible = value;
            }
        }
    }
}
