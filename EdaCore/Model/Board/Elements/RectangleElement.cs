using System;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un rectangle.
    /// </summary>
    /// 
    public sealed class RectangleElement : Element, IPosition, ISize, IRotation {

        private Point _position;
        private Size _size;
        private Angle _rotation;
        private Ratio _roundness;
        private int _thickness;
        private bool _filled;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="position">Posicio del centre geometric.</param>
        /// <param name="size">Amplada i alçada del rectangle.</param>
        /// <param name="roundness">Factor d'arrodoniment de les cantonades.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="thickness">Amplada de linia. Si es zero, es un rectangle ple.</param>
        /// <param name="filled">True indica si cal omplir el rectangle.</param>
        /// 
        public RectangleElement(LayerSet layerSet, Point position, Size size, Ratio roundness, Angle rotation, int thickness, bool filled) :
            base(layerSet) {

            _position = position;
            _size = size;
            _roundness = roundness;
            _rotation = rotation;
            _thickness = thickness;
            _filled = filled;
        }

        /// <inheritdoc/>
        /// 
        public override Element Clone() {

            return new RectangleElement(LayerSet, _position, _size, _roundness, _rotation, _thickness, _filled);
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
                var points = PolygonBuilder.MakeRectangle(_position, _size, Radius, _rotation);
                return new Polygon(points);
            }
            else {
                var outerSize = new Size(_size.Width + _thickness, _size.Height + _thickness);
                var outerPoints = PolygonBuilder.MakeRectangle(_position, outerSize, Radius, _rotation);

                var innerSize = new Size(_size.Width - _thickness, _size.Height - _thickness);
                var innerPoints = PolygonBuilder.MakeRectangle(_position, innerSize, Math.Max(0, Radius - _thickness), _rotation);

                return new Polygon(outerPoints, new Polygon(innerPoints));
            }
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            var outerSize = new Size(_size.Width + _thickness + spacing * 2, _size.Height + _thickness + spacing * 2);
            var points = PolygonBuilder.MakeRectangle(_position, outerSize, Radius, _rotation);
            return new Polygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            double width = _size.Width + _thickness;
            double height = _size.Height + _thickness;

            double a = _rotation.ToRadiants;

            int w = (int)(width * Math.Cos(a) + height * Math.Sin(a));
            int h = (int)(width * Math.Sin(a) + height * Math.Cos(a));

            return new Rect(_position.X - (w / 2), _position.Y - (h / 2), w, h);
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre geometric del rectangle.
        /// </summary>
        /// 
        public Point Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// Obte o asigna el tamany del rectangle.
        /// </summary>
        /// 
        public Size Size {
            get => _size;
            set => _size = value;
        }

        /// <summary>
        /// Obte o asigna l'angle de rotacio.
        /// </summary>
        /// 
        public Angle Rotation {
            get => _rotation;
            set => _rotation = value;
        }

        /// <summary>
        /// Obte o asigna el factor d'arrodoniment de les cantonades.
        /// </summary>
        /// 
        public Ratio Roundness {
            get => _roundness;
            set => _roundness = value;
        }

        /// <summary>
        /// Obte el radi de curvatura de les cantonades.
        /// </summary>
        /// 
        public int Radius =>
            (Math.Min(_size.Width, _size.Height) * _roundness) >> 1;
            
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
        /// Obte o asigna el indicador de rectangle ple. Es el mateix que Thickness = 0.
        /// </summary>
        /// 
        public bool Filled {
            get => (_thickness == 0) || _filled;
            set => _filled = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Rectangle;
    }
}
