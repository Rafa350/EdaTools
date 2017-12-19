namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System.Windows.Media;

    public enum LayerId {
        Unknown,
        TopKeepout,
        TopRestrict,
        TopNames,
        TopValues,
        TopPlace,
        TopDocument,
        TopStop,
        TopCream,
        TopGlue,
        TopPins,
        Top,
        Inner2,
        Inner3,
        Inner4,
        Inner5,
        Inner6,
        Inner7,
        Inner8,
        Inner9,
        Inner10,
        Inner11,
        Inner12,
        Inner13,
        Inner14,
        Inner15,
        Bottom,
        BottomPins,
        BottomGlue,
        BottomCream,
        BottomStop,
        BottomDocument,
        BottomPlace,
        BottomValues,
        BottomNames,
        BottomRestrict,
        BottomKeepout,

        Profile, 

        ViaRestrict,

        Vias,
        Pads,
        Holes,
        Drills,

        Unrouted,

        UserDefined1,
        UserDefined2,
        UserDefined3,
        UserDefined4,
        UserDefined5,
        UserDefined6,
        UserDefined7,
        UserDefined8,
        UserDefined9,
        UserDefined10,
        UserDefined11,
        UserDefined12,
        UserDefined13,
        UserDefined14,
        UserDefined15,
        UserDefined16
    }

    public enum LayerClass {
        Unknown,
        TopSignal,
        InnerSignal,
        BottomSignal,
        Design,
        Mechanical,
    }

    public sealed class Layer: IVisitable {

        private readonly LayerId id = LayerId.Unknown;
        private readonly LayerClass cls = LayerClass.Unknown;
        private string name;
        private Color color;
        private Layer mirror;
        private bool isVisible = true;
        private bool isMirror = false;

        /// <summary>
        /// Constructor per defecte.
        /// </summary>
        /// 
        public Layer() {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Identificador de la capa.</param>
        /// <param name="name">Nom de la capa.</param>
        /// <param name="color">Color dels elements.</param>
        /// <param name="isVisible">Indica si la capa es visible.</param>
        /// <param name="isMirror">Indica si la capa es dibuixa en mirall.</param>
        /// 
        public Layer(LayerId id, string name, Color color, bool isVisible = true, bool isMirror = false) {

            this.id = id;
            this.name = name;
            this.color = color;
            this.isVisible = isVisible;
            this.isMirror = isMirror;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Identificador de la capa.</param>
        /// <param name="name">Nom de la capa.</param>
        /// <param name="color">Color dels elements.</param>
        /// <param name="isVisible">Indica si la capa es visible.</param>
        /// <param name="mirror">Referencia a la seva capa mirall.</param>
        /// 
        public Layer(LayerId id, string name, Color color, bool isVisible, Layer mirror) {

            this.id = id;
            this.name = name;
            this.color = color;
            this.isVisible = isVisible;
            this.isMirror = false;
            this.mirror = mirror;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public LayerId Id {
            get {
                return id;
            }
        }

        public LayerClass Class {
            get {
                return cls;
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

        public bool IsMirror {
            get {
                return isMirror;
            }
            set {
                isMirror = value;
                if (isMirror)
                    mirror = null;
            }
        }

        public Layer Mirror {
            get {
                return mirror;
            }
            set {
                mirror = value;
                isMirror = false;
            }
        }
    }
}
