using System;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un text.
    /// </summary>
    /// 
    public sealed class TextElement : Element, ILayer, IPosition, IRotation {

        private LayerId _layerId;
        private Point _position;
        private Angle _rotation;
        private int _height;
        private int _thickness;
        private HorizontalTextAlign _horizontalAlign;
        private VerticalTextAlign _verticalAlign;
        private string _value;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="layerId">La capa.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="height">Alçada de lletra.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="horizontalAlign">Aliniacio horitzontal.</param>
        /// <param name="verticalAlign">Aliniacio vertical.</param>
        /// <param name="value">El valor del text.</param>
        /// 
        public TextElement(LayerId layerId, Point position, Angle rotation, int height, int thickness,
            HorizontalTextAlign horizontalAlign = HorizontalTextAlign.Left,
            VerticalTextAlign verticalAlign = VerticalTextAlign.Bottom, string value = null) :
            base() {

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            if (thickness <= 0)
                throw new ArgumentOutOfRangeException(nameof(thickness));

            _layerId = layerId;
            _position = position;
            _rotation = rotation;
            _height = height;
            _thickness = thickness;
            _horizontalAlign = horizontalAlign;
            _verticalAlign = verticalAlign;
            _value = value;
        }

        /// <inheritdoc/>
        /// 
        public override Element Clone() {

            return new TextElement(_layerId, _position, _rotation, _height, _thickness,
                _horizontalAlign, _verticalAlign, _value);
        }

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            return null;
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            return null;
        }

        /// <inheritdoc/>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// 
        public override bool IsOnLayer(LayerId layerId) =>
            _layerId == layerId;

        /// <summary>
        /// Obte o asigna la capa.
        /// </summary>
        /// 
        public LayerId LayerId {
            get => _layerId;
            set => _layerId = value;
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre del cercle.
        /// </summary>
        /// 
        public Point Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// Obte o asigna l'angle de rotacio.
        /// </summary>
        /// 
        public Angle Rotation {
            get => _rotation;
            set => _rotation = value;
        }

        /// <summary>
        /// Obte o asigna l'alçada de lletra.
        /// </summary>
        /// 
        public int Height {
            get {
                return _height;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Height");

                _height = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'amplada de linia.
        /// </summary>
        /// 
        public int Thickness {
            get {
                return _thickness;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Thickness");

                _thickness = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'aliniacio horitzontal del text.
        /// </summary>
        /// 
        public HorizontalTextAlign HorizontalAlign {
            get => _horizontalAlign;
            set => _horizontalAlign = value;
        }

        /// <summary>
        /// Obte o asigna l'aliniacio vertical del text.
        /// </summary>
        /// 
        public VerticalTextAlign VerticalAlign {
            get => _verticalAlign;
            set => _verticalAlign = value;
        }

        /// <summary>
        /// Obte o asigna el valor del text.
        /// </summary>
        /// 
        public string Value {
            get => _value;
            set => _value = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Text;
    }
}
