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

        private Point startPosition;
        private Point endPosition;
        private int thickness;
        private CapStyle lineCap = CapStyle.Round;

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

            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.thickness = thickness;
            this.lineCap = lineCap;
        }

        /// <summary>
        /// Clone l'alement.
        /// </summary>
        /// <returns>El clon de l'element.</returns>
        /// 
        public override Element Clone() {

            return new LineElement(LayerSet, startPosition, endPosition, thickness, lineCap);
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

            Point[] points = PolygonBuilder.MakeLineTrace(startPosition, endPosition, thickness, LineCap == CapStyle.Round);
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

            Point[] points = PolygonBuilder.MakeLineTrace(startPosition, endPosition, thickness + (spacing * 2), lineCap == CapStyle.Round);
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
                Math.Min(startPosition.X, endPosition.X) - thickness / 2,
                Math.Min(startPosition.Y, endPosition.Y) - thickness / 2,
                Math.Abs(endPosition.X - startPosition.X + 1) + thickness,
                Math.Abs(endPosition.Y - startPosition.Y + 1) + thickness);
        }

        /// <summary>
        /// Obte o asigna la posicio inicial.
        /// </summary>
        /// 
        public Point StartPosition {
            get {
                return startPosition;
            }
            set {
                startPosition = value;
            }
        }

        /// <summary>
        /// Obte o asigna la posicio final.
        /// </summary>
        /// 
        public Point EndPosition {
            get {
                return endPosition;
            }
            set {
                endPosition = value;
            }
        }

        /// <summary>
        ///  Obte o asigna l'amplada de linia.
        /// </summary>
        /// 
        public int Thickness {
            get {
                return thickness;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Thickness");

                thickness = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tipus d'extrem de linia.
        /// </summary>
        /// 
        public CapStyle LineCap {
            get {
                return lineCap;
            }
            set {
                lineCap = value;
            }
        }

        /// <summary>
        /// Obte el tipus d'element.
        /// </summary>
        /// 
        public override ElementType ElementType {
            get {
                return ElementType.Line;
            }
        }
    }
}

