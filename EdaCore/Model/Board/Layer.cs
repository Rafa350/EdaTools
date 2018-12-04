namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Funcio de la capa.
    /// </summary>
    /// 
    public enum LayerFunction {
        Unknown,     // Desconeguda
        Signal,      // Pistes, vias, pads, th, etc
        Design,      // Silk, names, etc
        Mechanical,  // Forats, fressat, etc
        Outline      // Perfil extern
    }

    /// <summary>
    /// Clare quie representa una capa de la placa.
    /// </summary>
    /// 
    public sealed class Layer: IVisitable {

        public static readonly LayerId TopId = new LayerId("Top", BoardSide.Top);
        public static readonly LayerId Inner1Id = new LayerId("Inner1", BoardSide.Inner);
        public static readonly LayerId Inner2Id = new LayerId("Inner2", BoardSide.Inner);
        public static readonly LayerId Inner3Id = new LayerId("Inner3", BoardSide.Inner);
        public static readonly LayerId Inner4Id = new LayerId("Inner4", BoardSide.Inner);
        public static readonly LayerId Inner5Id = new LayerId("Inner5", BoardSide.Inner);
        public static readonly LayerId Inner6Id = new LayerId("Inner6", BoardSide.Inner);
        public static readonly LayerId Inner7Id = new LayerId("Inner7", BoardSide.Inner);
        public static readonly LayerId Inner8Id = new LayerId("Inner8", BoardSide.Inner);
        public static readonly LayerId Inner9Id = new LayerId("Inner9", BoardSide.Inner);
        public static readonly LayerId Inner10Id = new LayerId("Inner10", BoardSide.Inner);
        public static readonly LayerId Inner11Id = new LayerId("Inner11", BoardSide.Inner);
        public static readonly LayerId Inner12Id = new LayerId("Inner12", BoardSide.Inner);
        public static readonly LayerId Inner13Id = new LayerId("Inner13", BoardSide.Inner);
        public static readonly LayerId Inner14Id = new LayerId("Inner14", BoardSide.Inner);
        public static readonly LayerId BottomId = new LayerId("Bottom", BoardSide.Bottom);
        public static readonly LayerId TopStopId = new LayerId("Stop", BoardSide.Top);
        public static readonly LayerId BottomStopId = new LayerId("Stop", BoardSide.Bottom);
        public static readonly LayerId TopCreamId = new LayerId("Cream", BoardSide.Top);
        public static readonly LayerId BottomCreamId = new LayerId("Cream", BoardSide.Bottom);
        public static readonly LayerId TopGlueId = new LayerId("Glue", BoardSide.Top);
        public static readonly LayerId BottomGlueId = new LayerId("Glue", BoardSide.Bottom);
        public static readonly LayerId TopPlaceId = new LayerId("Place", BoardSide.Top);
        public static readonly LayerId BottomPlaceId = new LayerId("Place", BoardSide.Bottom);
        public static readonly LayerId TopDocumentId = new LayerId("Document", BoardSide.Top);
        public static readonly LayerId BottomDocumentId = new LayerId("Document", BoardSide.Bottom);
        public static readonly LayerId TopNamesId = new LayerId("Names", BoardSide.Top);
        public static readonly LayerId BottomNamesId = new LayerId("Names", BoardSide.Bottom);
        public static readonly LayerId TopValuesId = new LayerId("Values", BoardSide.Top);
        public static readonly LayerId BottomValuesId = new LayerId("Values", BoardSide.Bottom);
        public static readonly LayerId TopRestrictId = new LayerId("Restrict", BoardSide.Top);
        public static readonly LayerId BottomRestrictId = new LayerId("Restrict", BoardSide.Bottom);
        public static readonly LayerId ViaRestrictId = new LayerId("ViaRestrict");
        public static readonly LayerId TopKeepoutId = new LayerId("Keepout", BoardSide.Top);
        public static readonly LayerId BottomKeepoutId = new LayerId("Keepout", BoardSide.Bottom);
        public static readonly LayerId DrillsId = new LayerId("Drills");
        public static readonly LayerId HolesId = new LayerId("Holes");
        public static readonly LayerId MillingId = new LayerId("Milling");
        public static readonly LayerId PadsId = new LayerId("Pads");
        public static readonly LayerId ViasId = new LayerId("Vias");
        public static readonly LayerId UnroutedId = new LayerId("Unrouted");
        public static readonly LayerId ProfileId = new LayerId("Profile");
        public static readonly LayerId UnknownId = new LayerId("Unknown");

        private readonly LayerFunction function = LayerFunction.Unknown;
        private readonly LayerId id;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Identificador de la capa.</param>
        /// <param name="function">Functio de la capa.</param>
        /// 
        public Layer(LayerId id, LayerFunction function) {

            this.id = id;
            this.function = function;
        }

        /// <summary>
        /// Obte un clon de l'objecte.
        /// </summary>
        /// <returns>El clon obtingut.</returns>
        /// 
        public Layer Clone() {

            return new Layer(id, function);
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte l'identificador de la capa.
        /// </summary>
        /// 
        public LayerId Id {
            get {
                return id;
            }
        }

        /// <summary>
        /// Obte el nom.
        /// </summary>
        /// 
        public string Name {
            get {
                return id.FullName;
            }
        }

        /// <summary>
        /// Obte la cara
        /// </summary>
        /// 
        public BoardSide Side {
            get {
                return id.Side;
            }
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
    }
}
