using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un pad superficial
    /// </summary>
    /// 
    public sealed class SmdPadElement : PadElement {

        private Size _size;
        private Ratio _roundness;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom del pad.</param>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="size">Tamany</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="roundness">Percentatge d'arrodoniment de les cantonades.</param>
        /// 
        public SmdPadElement(string name, LayerSet layerSet, Point position, Size size, Angle rotation, Ratio roundness) :
            base(name, layerSet, position, rotation) {

            _size = size;
            _roundness = roundness;
        }

        /// <inheritdoc/>
        /// 
        public override Element Clone() {

            return new SmdPadElement(Name, LayerSet, Position, _size, Rotation, _roundness);
        }

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        public override int GetHashCode() =>
            Position.GetHashCode() +
            Size.GetHashCode() +
            (Rotation.GetHashCode() * 73429) +
            _roundness.GetHashCode();

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            int hash = GetHashCode() + side.GetHashCode() * 981;
            Polygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {
                
                Point[] points = PolygonBuilder.MakeRectangle(Position, Size, Radius, Rotation);
                polygon = new Polygon(points);

                PolygonCache.Save(hash, polygon);
            }
            return polygon;
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            int hash = GetHashCode() + (side.GetHashCode() * 71) + (spacing * 27009);
            Polygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {

                Point[] points = PolygonBuilder.MakeRectangle(
                    Position,
                    new Size(
                        _size.Width + spacing + spacing,
                        _size.Height + spacing + spacing),
                    Radius + spacing,
                    Rotation);
                polygon = new Polygon(points);

                PolygonCache.Save(hash, polygon);
            }

            return polygon;
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetThermalPolygon(BoardSide side, int spacing, int width) {

            Polygon pour = GetOutlinePolygon(side, spacing);
            Polygon thermal = new Polygon(
                PolygonBuilder.MakeCross(
                    Position,
                    new Size(
                        _size.Width + spacing + spacing,
                        _size.Height + spacing + spacing),
                    width,
                    Rotation));

            List<Polygon> childs = new List<Polygon>();
            childs.AddRange(PolygonProcessor.Clip(pour, thermal, PolygonProcessor.ClipOperation.Diference));
            //if (childs.Count != 4)
            //    throw new InvalidProgramException("Thermal generada incorrectamente.");
            return new Polygon(null, childs.ToArray());
        }

        /// <inheritdoc/>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            double a = Rotation.ToRadiants;

            int w = (int)(_size.Width * Math.Cos(a) + _size.Height * Math.Sin(a));
            int h = (int)(_size.Width * Math.Sin(a) + _size.Height * Math.Cos(a));

            return new Rect(Position.X - (w / 2), Position.Y - (h / 2), w, h);
        }

        /// <summary>
        /// El tamany del pad.
        /// </summary>
        /// 
        public Size Size {
            get => _size;
            set => _size = value;
        }

        /// <summary>
        /// El factor d'arrodoniment de les cantonades del pad.
        /// </summary>
        /// 
        public Ratio Roundness { 
            get => _roundness;
            set => _roundness = value;
        }

        /// <summary>
        /// Radi de curvatura de les cantonades.
        /// </summary>
        /// 
        public int Radius =>
            (Math.Min(_size.Width, _size.Height) * _roundness) / 2;

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType => 
            ElementType.SmdPad;
    }
}
