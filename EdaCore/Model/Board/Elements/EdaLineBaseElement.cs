using System;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una linia.
    /// </summary>
    /// 
    public abstract class EdaLineBaseElement: EdaElementBase, IEdaConectable {

        private EdaPoint _startPosition;
        private EdaPoint _endPosition;
        private int _thickness;
        private EdaLineCap _lineCap = EdaLineCap.Round;

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            var points = EdaPointFactory.CreateLineTrace(_startPosition, _endPosition, _thickness, LineCap == EdaLineCap.Round);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            var points = EdaPointFactory.CreateLineTrace(_startPosition, _endPosition, _thickness + (spacing * 2), _lineCap == EdaLineCap.Round);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override int GetHashCode() =>
            HashCode.Combine(_startPosition, _endPosition, _thickness, _lineCap, base.GetHashCode());

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) {

            return new EdaRect(
                Math.Min(_startPosition.X, _endPosition.X) - _thickness / 2,
                Math.Min(_startPosition.Y, _endPosition.Y) - _thickness / 2,
                Math.Abs(_endPosition.X - _startPosition.X + 1) + _thickness,
                Math.Abs(_endPosition.Y - _startPosition.Y + 1) + _thickness);
        }

        /// <summary>
        /// Obte o asigna la posicio inicial.
        /// </summary>
        /// 
        public EdaPoint StartPosition {
            get => _startPosition;
            set => _startPosition = value;
        }

        /// <summary>
        /// Obte o asigna la posicio final.
        /// </summary>
        /// 
        public EdaPoint EndPosition {
            get => _endPosition;
            set => _endPosition = value;
        }

        /// <summary>
        ///  Obte o asigna l'amplada de linia.
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
        /// Obte o asigna el tipus d'extrem de linia.
        /// </summary>
        /// 
        public EdaLineCap LineCap {
            get => _lineCap;
            set => _lineCap = value;
        }
    }
}

