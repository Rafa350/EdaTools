namespace MikroPic.EdaTools.v1.Model {

    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Model.Elements;

    public enum LayerId {
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

        ViaRestrict,

        Vias,
        Pads,
        Holes,

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

    public sealed class Layer: IVisitable {

        private LayerId id;
        private string name;
        private Color color;
        private Layer mirror;
        private bool isVisible = true;
        private bool isMirror = false;
        private List<ElementBase> elements;
        private static Dictionary<ElementBase, Layer> revElements;

        /// <summary>
        /// Constructor per defecte.
        /// </summary>
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
        public Layer(LayerId id, string name, Color color, bool isVisible, Layer mirror) {

            this.id = id;
            this.name = name;
            this.color = color;
            this.isVisible = isVisible;
            this.isMirror = false;
            this.mirror = mirror;
        }

        public void AddElement(ElementBase element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if ((elements != null) && elements.Contains(element))
                throw new InvalidOperationException(
                    String.Format("El elemento ya pertenece a la capa '{0}'.", name));

            if (elements == null)
                elements = new List<ElementBase>();
            elements.Add(element);

            if (revElements == null)
                revElements = new Dictionary<ElementBase, Layer>();
            revElements.Add(element, this);
        }

        public void RemoveElement(ElementBase element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if ((elements == null) || !elements.Contains(element))
                throw new InvalidOperationException(
                    String.Format("El elemento no pertenece a la capa '{0}'.", name));

            elements.Remove(element);
            if (elements.Count == 0)
                elements = null;

            revElements.Remove(element);
            if (revElements.Count == 0)
                revElements = null;
        }

        internal static Layer LayerOf(ElementBase element) {
            
            if ((revElements != null) && revElements.ContainsKey(element))
                return revElements[element];
            else
                return null;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public LayerId Id {
            get {
                return id;
            }
        }

        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

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

        public IEnumerable<ElementBase> Elements {
            get {
                return elements;
            }
        }
    }
}
