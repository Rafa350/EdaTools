namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

    public sealed partial class Part : IPosition, IRotation, IName, IBoardVisitable {

        private readonly string _name;
        private Point _position;
        private Angle _rotation;
        private bool _flip;
        private readonly Component _component;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="component">El component associat.</param>
        /// <param name="name">El nom.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Angle de rotacio</param>
        /// <param name="flip">Indica la cara de la placa.</param>
        /// 
        public Part(Component component, string name, Point position, Angle rotation, bool flip = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (component == null)
                throw new ArgumentNullException(nameof(component));

            _name = name;
            _position = position;
            _rotation = rotation;
            _flip = flip;
            _component = component;
        }

        /// <summary>
        /// Clona l'objecte.
        /// </summary>
        /// <param name="name">El nom del clon.</param>
        /// <param name="component">El component a asignar.</param>
        /// <returns>El clon de l'objecte obtingut.</returns>
        /// 
        public Part Clone(string name, Component component) {

            Part part = new Part(component, name, _position, _rotation, _flip);
            foreach (var attribute in attributes.Values)
                part.AddAttribute(attribute.Clone());

            return part;
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public override string ToString() {

            return String.Format("Part: '{0}'", _name);
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
        /// <param name="throwOnError">True si dispara una execepcio si no el troba.</param>
        /// <returns>El pad. Null si no el troba.</returns>
        /// 
        public PadElement GetPad(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            PadElement pad = _component.GetPad(name, false);
            if (pad != null)
                return pad;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el pad '{0}' en el part '{1}'.", name, this._name));

            else
                return null;
        }

        /// <summary>
        /// Obte o asigna el nom.
        /// </summary>
        /// 
        public string Name =>_name;

        /// <summary>
        /// Obte el component
        /// </summary>
        /// 
        public Component Component => _component;

        /// <summary>
        /// Obte o asigna la posicio.
        /// </summary>
        /// 
        public Point Position {
            get {
                return _position;
            }
            set {
                _position = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'angle de rotacio.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return _rotation;
            }
            set {
                _rotation = value;
            }
        }

        /// <summary>
        /// Obte o asigna si el component esta girat
        /// </summary>
        /// 
        public bool Flip {
            get {
                return _flip;
            }
            set {
                _flip = value;
            }
        }

        /// <summary>
        /// Indica si el component esta girat.
        /// </summary>
        /// 
        public bool IsFlipped => _flip;

        /// <summary>
        /// Indica si conte elements
        /// </summary>
        /// 
        public bool HasElements => _component.HasElements;

        /// <summary>
        /// Enumera els elements.
        /// </summary>
        /// 
        public IEnumerable<Element> Elements => _component.Elements;

        /// <summary>
        /// Indica si conte pads.
        /// </summary>
        /// 
        public bool HasPads => _component.HasPads;

        /// <summary>
        /// Enumera els pads
        /// </summary>
        /// 
        public IEnumerable<PadElement> Pads => _component.Pads;
    }
}
