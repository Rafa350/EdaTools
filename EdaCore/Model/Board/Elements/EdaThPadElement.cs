using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un pad throught hole
    /// </summary>
    /// 
    public sealed class EdaThPadElement : EdaPadElement {

        public enum ThPadCornerShape {
            Round,
            Flat
        }

        private readonly int _drcTopSizeMin = 175000;
        private readonly int _drcTopSizeMax = 2500000;
        private readonly EdaRatio _drcTopSizePercent = EdaRatio.P25;
        private readonly int _drcBottomSizeMin = 175000;
        private readonly int _drcBottomSizeMax = 2500000;
        private readonly EdaRatio _drcBottomSizePercent = EdaRatio.P25;

        private EdaSize _topSize;
        private EdaSize _innerSize;
        private EdaSize _bottomSize;
        private EdaRatio _cornerRatio = EdaRatio.Zero;
        private ThPadCornerShape _cornerType = ThPadCornerShape.Round;
        private int _drill;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el valor hash del objecte.
        /// </summary>
        /// <returns>El valor hash.</returns>
        /// 
        public override int GetHashCode() =>
            Position.GetHashCode() +
            _innerSize.GetHashCode() +
            _topSize.GetHashCode() +
            _bottomSize.GetHashCode() +
            _cornerRatio.GetHashCode() +
            _cornerType.GetHashCode() +
            Rotation.GetHashCode() +
            _drill * 37000;

        /// <summary>
        /// Crea la llista de punts d'un poligon
        /// </summary>
        /// <param name="side">Cara de la placa</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private EdaPoints MakePoints(BoardSide side, int spacing) {

            EdaSize size = GetSize(side);

            return EdaPoints.CreateRectangle(
                Position,
                new EdaSize(size.Width + spacing + spacing, size.Height + spacing + spacing),
                _cornerRatio,
                _cornerType == ThPadCornerShape.Round,
                Rotation);
        }

        /// <summary>
        /// Obte la llista de punts del poligon del forat.
        /// </summary>
        /// <returns>La llista de punts.</returns>
        /// 
        private EdaPoints MakeHolePonts() {

            return EdaPoints.CreateCircle(Position, _drill / 2);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            int hash = GetHashCode() + (side.GetHashCode() * 2798761);
            Polygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {
                
                var points = MakePoints(side, 0);
                var holePoints = MakeHolePonts();
                polygon = new Polygon(points, new Polygon(holePoints));
                
                PolygonCache.Save(hash, polygon);
            }

            return polygon;
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            int hash = GetHashCode() + (side.GetHashCode() * 47211) + spacing * 99997;
            Polygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {

                var points = MakePoints(side, spacing);
                polygon = new Polygon(points);

                PolygonCache.Save(hash, polygon);
            }

            return polygon;
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetThermalPolygon(BoardSide side, int spacing, int width) {

            EdaSize size = GetSize(side);
            int w = size.Width + spacing + spacing;
            int h = size.Height + spacing + spacing;

            Polygon pour = GetOutlinePolygon(side, spacing);
            Polygon thermal = new Polygon(EdaPoints.CreateCross(Position, new EdaSize(w, h), width, Rotation));

            List<Polygon> childs = new List<Polygon>();
            childs.AddRange(PolygonProcessor.Clip(pour, thermal, PolygonProcessor.ClipOperation.Diference));
            if (childs.Count != 4)
                throw new InvalidProgramException("Thermal generada incorrectamente.");
            return new Polygon(null, childs.ToArray());
        }

        /// <summary>
        /// Obte el poligon del forat
        /// </summary>
        /// <returns>El poligon </returns>
        /// 
        public Polygon GetDrillPolygon() {

            var points = EdaPoints.CreateCircle(Position, _drill / 2);
            return new Polygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(BoardSide side) =>
            new EdaRect(new EdaPoint(0, 0), GetSize(side));

        /// <summary>
        /// Obte el tamany en funcio de la cara de la placa.
        /// </summary>
        /// <param name="side">La cara.</param>
        /// <returns>El valor del tamany.</returns>
        /// 
        private EdaSize GetSize(BoardSide side) {

            return
                side == BoardSide.Top ? TopSize :
                side == BoardSide.Inner ? InnerSize :
                BottomSize;
        }

        /// <inheritdoc/>
        /// 
        public override bool IsOnLayer(EdaLayerId layerId) =>
            layerId.IsSignal || base.IsOnLayer(layerId);

        /// <summary>
        /// L'arrodoniment o planitut de les cantonades.
        /// </summary>
        /// 
        public EdaRatio CornerRatio {
            get => _cornerRatio;
            set => _cornerRatio = value;
        }

        /// <summary>
        /// Forma de les cantonades.
        /// </summary>
        /// 
        public ThPadCornerShape CornerShape {
            get => _cornerType;
            set => _cornerType = value;
        }

        /// <summary>
        /// El diametre del forat.
        /// </summary>
        /// 
        public int Drill {
            get => _drill;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Drill));

                _drill = value;
            }
        }

        /// <summary>
        /// El tamany del pad de la cara superior.
        /// </summary>
        /// 
        public EdaSize TopSize {
            get => _topSize;
            set => _topSize = value;
        }

        /// <summary>
        /// El tamany del pad de la cara inferior
        /// </summary>
        /// 
        public EdaSize BottomSize {
            get => _bottomSize;
            set => _bottomSize = value;
        }

        /// <summary>
        /// El tamany del pad de la capa interna.
        /// </summary>
        /// 
        public EdaSize InnerSize {
            get => _innerSize;
            set => _innerSize = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.ThPad;
    }
}
