namespace EdaBoardViewer.Render {

    using Avalonia.Media;

    using MikroPic.EdaTools.v1.Core.Model.Board;

    public enum VisualMode {
        Element,
        Outline,
        Drill
    }

    public sealed class VisualLayer {

        private readonly EdaLayerId[] _layerIds;
        private readonly ElementType[] _elementTypes;
        private readonly string _name;
        private readonly VisualMode _visualMode;
        private readonly Color _color;
        private bool _visible;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom de la capa.</param>
        /// <param name="layerIds">Identificadors de les capes a mostrar.</param>
        /// <param name="elementTypes">Tipus d'elements a mostrar.</param>
        /// <param name="visible">Indica si es visible.</param>
        /// <param name="visualMode">Modus de visualitzacio.</param>
        /// <param name="color">Color.</param>
        /// 
        public VisualLayer(string name, EdaLayerId[] layerIds, ElementType[] elementTypes, bool visible, VisualMode visualMode, Color color) {

            _name = name;
            _layerIds = layerIds;
            _elementTypes = elementTypes;
            _visible = visible;
            _visualMode = visualMode;
            _color = color;
        }

        /// <summary>
        /// Obte el nom de la capa
        /// </summary>
        /// 
        public string Name =>
            _name;

        /// <summary>
        /// Comprova si un element es visible en aquesta capa.
        /// </summary>
        /// <param name="part">El part al que pertany l'element.</param>
        /// <param name="element">L'element a comprobar.</param>
        /// <returns>True si es visible, false en cas contrari.</returns>
        /// 
        public bool IsVisible(EdaPart part, EdaElement element) {

            // Seleccio per capa
            //
            bool layerOk = false;
            if (_layerIds == null)
                layerOk = true;
            else
                foreach (var layerId in _layerIds) {
                    if (element.IsOnLayer(((part != null) && part.IsFlipped) ? layerId.Flip() : layerId)) {
                        layerOk = true;
                        break;
                    }
                }

            // Seleccio per tipus
            //
            bool typeOk = false;
            if (_elementTypes == null)
                typeOk = true;
            else
                foreach (var elementType in _elementTypes) {
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
        public EdaLayerId[] LayerIds =>
            _layerIds;

        /// <summary>
        /// Obte la llista de tipus d'elements.
        /// </summary>
        /// 
        public ElementType[] ElementTypes =>
            _elementTypes;

        /// <summary>
        /// Obte el modus de visualitzacio.
        /// </summary>
        /// 
        public VisualMode VisualMode =>
            _visualMode;

        /// <summary>
        /// Obte o asigna el indicador de visibilitat.
        /// </summary>
        /// 
        public bool Visible {
            get {
                return _visible;
            }
            set {
                _visible = value;
            }
        }

        /// <summary>
        /// Obte color.
        /// </summary>
        /// 
        public Color Color =>
            Color.FromRgb(_color.R, _color.G, _color.B);

        /// <summary>
        /// Obte la opacitat.
        /// </summary>
        /// 
        public double Opacity =>
            _color.A / 255.0;
    }
}
