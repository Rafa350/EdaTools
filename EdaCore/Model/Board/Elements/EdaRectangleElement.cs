using System;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un rectangle.
    /// </summary>
    /// 
    public sealed class EdaRectangleElement: EdaElement {

        private EdaPoint _position;
        private EdaSize _size;
        private EdaAngle _rotation;
        private EdaRatio _cornerRatio;
        private int _thickness;
        private bool _filled;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            var outerSize = new EdaSize(_size.Width + _thickness, _size.Height + _thickness);
            var outerPoints = EdaPointFactory.CreateRectangle(_position, outerSize, _cornerRatio, true, _rotation);
            if (_filled)
                return new EdaPolygon(outerPoints);          
            else {
                var innerSize = new EdaSize(_size.Width - _thickness, _size.Height - _thickness);
                var innerCornerRatio = EdaRatio.FromPercent((double)(CornerSize - Thickness) / (Math.Min(innerSize.Width, innerSize.Height) / 2));
                var innerPoints = EdaPointFactory.CreateRectangle(_position, innerSize, innerCornerRatio, true, _rotation);
                return new EdaPolygon(outerPoints, new EdaPolygon(innerPoints));
            }
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            var outerSize = new EdaSize(_size.Width + _thickness + spacing * 2, _size.Height + _thickness + spacing * 2);
            var points = EdaPointFactory.CreateRectangle(_position, outerSize, _cornerRatio, true, _rotation);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) {

            double width = _size.Width + _thickness;
            double height = _size.Height + _thickness;

            double a = _rotation.AsRadiants;

            int w = (int)(width * Math.Cos(a) + height * Math.Sin(a));
            int h = (int)(width * Math.Sin(a) + height * Math.Cos(a));

            return new EdaRect(_position.X - (w / 2), _position.Y - (h / 2), w, h);
        }

        /// <summary>
        ///  La posicio del centre geometric del rectangle.
        /// </summary>
        /// 
        public EdaPoint Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// El tamany del rectangle.
        /// </summary>
        /// 
        public EdaSize Size {
            get => _size;
            set => _size = value;
        }

        /// <summary>
        /// L'angle de rotacio.
        /// </summary>
        /// 
        public EdaAngle Rotation {
            get => _rotation;
            set => _rotation = value;
        }

        /// <summary>
        /// El factor d'arrodoniment de les cantonades.
        /// </summary>
        /// 
        public EdaRatio CornerRatio {
            get => _cornerRatio;
            set => _cornerRatio = value;
        }

        /// <summary>
        /// El radi de curvatura de les cantonades.
        /// </summary>
        /// 
        public int CornerSize =>
            (Math.Min(_size.Width, _size.Height) * _cornerRatio) / 2;

        /// <summary>
        /// Amplada de linia.
        /// </summary>
        /// 
        public int Thickness {
            get => _thickness;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Thickness));
                _thickness = value;
            }
        }

        /// <summary>
        /// Indicador de rectangle ple.
        /// </summary>
        /// 
        public bool Filled {
            get => _filled;
            set => _filled = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Rectangle;
    }
}
