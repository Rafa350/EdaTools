﻿namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa un forat no conductor.
    /// </summary>
    public sealed class HoleElement: Element, IPosition {

        private Point position;
        private double drill;

        /// <summary>
        /// Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public HoleElement() {
        }

        /// <summary>
        /// Constructir de l'objecte.
        /// </summary>
        /// <param name="position">Pocicio del centre.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// 
        public HoleElement(Point position, double drill) { 

            if (drill <= 0)
                throw new ArgumentOutOfRangeException("drill");

            this.position = position;
            this.drill = drill;
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
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon() {

            return PolygonBuilder.BuildCircle(position, drill / 2);
        }

        /// <summary>
        /// Crea el poligon espaiat del element.
        /// </summary>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPourPolygon(double spacing) {

            return PolygonBuilder.BuildCircle(position, (drill / 2) + spacing);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox() {

            return new Rect(position.X - drill / 2, position.Y - drill / 2, drill, drill);
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre del cercle.
        /// </summary>
        /// 
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        /// <summary>
        /// Obte o asigna el diametre del forat.
        /// </summary>
        /// 
        public double Drill {
            get {
                return drill;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Drill");

                drill = value;
            }
        }
    }
}
