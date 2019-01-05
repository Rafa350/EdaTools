namespace MikroPic.EdaTools.v1.BoardEditor.Render {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Core.Model.Board;

    public sealed class VisualLayer {

        private readonly LayerId[] layerIds;
        private readonly ElementType[] elementTypes;
        private readonly string name;
        private bool visible;
        private Color color;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom de la capa.</param>
        /// <param name="layerSet">Conjunt de capes de la placa.</param>
        /// <param name="visible">Indica si es visible.</param>
        /// <param name="color">Color</param>
        /// 
        public VisualLayer(string name, LayerId[] layerIds, ElementType[] elementTypes, bool visible, Color color) {

            this.name = name;
            this.layerIds = layerIds;
            this.elementTypes = elementTypes;
            this.visible = visible;
            this.color = color;
        }

        /// <summary>
        /// Obte el nom de la capa
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Comprova si un element es visible en aquesta capa.
        /// </summary>
        /// <param name="part">El part al que pertany l'element.</param>
        /// <param name="element">L'element a comprobar.</param>
        /// <returns>True si es visible, false en cas contrari.</returns>
        /// 
        public bool IsVisible(Part part, Element element) {

            LayerSet layerSet = ((part != null) && (part.Side == PartSide.Bottom)) ?
                part.GetLocalLayerSet(element) :
                element.LayerSet;

            // Seleccio per capa
            //
            bool layerOk = false;
            if (layerIds == null)
                layerOk = true;
            else
                foreach (var layerId in layerIds) {
                    if (layerSet.Contains(layerId)) {
                        layerOk = true;
                        break;
                    }
                }

            // Seleccio per tipus
            //
            bool typeOk = false;
            if (elementTypes == null)
                typeOk = true;
            else
                foreach (var elementType in elementTypes) {
                    if (element.ElementType == elementType) {
                        typeOk = true;
                        break;
                    }
                }

            return layerOk && typeOk;
        }

        /// <summary>
        /// Obte la llista de capes de la placa.
        /// </summary>
        /// 
        public LayerId[] LayerIds {
            get {
                return layerIds;
            }
        }

        /// <summary>
        /// Obte la llista de tipus d'elements.
        /// </summary>
        /// 
        public ElementType[] ElementTypes {
            get {
                return elementTypes;
            }
        }

        /// <summary>
        /// Obte o asigna el indicador de visibilitat.
        /// </summary>
        /// 
        public bool Visible {
            get {
                return visible;
            }
            set {
                visible = value;
            }
        }

        /// <summary>
        /// Obte o asigna el color.
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

        public double Opacity {
            get {
                return color.A / 255.0;
            }
        }
    }
}
