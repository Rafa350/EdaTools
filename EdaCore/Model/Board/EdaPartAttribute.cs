using System;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Core.Model.Common;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa un atribut
    /// </summary>
    /// 
    public sealed class EdaPartAttribute: IEdaVisitable<IEdaBoardVisitor> {

        private readonly string _name;
        private string _value;
        private EdaPoint _position;
        private EdaAngle _rotation;
        private int _height;
        private HorizontalTextAlign _horizontalAlign = HorizontalTextAlign.Center;
        private VerticalTextAlign _verticalAlign = VerticalTextAlign.Middle;
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
        public EdaPartAttribute(string name, string value, bool isVisible = false) {

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

        public EdaPartAttribute(string name, EdaPoint position, EdaAngle rotation, int height,
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

        public void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Indica si l'atribut es visible o no.
        /// </summary>
        /// 
        public bool IsVisible {
            get => _isVisible;
            set => _isVisible = value;
        }

        /// <summary>
        /// Obte el nom del atribut.
        /// </summary>
        /// 
        public string Name =>
            _name;

        /// <summary>
        /// La posicio del atribut.
        /// </summary>
        /// 
        public EdaPoint Position {
            get => _position;
            set {
                _position = value;
                _usePosition = true;
            }
        }

        /// <summary>
        /// L'angle de rotacio de l'atribut.
        /// </summary>
        /// 
        public EdaAngle Rotation {
            get => _rotation;
            set {
                _rotation = value;
                _useRotation = true;
            }
        }

        /// <summary>
        /// L'aliniacio del atribut.
        /// </summary>
        /// 
        public HorizontalTextAlign HorizontalAlign {
            get => _horizontalAlign;
            set {
                _horizontalAlign = value;
                _useAlign = true;
            }
        }

        /// <summary>
        /// L'aliniacio del atribut.
        /// </summary>
        /// 
        public VerticalTextAlign VerticalAlign {
            get => _verticalAlign;
            set {
                _verticalAlign = value;
                _useAlign = true;
            }
        }

        /// <summary>
        /// L'alçada del atribut.
        /// </summary>
        /// 
        public int Height {
            get => _height;
            set {
                _height = value;
                _useHeight = true;
            }
        }

        /// <summary>
        /// El valor de l'atribut.
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