using System;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un cercle.
    /// </summary>
    /// 
    public sealed class CircleElement : Element, IPosition {

        private Point _position;
        private int _radius;
        private int _thickness;
        private bool _filled;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="filled">True si cal omplir el cercle.</param>
        /// 
        public CircleElement(LayerSet layerSet, Point position, int radius, int thickness, bool filled) :
            base(layerSet) {

            _position = position;
            _radius = radius;
            _thickness = thickness;
            _filled = filled;
        }

        /// <inheritdoc/>
        /// 
        public override Element Clone() {

            return new CircleElement(LayerSet, _position, _radius, _thickness, _filled);
        }

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            if (Filled) {
                var points = PolygonBuilder.MakeCircle(_position, _radius);
                return new Polygon(points);
            }
            else {
                var outerPoints = PolygonBuilder.MakeCircle(_position, _radius + (_thickness / 2));
                var innerPoints = PolygonBuilder.MakeCircle(_position, _radius - (_thickness / 2));
                return new Polygon(outerPoints, new Polygon(innerPoints));
            }
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            var points = PolygonBuilder.MakeCircle(_position, _radius + (_thickness / 2) + spacing);
            return new Polygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            int r = _radius + (_thickness / 2);
            return new Rect(_position.X - r, _position.Y - r, r + r, r + r);
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
        /// Obte o asigna el radi del cercle.
        /// </summary>
        /// 
        public int Radius {
            get {
                return _radius;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Radius");

                _radius = value;
            }
        }

        /// <summary>
        /// Obte o asigna el diametre del cercle.
        /// </summary>
        /// 
        public int Diameter {
            get {
                return _radius * 2;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Diameter");

                _radius = value / 2;
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
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Thickness");

                _thickness = value;
            }
        }

        /// <summary>
        /// Obte o asigna el indicador de cercle ple.
        /// </summary>
        /// 
        public bool Filled { 
            get => (_thickness == 0) || _filled;
            set => _filled = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Circle;
    }
}
