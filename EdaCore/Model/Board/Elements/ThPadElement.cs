using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un pad throug hole
    /// </summary>
    /// 
    public sealed class ThPadElement : PadElement {

        public enum ThPadShape {
            Square,
            Octagon,
            Circle,
            Oval
        }

        private const double _cos2250 = 0.92387953251128675612818318939679;

        private int _drcTopSizeMin = 175000;
        private int _drcTopSizeMax = 2500000;
        private Ratio _drcTopSizePercent = Ratio.P25;
        private int _drcBottomSizeMin = 175000;
        private int _drcBottomSizeMax = 2500000;
        private Ratio _drcBottomSizePercent = Ratio.P25;

        private ThPadShape _shape = ThPadShape.Circle;
        private int _topSize;
        private int _innerSize;
        private int _bottomSize;
        private int _drill;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">Nom.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Orientacio.</param>
        /// <param name="size">Tamany/diametre del pad.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// <param name="shape">Forma de la corona.</param>
        /// 
        public ThPadElement(string name, Point position, Angle rotation, int size, ThPadShape shape, int drill) :
            this(name, position, rotation, size, size, size, shape, drill) {
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">Nom.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Orientacio.</param>
        /// <param name="topSize">Tamany/diametre del pad.</param>
        /// <param name="innerSize">Tamany/diametre del pad.</param>
        /// <param name="innerSize">Tamany/diametre del pad.</param>
        /// <param name="bottom">Diametre del forat.</param>
        /// <param name="drill">Forma de la corona.</param>
        /// 
        public ThPadElement(string name, Point position, Angle rotation, int topSize, int innerSize,
            int bottomSize, ThPadShape shape, int drill) :
            base(name, position, rotation) {

            if (topSize < 0)
                throw new ArgumentOutOfRangeException(nameof(topSize));

            if (innerSize < 0)
                throw new ArgumentOutOfRangeException(nameof(innerSize));

            if (bottomSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bottomSize));

            if (drill <= 0)
                throw new ArgumentOutOfRangeException(nameof(drill));

            _topSize = topSize;
            _innerSize = innerSize;
            _bottomSize = bottomSize;
            _drill = drill;
            _shape = shape;
        }


        /// <inheritdoc/>
        /// 
        public override Element Clone() {

            return new ThPadElement(Name, Position, Rotation, _topSize, _innerSize, _bottomSize, _shape, _drill);
        }

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        public override int GetHashCode() =>
            Position.GetHashCode() +
            (_innerSize * 21032) +
            (_topSize * 78931) +
            (_bottomSize * 974) +
            (_shape.GetHashCode() * 721) +
            Rotation.GetHashCode() +
            _drill * 37000;

        /// <summary>
        /// Crea la llista de punts d'un poligon
        /// </summary>
        /// <param name="side">Cara de la placa</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private Point[] MakePoints(BoardSide side, int spacing) {

            int size = GetSize(side);
            int sizeM2 = size * 2;
            int sizeD2 = size / 2;

            int spacingM2 = spacing * 2;
            int spacingD2 = spacing / 2;

            switch (_shape) {
                case ThPadShape.Square:
                    return PolygonBuilder.MakeRectangle(
                        Position,
                        new Size(size + spacingM2, size + spacingM2),
                        spacing,
                        Rotation);

                case ThPadShape.Octagon: {
                    int s = (int)((double)sizeD2 / _cos2250);
                    return PolygonBuilder.MakeRegularPolygon(
                        8,
                        Position,
                        s + spacing,
                        Rotation + Angle.FromValue(2250));
                }

                case ThPadShape.Oval:
                    return PolygonBuilder.MakeRectangle(
                        Position,
                        new Size(sizeM2 + spacingM2, size + spacingM2),
                        sizeD2 + spacing,
                        Rotation);

                default:
                    return PolygonBuilder.MakeCircle(
                        Position,
                        sizeD2 + spacing);
            }
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            int hash = GetHashCode() + (side.GetHashCode() * 2798761);
            Polygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {

                var points = MakePoints(side, 0);
                var holePoints = PolygonBuilder.MakeCircle(Position, _drill / 2);
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

            int size = GetSize(side);

            int w = ((_shape == ThPadShape.Oval ? 2 : 1) * size) + spacing + spacing;
            int h = size + spacing + spacing;

            Polygon pour = GetOutlinePolygon(side, spacing);
            Polygon thermal = new Polygon(PolygonBuilder.MakeCross(Position, new Size(w, h), width, Rotation));

            List<Polygon> childs = new List<Polygon>();
            childs.AddRange(PolygonProcessor.Clip(pour, thermal, PolygonProcessor.ClipOperation.Diference));
            if (childs.Count != 4)
                throw new InvalidProgramException("Thermal generada incorrectamente.");
            return new Polygon(null, childs.ToArray());
        }

        /// <inheritdoc/>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            int size = GetSize(side);

            int w = ((_shape == ThPadShape.Oval ? 2 : 1) * size);
            int h = _topSize;

            return new Rect(Position.X - (w / 2), Position.Y - (h / 2), w, h);
        }

        /// <summary>
        /// Obte el tamany en funcio de la cara de la placa.
        /// </summary>
        /// <param name="side">La cara.</param>
        /// <returns>El valor del tamany.</returns>
        /// 
        private int GetSize(BoardSide side) {

            return
                side == BoardSide.Top ? TopSize :
                side == BoardSide.Inner ? InnerSize :
                BottomSize;
        }

        /// <inheritdoc/>
        /// 
        public override bool IsOnLayer(LayerId layerId) =>
            layerId.IsSignal || (layerId == LayerId.Drills);

        /// <summary>
        /// Obte o asigna la forma del pad.
        /// </summary>
        /// 
        public ThPadShape Shape {
            get => _shape;
            set => _shape = value;
        }

        /// <summary>
        /// Obte o asigna el diametre del forat.
        /// </summary>
        /// 
        public int Drill {
            get {
                return _drill;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Drill");

                _drill = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany del pad de la cara superior.
        /// </summary>
        /// 
        public int TopSize {
            get {
                if (_topSize == 0) {
                    int ring = Math.Max(_drcTopSizeMin, Math.Min(_drcTopSizeMax, _drill * _drcTopSizePercent));
                    return _drill + ring * 2;
                }
                else
                    return _topSize;
            }
            set {
                _topSize = value;
            }
        }
        /// <summary>
        /// Obte o asigna el tamany del pad de la cara inferior
        /// </summary>
        /// 
        public int BottomSize {
            get {
                if (_bottomSize == 0) {
                    int ring = Math.Max(_drcBottomSizeMin, Math.Min(_drcBottomSizeMax, _drill * _drcBottomSizePercent));
                    return _drill + ring * 2;
                }
                else
                    return _bottomSize;
            }
            set {
                _bottomSize = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany del pad de la capa interna.
        /// </summary>
        /// 
        public int InnerSize {
            get {
                if (_innerSize == 0) {
                    int ring = Math.Max(_drcTopSizeMin, Math.Min(_drcTopSizeMax, _drill * _drcTopSizePercent));
                    return _drill + ring * 2;
                }
                else
                    return _innerSize;
            }
            set {
                _innerSize = value;
            }
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType => 
            ElementType.ThPad;
    }
}
