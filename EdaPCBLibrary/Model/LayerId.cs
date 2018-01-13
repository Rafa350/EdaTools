namespace MikroPic.EdaTools.v1.Pcb.Model {

    public struct LayerId {

        private enum LayerIdentifier {
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
            UserDefined
        }

        private LayerIdentifier id;
        private string name;

        public static readonly LayerId Unknown = new LayerId(LayerIdentifier.Unknown);
        public static readonly LayerId Top = new LayerId(LayerIdentifier.Top);
        public static readonly LayerId TopPlace = new LayerId(LayerIdentifier.TopPlace);
        public static readonly LayerId TopStop = new LayerId(LayerIdentifier.TopStop);
        public static readonly LayerId TopNames = new LayerId(LayerIdentifier.TopNames);
        public static readonly LayerId TopValues = new LayerId(LayerIdentifier.TopValues);
        public static readonly LayerId TopDocument = new LayerId(LayerIdentifier.TopDocument);
        public static readonly LayerId TopCream = new LayerId(LayerIdentifier.TopCream);
        public static readonly LayerId TopGlue = new LayerId(LayerIdentifier.TopGlue);
        public static readonly LayerId TopRestrict = new LayerId(LayerIdentifier.TopRestrict);
        public static readonly LayerId TopKeepout = new LayerId(LayerIdentifier.TopKeepout);
        public static readonly LayerId Bottom = new LayerId(LayerIdentifier.Bottom);
        public static readonly LayerId BottomPlace = new LayerId(LayerIdentifier.BottomPlace);
        public static readonly LayerId BottomStop = new LayerId(LayerIdentifier.BottomStop);
        public static readonly LayerId BottomNames = new LayerId(LayerIdentifier.BottomNames);
        public static readonly LayerId BottomValues = new LayerId(LayerIdentifier.BottomValues);
        public static readonly LayerId BottomDocument = new LayerId(LayerIdentifier.BottomDocument);
        public static readonly LayerId BottomCream = new LayerId(LayerIdentifier.BottomCream);
        public static readonly LayerId BottomGlue = new LayerId(LayerIdentifier.BottomGlue);
        public static readonly LayerId BottomRestrict = new LayerId(LayerIdentifier.BottomRestrict);
        public static readonly LayerId BottomKeepout = new LayerId(LayerIdentifier.BottomKeepout);
        public static readonly LayerId Vias = new LayerId(LayerIdentifier.Vias);
        public static readonly LayerId ViaRestrict = new LayerId(LayerIdentifier.ViaRestrict);
        public static readonly LayerId Pads = new LayerId(LayerIdentifier.Pads);
        public static readonly LayerId Holes = new LayerId(LayerIdentifier.Holes);
        public static readonly LayerId Drills = new LayerId(LayerIdentifier.Drills);
        public static readonly LayerId Profile = new LayerId(LayerIdentifier.Profile);
        public static readonly LayerId Unrouted = new LayerId(LayerIdentifier.Unrouted);

        /// <summary>
        /// Constructor privat.
        /// </summary>
        /// <param name="id">Identificador.</param>
        /// 
        private LayerId(LayerIdentifier id) {

            this.id = id;
            name = null;
        }

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// <param name="name">Nom identificatiu.</param>
        /// 
        public LayerId(string name) {

            id = LayerIdentifier.UserDefined;
            this.name = name;
        }

        public override int GetHashCode() {

            return id.GetHashCode() ^ (name != null ? name.GetHashCode() : 0xF5689);
        }

        public override string ToString() {

            return name == null ? id.ToString() : name;
        }

        public override bool Equals(object obj) {

            if ((obj != null) && (obj is LayerId))
                return id == ((LayerId)obj).id &&
                       name == ((LayerId)obj).name;
            else
                return false;
        }

        public static bool operator == (LayerId a, LayerId b) {

            return (a.id == b.id) && (a.name == b.name);
        }

        public static bool operator !=(LayerId a, LayerId b) {

            return (a.id != b.id) || (a.name != b.name);
        }

        public string Name {
            get {
                return name;
            }
        }
    }
}
