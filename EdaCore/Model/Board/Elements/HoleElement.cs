using System;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un forat no conductor.
    /// </summary>
    public sealed class HoleElement : Element, IPosition {

        private Point _position;
        private int _drill;

        /// <summary>
        /// Constructir de l'objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="position">Pocicio del centre.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// 
        public HoleElement(LayerSet layerSet, Point position, int drill) :
            base(layerSet) {

            if (drill <= 0)
                throw new ArgumentOutOfRangeException(nameof(drill));

            _position = position;
            _drill = drill;
        }

        /// <summary>
        /// Obte un clon de l'element.
        /// </summary>
        /// <returns>El clon de l'element.</returns>
        /// 
        public override Element Clone() {

            return new HoleElement(LayerSet, _position, _drill);
        }

        /// <summary>
        /// Accepta un visitador del objecte.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public override void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            Point[] points = PolygonBuilder.MakeCircle(_position, _drill / 2);
            return new Polygon(points);
        }

        /// <summary>
        /// Crea el poligon exterior del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            Point[] points = PolygonBuilder.MakeCircle(_position, (_drill / 2) + spacing);
            return new Polygon(points);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            return new Rect(_position.X - _drill / 2, _position.Y - _drill / 2, _drill, _drill);
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
        /// Obte o asigna el diametre del forat.
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
        /// Obte el tipus d'element.
        /// </summary>
        /// 
        public override ElementType ElementType =>
            ElementType.Hole;
    }
}

