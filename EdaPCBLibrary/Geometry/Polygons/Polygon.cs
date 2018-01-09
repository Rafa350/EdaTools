﻿namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public sealed class Polygon: IEnumerable<Point> {

        private List<Point> points;
        private List<Polygon> holes;

        /// <summary>
        /// Constructor. Crea un poligon buit.
        /// </summary>
        /// 
        public Polygon() {
        }

        /// <summary>
        /// Constructor. Crea un poligon a partir d'una coleccio de punts.
        /// </summary>
        /// <param name="points">Coleccio de punts.</param>
        /// 
        public Polygon(IEnumerable<Point> points) {

            if (points == null)
                throw new ArgumentNullException("points");

            InternalAddPoints(points);
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="point">El punt a afeigir.</param>
        /// 
        public void Add(Point point) {

            if (point == null)
                throw new ArgumentNullException("point");

            InternalAddPoint(point);
        }

        /// <summary>
        /// Afegeix ua serie de punts al poligon.
        /// </summary>
        /// <param name="points">Els punta a afeigir.</param>
        /// 
        public void Add(IEnumerable<Point> points) {

            if (points == null)
                throw new ArgumentNullException("points");

            InternalAddPoints(points);
        }

        /// <summary>
        /// Afegeig un forat al poligon.
        /// </summary>
        /// <param name="polygon">El forat a afeigir.</param>
        /// 
        public void AddHole(Polygon polygon) {

            if (polygon == null)
                throw new ArgumentNullException("hole");

            InternalAddHole(polygon);
        }

        /// <summary>
        /// Clona el poligon. (Copia en profunditat.)
        /// </summary>
        /// <returns>El nou poligon.</returns>
        /// 
        public Polygon Clone() {

            Polygon polygon = new Polygon(points);
            if (holes != null)
                foreach (Polygon hole in holes)
                    polygon.AddHole(hole.Clone());

            return polygon;
        }

        public void Transform(Matrix m) {

            for (int i = 0; i < points.Count; i++)
                points[i] = m.Transform(points[i]);
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="point">El punt a afeigir.</param>
        /// 
        private void InternalAddPoint(Point point) {

            if (points == null)
                points = new List<Point>();

            points.Add(point);
        }

        /// <summary>
        /// Afegeix una serie de punts al poligon.
        /// </summary>
        /// <param name="points">Els punts a afeigir.</param>
        /// 
        private void InternalAddPoints(IEnumerable<Point> points) {

            if (this.points == null)
                this.points = new List<Point>(points);
            else
                this.points.AddRange(points);
        }

        /// <summary>
        /// Afegeig un forat al poligon.
        /// </summary>
        /// <param name="hole">El forat a afeigir.</param>
        /// 
        private void InternalAddHole(Polygon hole) {

            if (holes == null)
                holes = new List<Polygon>();

            holes.Add(hole);
        }

        /// <summary>
        /// Afegeix una serie de forats al poligon.
        /// </summary>
        /// <param name="holes">Els forats a afeigir.</param>
        /// 
        private void InternalAddHole(IEnumerable<Polygon> holes) {

            if (this.holes == null)
                this.holes = new List<Polygon>(holes);
            else
                this.holes.AddRange(holes);
        }


        /// <summary>
        /// Obte el bounding-box del poligon.
        /// </summary>
        /// 
        public Rect BoundingBox {
            get {
                double minX = Double.MaxValue;
                double minY = Double.MaxValue;
                double maxX = Double.MinValue;
                double maxY = Double.MinValue;

                foreach (Point point in points) {

                    if (point.X < minX)
                        minX = point.X;
                    if (point.Y < minY)
                        minY = point.Y;

                    if (point.X > maxX)
                        maxX = point.X;
                    if (point.Y > maxY)
                        maxY = point.Y;
                }

                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
        }

        /// <summary>
        /// Obte el numero de puns en el poligon
        /// </summary>
        /// 
        public int Count {
            get {
                return points.Count;
            }
        }

        /// <summary>
        /// Indica si el poligon te forats.
        /// </summary>
        /// 
        public bool HasHoles {
            get {
                return holes != null;
            }
        }

        /// <summary>
        /// Enumera la llista de forats.
        /// </summary>
        /// 
        public IEnumerable<Polygon> Holes {
            get {
                return holes;
            }
        }

        /// <summary>
        /// Retorna el enumerador del poligon. 
        /// </summary>
        /// <returns>El enumerador.</returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator() {

            return points.GetEnumerator();
        }

        /// <summary>
        /// Retorna el enumerador del poligon. 
        /// </summary>
        /// <returns>El enumerador.</returns>
        /// 
        public IEnumerator<Point> GetEnumerator() {

            return points.GetEnumerator();
        }
    }
}
