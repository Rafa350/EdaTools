using System;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Core.Model.Common;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa un atribut
    /// </summary>
    /// 
    public sealed class PartAttribute : IVisitable<IBoardVisitor> {

        private readonly string _name;
        private string _value;
        private Point _position;
        private Angle _rotation;
        private int _height;
        private HorizontalTextAlign _horizontalAlign;
        private VerticalTextAlign _verticalAlign;
        private bool _usePosition;
        private bool _useRotation;
        private bool _useHeight;
        private bool _useAlign;
        private bool _isVisible;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">Nom del atribut</param>
        /// <param name="value">Valor del atribut.</param>
        /// <param name="isVisible">Indica si es visible.</param>
        /// 
        public PartAttribute(string name, string value, bool isVisible = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
            _value = value;
            _isVisible = isVisible;
            _horizontalAlign = HorizontalTextAlign.Left;
            _verticalAlign = VerticalTextAlign.Bottom;
            _usePosition = false;
            _useRotation = false;
            _useHeight = false;
            _useAlign = false;
        }

        public PartAttribute(string name, Point position, Angle rotation, int height,
            HorizontalTextAlign horizontalAlign, VerticalTextAlign verticalAlign, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
            _value = value;
            _isVisible = true;

            _position = position;
            _usePosition = true;

            _rotation = rotation;
            _useRotation = true;

            _height = height;
            _useHeight = true;

            _horizontalAlign = horizontalAlign;
            _verticalAlign = verticalAlign;
            _useAlign = true;
        }

        public PartAttribute Clone() {

            var attribute = new PartAttribute(_name, _value, _isVisible);
            attribute._position = _position;
            attribute._usePosition = _usePosition;
            attribute._rotation = _rotation;
            attribute._useRotation = _useRotation;
            attribute._height = _height;
            attribute._useHeight = _useHeight;
            attribute._horizontalAlign = _horizontalAlign;
            attribute._verticalAlign = _verticalAlign;
            attribute._useAlign = _useAlign;

            return attribute;
        }

        public void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Indica si l'atribut es visible o no.
        /// </summary>
        /// 
        public bool IsVisible {
            get =>_isVisible;
            set => _isVisible = value;
        }

        /// <summary>
        /// Obte el nom del atribut.
        /// </summary>
        /// 
        public string Name =>
            _name;

        /// <summary>
        /// Obte la posicio del atribut.
        /// </summary>
        /// 
        public Point Position {
            get {
                return _position;
            }
            set {
                _position = value;
                _usePosition = true;
            }
        }

        /// <summary>
        /// Obte l'angle de rotacio de l'atribut.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return _rotation;
            }
            set {
                _rotation = value;
                _useRotation = true;
            }
        }

        /// <summary>
        /// Obte l'aliniacio del atribut.
        /// </summary>
        /// 
        public HorizontalTextAlign HorizontalAlign {
            get {
                return _horizontalAlign;
            }
            set {
                _horizontalAlign = value;
                _useAlign = true;
            }
        }

        /// <summary>
        /// Obte l'aliniacio del atribut.
        /// </summary>
        /// 
        public VerticalTextAlign VerticalAlign {
            get {
                return _verticalAlign;
            }
            set {
                _verticalAlign = value;
                _useAlign = true;
            }
        }

        /// <summary>
        /// Obte l'alçada del atribut.
        /// </summary>
        /// 
        public int Height {
            get {
                return _height;
            }
            set {
                _height = value;
                _useHeight = true;
            }
        }

        /// <summary>
        /// Obte o asigna el valor de l'atribut.
        /// </summary>
        /// 
        public string Value {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        /// Indica si conte informacio de posicio
        /// </summary>
        /// 
        public bool UsePosition =>
            _usePosition;

        /// <summary>
        /// Indica si conte informacio de rotacio.
        /// </summary>
        /// 
        public bool UseRotation =>
            _useRotation;

        /// <summary>
        /// Indica si conte informacio d'alçada
        /// </summary>
        /// 
        public bool UseHeight =>
            _useHeight;

        /// <summary>
        /// Indica si conte informacio d'aliniacio.
        /// </summary>
        /// 
        public bool UseAlign =>
            _useAlign;
    }
}