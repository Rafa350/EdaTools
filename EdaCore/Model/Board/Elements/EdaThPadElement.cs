using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un pad throught hole
    /// </summary>
    /// 
    public sealed class EdaThPadElement: EdaPadElement {

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
        private int _slot;

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
            _drill * 37000 +
            _slot * 211;

        /// <summary>
        /// Crea la llista de punts del poligon del pad.
        /// </summary>
        /// <param name="layerId">La capa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private EdaPoints MakePadPoints(EdaLayerId layerId, int spacing) {

            EdaSize size = GetPadSize(layerId);

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
        private EdaPoints MakeHolePoints() {

            if (_slot <= _drill)
                return EdaPoints.CreateCircle(Position, _drill / 2);

            else {
                var size = GetSmallestPadSize();
                bool h = size.Width >= size.Height;
                int offset = (_slot - _drill) / 2;
                var start = new EdaPoint(
                    Position.X - (h ? offset : 0),
                    Position.Y - (h ? 0 : offset));
                var end = new EdaPoint(
                    Position.X + (h ? offset : 0),
                    Position.Y + (h ? 0 : offset));
                return EdaPoints.CreateLineTrace(start, end, _drill, true);
            }
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            int hash = GetHashCode() + (layerId.GetHashCode() * 2798761);
            EdaPolygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {

                var padPoints = MakePadPoints(layerId, 0);
                var holePoints = MakeHolePoints();
                polygon = new EdaPolygon(padPoints, new EdaPolygon(holePoints));

                PolygonCache.Save(hash, polygon);
            }

            return polygon;
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            int hash = GetHashCode() + (layerId.GetHashCode() * 47211) + spacing * 99997;
            EdaPolygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {

                var padPoints = MakePadPoints(layerId, spacing);
                polygon = new EdaPolygon(padPoints);

                PolygonCache.Save(hash, polygon);
            }

            return polygon;
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetThermalPolygon(EdaLayerId layerId, int spacing, int width) {

            EdaSize size = GetPadSize(layerId);
            int w = size.Width + spacing + spacing;
            int h = size.Height + spacing + spacing;

            var padPolygon = GetOutlinePolygon(layerId, spacing);
            var thermalPolygon = new EdaPolygon(EdaPoints.CreateCross(Position, new EdaSize(w, h), width, Rotation));

            List<EdaPolygon> childs = new List<EdaPolygon>();
            childs.AddRange(PolygonProcessor.Clip(padPolygon, thermalPolygon, PolygonProcessor.ClipOperation.Diference));
            if (childs.Count != 4)
                throw new InvalidProgramException("Thermal generada incorrectamente.");
            return new EdaPolygon(null, childs);
        }

        /// <summary>
        /// Obte el poligon del forat
        /// </summary>
        /// <returns>El poligon </returns>
        /// 
        public EdaPolygon GetDrillPolygon() {

            var holePoints = MakeHolePoints();
            return new EdaPolygon(holePoints);
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) =>
            new EdaRect(new EdaPoint(0, 0), GetPadSize(layerId));

        /// <summary>
        /// Obte el tamany del pad en funcio de la capa.
        /// </summary>
        /// <param name="layerId">La capa.</param>
        /// <returns>El valor del tamany.</returns>
        /// 
        private EdaSize GetPadSize(EdaLayerId layerId) {

            var side = layerId.Side;
            return
                side == BoardSide.Top ? TopSize :
                side == BoardSide.Inner ? InnerSize :
                BottomSize;
        }

        /// <summary>
        /// Obte el tamany mes petit del pad.
        /// </summary>
        /// <returns>El valor del tamany.</returns>
        /// 
        private EdaSize GetSmallestPadSize() {

            return new EdaSize(
                Math.Min(_topSize.Width, Math.Min(_innerSize.Width, _bottomSize.Width)),
                Math.Min(_topSize.Height, Math.Min(_innerSize.Height, _bottomSize.Height)));
        }

        /// <inheritdoc/>
        /// 
        public override bool IsOnLayer(EdaLayerId layerId) =>
            layerId.IsSignal || base.IsOnLayer(layerId) || (layerId == EdaLayerId.Platted);

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
                if ((value < 0) || (value >= DrilHiLimit))
                    throw new ArgumentOutOfRangeException(nameof(Drill));

                _drill = value;
            }
        }

        /// <summary>
        /// El maxim valor del diametre del forat
        /// </summary>
        /// 
        public int DrilHiLimit {
            get {
                EdaSize size = GetSmallestPadSize();
                return Math.Min(size.Width, size.Height);
            }
        }

        /// <summary>
        /// L'amplada del slot.
        /// </summary>
        /// 
        public int Slot {
            get => _slot;
            set {
                if ((value < 0) || (value >= SlotHiLimit))
                    throw new ArgumentOutOfRangeException(nameof(Slot));

                _slot = value;
            }
        }

        /// <summary>
        /// El maxim valor del slot
        /// </summary>
        /// 
        public int SlotHiLimit {
            get {
                EdaSize size = GetSmallestPadSize();
                return Math.Min(size.Width, size.Height);
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
