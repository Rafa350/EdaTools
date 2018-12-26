namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System;
    using System.Collections.Generic;


    public enum PartSide {
        Top,
        Bottom
    }

    public sealed partial class Part: IPosition, IRotation, IName, IVisitable {

        private readonly string name;
        private Point position;
        private Angle rotation;
        private PartSide side = PartSide.Top;
        private readonly Component component;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="component">El component associat.</param>
        /// <param name="name">El nom.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Angle de rotacio</param>
        /// <param name="side">Indica la cara de la placa.</param>
        /// 
        public Part(Component component, string name, Point position, Angle rotation, PartSide side = PartSide.Top) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (component == null)
                throw new ArgumentNullException("component");

            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.side = side;
            this.component = component;
        }

        /// <summary>
        /// Clona l'objecte.
        /// </summary>
        /// <param name="name">El nom del clon.</param>
        /// <param name="component">El component a asignar.</param>
        /// <returns>El clon de l'objecte obtingut.</returns>
        /// 
        public Part Clone(string name, Component component) {

            Part part = new Part(component, name, position, rotation, side);
            foreach (var attribute in attributes.Values)
                part.AddAttribute(attribute.Clone());

            return part;
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
        /// Converteix a string
        /// </summary>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public override string ToString() {

            return String.Format("Part: '{0}'", name);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public Rect GetBoundingBox(BoardSide side) {

            if (HasElements) {

                int left = int.MaxValue;
                int top = int.MaxValue;
                int right = int.MinValue;
                int bottom = int.MinValue;

                foreach (var element in Elements) {
                    Rect r = element.GetBoundingBox(side);
                    if (left > r.Left)
                        left = r.Left;
                    if (top > r.Top)
                        top = r.Top;
                    if (right < r.Right)
                        right = r.Right;
                    if (bottom < r.Bottom)
                        bottom = r.Bottom;
                }

                return new Rect(left, top, right - left + 1, top - bottom + 1);
            }
            else
                return new Rect();
        }

        /// <summary>
        /// Obte un pad pel seu nom.
        /// </summary>
        /// <param name="name">El nom del pad.</param>
        /// <param name="throwOnError">True si dispara una execpcio si no el troba.</param>
        /// <returns>El pad. Null si no el troba.</returns>
        /// 
        public PadElement GetPad(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            PadElement pad = component.GetPad(name, false);
            if (pad != null)
                return pad;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el pad '{0}' en el part '{1}'.", name, this.name));

            else
                return null;
        }

        /// <summary>
        /// Obte o asigna el nom.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Obte el component
        /// </summary>
        /// 
        public Component Component {
            get {
                return component;
            }
        }

        /// <summary>
        /// Obte o asigna la posicio.
        /// </summary>
        /// 
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'angle de rotacio.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
            }
        }

        /// <summary>
        /// Obte la cara en la que es monta el component.
        /// </summary>
        /// 
        public PartSide Side {
            get {
                return side;
            }
            set {
                side = value;
            }
        }

        /// <summary>
        /// Indica si conte elements
        /// </summary>
        /// 
        public bool HasElements {
            get {
                return component.HasElements;
            }
        }

        /// <summary>
        /// Enumera els elements.
        /// </summary>
        /// 
        public IEnumerable<Element> Elements {
            get {
                return component.Elements;
            }
        }

        /// <summary>
        /// Indica si conte pads.
        /// </summary>
        /// 
        public bool HasPads {
            get {
                return component.HasPads;
            }
        }

        /// <summary>
        /// Enumera els pads
        /// </summary>
        /// 
        public IEnumerable<PadElement> Pads {
            get {
                return component.Pads;
            }
        }
    }
}
