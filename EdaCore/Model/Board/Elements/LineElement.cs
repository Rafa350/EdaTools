﻿namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    using System;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;

    /// <summary>
    /// Clase que representa una linia.
    /// </summary>
    public class LineElement : Element, IConectable {

        public enum CapStyle {
            Round,
            Flat
        }

        private Point _startPosition;
        private Point _endPosition;
        private int _thickness;
        private CapStyle _lineCap = CapStyle.Round;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// <param name="startPosition">La posicio inicial.</param>
        /// <param name="endPosition">La posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="lineCap">Forma dels extrems de linia.</param>
        /// 
        public LineElement(LayerSet layerSet, Point startPosition, Point endPosition, int thickness, CapStyle lineCap) :
            base(layerSet) {

            if (thickness < 0)
                throw new ArgumentOutOfRangeException(nameof(thickness));

            _startPosition = startPosition;
            _endPosition = endPosition;
            _thickness = thickness;
            _lineCap = lineCap;
        }

        /// <summary>
        /// Clone l'alement.
        /// </summary>
        /// <returns>El clon de l'element.</returns>
        /// 
        public override Element Clone() {

            return new LineElement(LayerSet, _startPosition, _endPosition, _thickness, _lineCap);
        }

        /// <summary>
        /// Accepta un visitador.
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

            Point[] points = PolygonBuilder.MakeLineTrace(_startPosition, _endPosition, _thickness, LineCap == CapStyle.Round);
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

            Point[] points = PolygonBuilder.MakeLineTrace(_startPosition, _endPosition, _thickness + (spacing * 2), _lineCap == CapStyle.Round);
            return new Polygon(points);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            return new Rect(
                Math.Min(_startPosition.X, _endPosition.X) - _thickness / 2,
                Math.Min(_startPosition.Y, _endPosition.Y) - _thickness / 2,
                Math.Abs(_endPosition.X - _startPosition.X + 1) + _thickness,
                Math.Abs(_endPosition.Y - _startPosition.Y + 1) + _thickness);
        }

        /// <summary>
        /// Obte o asigna la posicio inicial.
        /// </summary>
        /// 
        public Point StartPosition { 
            get => _startPosition;
            set => _startPosition = value;
        }

        /// <summary>
        /// Obte o asigna la posicio final.
        /// </summary>
        /// 
        public Point EndPosition {
            get => _endPosition;
            set => _endPosition = value;
        }

        /// <summary>
        ///  Obte o asigna l'amplada de linia.
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
        /// Obte o asigna el tipus d'extrem de linia.
        /// </summary>
        /// 
        public CapStyle LineCap {
            get => _lineCap;
            set => _lineCap = value;
        }

        /// <summary>
        /// Obte el tipus d'element.
        /// </summary>
        /// 
        public override ElementType ElementType =>
            ElementType.Line;
    }
}

