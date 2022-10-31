using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una via
    /// </summary>
    /// 
    public sealed class EdaViaElement: EdaElement, IEdaConectable {

        private const double _cos2250 = 0.92387953251128675612818318939679;

        private readonly int _drcOuterSizeMin = 125000;
        private readonly int _drcOuterSizeMax = 2500000;
        private readonly EdaRatio _drcOuterSizePercent = EdaRatio.P25;
        private readonly int _drcInnerSizeMin = 125000;
        private readonly int _drcInnerSizeMax = 2500000;
        private readonly EdaRatio _drcInnerSizePercent = EdaRatio.P25;

        private EdaPoint _position;
        private int _drillDiameter;
        private int _outerSize = 0;
        private int _innerSize = 0;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override int GetHashCode() =>
            HashCode.Combine(_position, _drillDiameter, _outerSize, _innerSize, base.GetHashCode());

        /// <summary>
        /// Obte la llista de puns pels poligons
        /// </summary>
        /// <param name="layerId">La capa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private IEnumerable<EdaPoint> MakeViaPoints(EdaLayerId layerId, int spacing) {

            int size = layerId.Side == BoardSide.Inner ? InnerSize : OuterSize;
            return EdaPointFactory.CreateCircle(_position, (size / 2) + spacing);
        }

        /// <summary>
        /// Obte la llista de punts pel poligon del forat.
        /// </summary>
        /// <returns>La llista de puints.</returns>
        /// 
        private IEnumerable<EdaPoint> MakeHolePoints() {

            return EdaPointFactory.CreateCircle(_position, _drillDiameter / 2);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            var viaPoints = MakeViaPoints(layerId, 0);
            var holePoints = MakeHolePoints();
            return new EdaPolygon(viaPoints, holePoints);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            var viaPoints = MakeViaPoints(layerId, spacing);
            return new EdaPolygon(viaPoints);
        }

        /// <summary>
        /// Obte el poligon del forat
        /// </summary>
        /// <returns>El poligon </returns>
        /// 
        public EdaPolygon GetDrillPolygon() {

            var points = MakeHolePoints();
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) {

            int size = layerId.Side == BoardSide.Inner ? InnerSize : OuterSize;
            return new EdaRect(_position.X - (size / 2), _position.Y - (size / 2), size, size);
        }

        /// <inheritdoc/>
        /// 
        public override bool IsOnLayer(EdaLayerId layerId) =>
            layerId.IsSignal || (layerId == EdaLayerId.Vias) || (layerId == EdaLayerId.Platted);

        /// <summary>
        /// La posicio.
        /// </summary>
        /// 
        public EdaPoint Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// El diametre del forat
        /// </summary>
        /// 
        public int DrillDiameter {
            get => _drillDiameter;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(DrillDiameter));

                _drillDiameter = value;
            }
        }

        /// <summary>
        /// Tamany de la corona de les capes externes.
        /// </summary>
        /// 
        public int OuterSize {
            get {
                if (_outerSize == 0) {
                    int ring = Math.Max(_drcOuterSizeMin, Math.Min(_drcOuterSizeMax, _drillDiameter * _drcOuterSizePercent));
                    return _drillDiameter + ring * 2;
                }
                else
                    return _outerSize;
            }
            set => _outerSize = value;
        }

        /// <summary>
        /// Tamany de la corona de les capes internes.
        /// </summary>
        /// 
        public int InnerSize {
            get {
                if (_innerSize == 0) {
                    int ring = Math.Max(_drcInnerSizeMin, Math.Min(_drcInnerSizeMax, _drillDiameter * _drcInnerSizePercent));
                    return _drillDiameter + ring * 2;
                }
                else
                    return _innerSize;
            }
            set => _innerSize = value;
        }
    }
}
