using System;

using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una via
    /// </summary>
    /// 
    public sealed class ViaElement : EdaElement, IEdaPosition, IEdaConectable {

        public enum ViaShape {
            Square,
            Octagon,
            Circle
        }

        private const double _cos2250 = 0.92387953251128675612818318939679;

        private int _drcOuterSizeMin = 125000;
        private int _drcOuterSizeMax = 2500000;
        private EdaRatio _drcOuterSizePercent = EdaRatio.P25;
        private int _drcInnerSizeMin = 125000;
        private int _drcInnerSizeMax = 2500000;
        private EdaRatio _drcInnerSizePercent = EdaRatio.P25;

        private EdaPoint _position;
        private int _drill;
        private int _outerSize = 0;
        private int _innerSize = 0;
        private ViaShape _shape = ViaShape.Circle;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el hash del objecte.
        /// </summary>
        /// <returns>El hash.</returns>
        /// 
        public override int GetHashCode() =>
            (_position.GetHashCode() * 17) +
            (_outerSize * 31) +
            (_innerSize * 111) +
            (_drill * 13) +
            (_shape.GetHashCode() * 23);

        /// <summary>
        /// Calcula la llista de puns pels poligons
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private EdaPoint[] MakePoints(BoardSide side, int spacing) {

            int size = side == BoardSide.Inner ? InnerSize : OuterSize;
            int sizeM2 = size * 2;
            int sizeD2 = size / 2;

            int spacingM2 = spacing * 2;
            int spacingD2 = spacing / 2;

            ViaShape shape = side == BoardSide.Inner ? ViaShape.Circle : this._shape;

            switch (shape) {
                case ViaShape.Square:
                    return PolygonBuilder.MakeRectangle(
                        _position,
                        new EdaSize(size + spacingM2, size + spacingM2),
                        0,
                        EdaAngle.FromValue(0));

                case ViaShape.Octagon:
                    return PolygonBuilder.MakeRegularPolygon(
                        8,
                        _position,
                        (int)((double)sizeD2 / _cos2250) + spacing,
                        EdaAngle.FromValue(2250));

                default:
                    return PolygonBuilder.MakeCircle(
                        _position,
                        sizeD2 + spacing);
            }
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            int hash = GetHashCode() * side.GetHashCode() * 273;
            Polygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {

                var points = MakePoints(side, 0);
                var holePoints = PolygonBuilder.MakeCircle(_position, _drill / 2);
                polygon = new Polygon(points, new Polygon(holePoints));

                PolygonCache.Save(hash, polygon);
            }

            return polygon;
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            int hash = GetHashCode() + (side.GetHashCode() * 11327) + (spacing * 131);
            Polygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {

                var points = MakePoints(side, spacing);
                polygon = new Polygon(points);

                PolygonCache.Save(hash, polygon);
            }

            return polygon;
        }

        /// <summary>
        /// Obte el poligon del forat
        /// </summary>
        /// <returns>El poligon </returns>
        /// 
        public Polygon GetDrillPolygon() {

            var points = PolygonBuilder.MakeCircle(Position, _drill / 2);
            return new Polygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            int size = side == BoardSide.Inner ? InnerSize : OuterSize;
            return new Rect(_position.X - (size / 2), _position.Y - (size / 2), size, size);
        }

        /// <inheritdoc/>
        /// 
        public override bool IsOnLayer(EdaLayerId layerId) =>
            layerId.IsSignal || (layerId == EdaLayerId.Vias) || (layerId == EdaLayerId.Drills);

        /// <summary>
        ///  Obte o asigna la posicio del centre del cercle.
        /// </summary>
        /// 
        public EdaPoint Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// Obte o asigna el diametre del forat
        /// </summary>
        /// 
        public int Drill {
            get => _drill;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Drill");

                _drill = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany de la corona de les capes externes.
        /// </summary>
        /// 
        public int OuterSize {
            get {
                if (_outerSize == 0) {
                    int ring = Math.Max(_drcOuterSizeMin, Math.Min(_drcOuterSizeMax, _drill * _drcOuterSizePercent));
                    return _drill + ring * 2;
                }
                else
                    return _outerSize;
            }
            set => _outerSize = value;
        }

        /// <summary>
        /// Obte o asigna el tamany de la corona de les capes internes.
        /// </summary>
        /// 
        public int InnerSize {
            get {
                if (_innerSize == 0) {
                    int ring = Math.Max(_drcInnerSizeMin, Math.Min(_drcInnerSizeMax, _drill * _drcInnerSizePercent));
                    return _drill + ring * 2;
                }
                else
                    return _innerSize;
            }
            set => _innerSize = value;
        }

        /// <summary>
        /// Obte o asigna la forma exterior. Les interiors sempre son circulars.
        /// </summary>
        /// 
        public ViaShape Shape {
            get => _shape;
            set => _shape = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Via;
    }
}
