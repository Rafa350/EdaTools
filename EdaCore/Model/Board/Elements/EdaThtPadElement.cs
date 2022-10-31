using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un pad THT
    /// </summary>
    /// 
    public sealed class EdaThtPadElement: EdaPadElement {

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
        private ThPadCornerShape _cornerShape = ThPadCornerShape.Round;
        private int _drillDiameter;
        private int _slot;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override int GetHashCode() =>
            HashCode.Combine(_topSize, _innerSize, _bottomSize, _cornerRatio, _cornerShape, _drillDiameter, _slot, base.GetHashCode());

        /// <summary>
        /// Crea la llista de punts del poligon del pad.
        /// </summary>
        /// <param name="layerId">La capa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private IEnumerable<EdaPoint> MakePadPoints(EdaLayerId layerId, int spacing) {

            var size = GetPadSize(layerId);

            return EdaPointFactory.CreateRectangle(
                Position,
                new EdaSize(size.Width + spacing + spacing, size.Height + spacing + spacing),
                _cornerRatio,
                _cornerShape == ThPadCornerShape.Round,
                Rotation);
        }

        /// <summary>
        /// Obte la llista de punts del poligon del forat.
        /// </summary>
        /// <returns>La llista de punts.</returns>
        /// 
        private IEnumerable<EdaPoint> MakeHolePoints() {

            if (_slot <= _drillDiameter)
                return EdaPointFactory.CreateCircle(Position, _drillDiameter / 2);

            else {
                var size = GetSmallestPadSize();
                bool h = size.Width >= size.Height;
                int offset = (_slot - _drillDiameter) / 2;
                var start = new EdaPoint(
                    Position.X - (h ? offset : 0),
                    Position.Y - (h ? 0 : offset));
                var end = new EdaPoint(
                    Position.X + (h ? offset : 0),
                    Position.Y + (h ? 0 : offset));
                // TODO: Falta fer la rotacio del forat
                return EdaPointFactory.CreateLineTrace(start, end, _drillDiameter, true);
            }
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            var padPoints = MakePadPoints(layerId, 0);
            var holePoints = MakeHolePoints();
            return new EdaPolygon(padPoints, holePoints);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            var padPoints = MakePadPoints(layerId, spacing);
            return new EdaPolygon(padPoints);
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
            new(new EdaPoint(0, 0), GetPadSize(layerId));

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
            get => _cornerShape;
            set => _cornerShape = value;
        }

        /// <summary>
        /// El diametre del forat.
        /// </summary>
        /// 
        public int DrillDiameter {
            get => _drillDiameter;
            set {
                if ((value < 0) || (value >= DrilHiLimit))
                    throw new ArgumentOutOfRangeException(nameof(DrillDiameter));

                _drillDiameter = value;
            }
        }

        /// <summary>
        /// Posicio del forat.
        /// </summary>
        /// 
        public EdaPoint DrillPosition =>
            Position;

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

        public EdaPoint SlotStartPosition {
            get {
                if (_slot < _drillDiameter)
                    return DrillPosition;
                else {
                    var size = GetSmallestPadSize();
                    bool h = size.Width >= size.Height;
                    int offset = (_slot - _drillDiameter) / 2;
                    return new EdaPoint(
                        Position.X - (h ? offset : 0),
                        Position.Y - (h ? 0 : offset));
                }
            }
        }

        public EdaPoint SlotEndPosition {
            get {
                if (_slot < _drillDiameter)
                    return DrillPosition;
                else {
                    var size = GetSmallestPadSize();
                    bool h = size.Width >= size.Height;
                    int offset = (_slot - _drillDiameter) / 2;
                    return new EdaPoint(
                        Position.X + (h ? offset : 0),
                        Position.Y + (h ? 0 : offset));
                }
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
    }
}
