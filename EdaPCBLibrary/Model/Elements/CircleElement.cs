﻿namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure.Polygons;
    using System;

    /// <summary>
    /// Clase que representa un cercle.
    /// </summary>
    public sealed class CircleElement: Element, IPosition {

        private PointInt position;
        private int radius;
        private int thickness;

        /// <summary>
        ///  Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public CircleElement():
            base() {
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <param name="thickness">Amplada de linia. Zero indica que es un disc.</param>
        /// 
        public CircleElement(PointInt position, int radius, int thickness = 0) :           
            base() {

            this.position = position;
            this.radius = radius;
            this.thickness = thickness;
        }

        /// <summary>
        /// Accepta un visitador del objecte.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            if (thickness == 0) {
                PointInt[] points = PolygonBuilder.BuildCircle(position, radius);
                return new Polygon(points);
            }
            else {
                PointInt[] outerPoints = PolygonBuilder.BuildCircle(position, radius + (thickness / 2));
                PointInt[] innerPoints = PolygonBuilder.BuildCircle(position, radius - (thickness / 2));
                return new Polygon(outerPoints, new Polygon[] { new Polygon(innerPoints) });
            }
        }

        /// <summary>
        /// Crea el poligon exterior del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            PointInt[] points = PolygonBuilder.BuildCircle(position, radius + (thickness / 2) + spacing);
            return new Polygon(points);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override RectInt GetBoundingBox(BoardSide side) {

            int r = radius + (thickness / 2);
            return new RectInt(position.X - r, position.Y - r, r + r, r + r);
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre del cercle.
        /// </summary>
        /// 
        public PointInt Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        /// <summary>
        /// Obte o asigna el radi del cercle.
        /// </summary>
        /// 
        public int Radius {
            get {
                return radius;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Radius");

                radius = value;
            }
        }

        /// <summary>
        /// Obte o asigna el diametre del cercle.
        /// </summary>
        /// 
        public int Diameter {
            get {
                return radius * 2;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Diameter");

                radius = value / 2; 
            }
        }

        /// <summary>
        /// Obte o asigna l'amplada de linia.
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
        /// Obte o asigna el indicador de cercle ple.
        /// </summary>
        /// 
        public bool Filled {
            get {
                return thickness == 0;
            }
            set {
                if (value)
                    thickness = 0; 
            }
        }
    }
}
