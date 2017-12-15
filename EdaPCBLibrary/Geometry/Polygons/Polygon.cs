namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using ClipperLib;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public sealed class Polygon {

        private List<IntPoint> points = new List<IntPoint>();

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="circle">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="inflate">El increment de tamany del poligon.</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public static Polygon FromElement(CircleElement circle, Part part, double inflate = 0) {

            Matrix m = new Matrix();
            m.Translate(circle.Position.X, circle.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotate, part.Position.X, part.Position.Y);
            }

            return FromCircle(circle.Diameter + (inflate * 2), m);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="rectangle">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="inflate">El increment de tamany del poligon.</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public static Polygon FromElement(RectangleElement rectangle, Part part, double inflate = 0) {

            Matrix m = new Matrix();
            m.Translate(rectangle.Position.X, rectangle.Position.Y);
            m.RotateAt(rectangle.Rotate, rectangle.Position.X, rectangle.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotate, part.Position.X, part.Position.Y);
            }

            return FromRectangle(new Size(rectangle.Size.Width + (inflate * 2), rectangle.Size.Height + (inflate * 2)), m);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="pad">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="inflate">El increment de tamany del poligon.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon FromElement(ThPadElement pad, Part part, double inflate = 0) {

            Matrix m = new Matrix();
            m.Translate(pad.Position.X, pad.Position.Y);
            m.RotateAt(pad.Rotate, pad.Position.X, pad.Position.Y);
            if (pad.Shape == ThPadElement.ThPadShape.Octogonal)
                m.RotateAt(22.5, pad.Position.X, pad.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotate, part.Position.X, part.Position.Y);
            }

            Polygon p = new Polygon();

            switch (pad.Shape) {
                case ThPadElement.ThPadShape.Circular:
                    p = FromCircle(pad.Size + (inflate + 2), m);
                    break;

                case ThPadElement.ThPadShape.Square:
                    p = FromRectangle(new Size(pad.Size + (inflate * 2), pad.Size + (inflate * 2)), m);
                    break;

                case ThPadElement.ThPadShape.Octogonal:
                    p = FromPolygon(8, pad.Size + (inflate * 2), m);
                    break;
            }

            return p;
        }

        /// <summary>
        /// Crea un poligon de 24 cares per simular un cercle, centrat en l'origen.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="m">Matriu de transformacio.</param>
        /// <returns>El piligon.</returns>
        /// 
        private static Polygon FromCircle(double diameter, Matrix m) {

            return FromPolygon(24, diameter, m);
        }

        /// <summary>
        /// Crea un poligon en forma de rectangle centrat en l'origen.
        /// </summary>
        /// <param name="size">Tamany del rectangle.</param>
        /// <param name="m">Matriu de transformacio.</param>
        /// <returns>El poligon.</returns>
        /// 
        private static Polygon FromRectangle(Size size, Matrix m) {

            Polygon p = new Polygon();

            double w = size.Width / 2;
            double h = size.Height / 2;
            p.AddPoint(m.Transform(new Point(-w, -h)));
            p.AddPoint(m.Transform(new Point(w, -h)));
            p.AddPoint(m.Transform(new Point(w, h)));
            p.AddPoint(m.Transform(new Point(-w, h)));

            return p;
        }

        /// <summary>
        /// Crea un poligon regular.
        /// </summary>
        /// <param name="sides">Numero de arestes.</param>
        /// <param name="diameter">Diametre extern.</param>
        /// <param name="m">Matriu de transformacio.</param>
        /// <returns>El poligon.</returns>
        /// 
        private static Polygon FromPolygon(int sides, double diameter, Matrix m) {

            Polygon p = new Polygon();

            double radius = diameter / 2;
            double angle = 0;
            double delta = 360.0 / sides * Math.PI / 180.0;
            while (sides-- > 0) {
                p.AddPoint(m.Transform(new Point(radius * Math.Cos(angle), radius * Math.Sin(angle))));
                angle += delta;
            }

            return p;
        }

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

            foreach (var point in points)
                AddPoint(point);
        }

        /// <summary>
        /// Constructor intern. Crea el poligon a partir d'una coleccio de punts.
        /// </summary>
        /// <param name="points">Coleccio de punts.</param>
        /// 
        private Polygon(IEnumerable<IntPoint> points) {

            this.points.AddRange(points);
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="x">Coordinada X del punt.</param>
        /// <param name="y">Coordinada Y del punt.</param>
        /// 
        public void AddPoint(double x, double y) {

            points.Add(new IntPoint(x * 10000, y * 10000));
        }

        /// <summary>
        /// Afegeix un punt al poligon.
        /// </summary>
        /// <param name="point">El punt a afeigir.</param>
        /// 
        public void AddPoint(Point point) {

            points.Add(new IntPoint(point.X * 10000, point.Y * 10000));
        }

        public IEnumerable<Polygon> Clip(Polygon clipPolygon) {

            Clipper cp = new Clipper();
            cp.AddPath(points, PolyType.ptSubject, true);
            cp.AddPath(clipPolygon.points, PolyType.ptClip, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(ClipType.ctDifference, solution);

            List<Polygon> polygons = new List<Polygon>();
            foreach (var poly in solution)
                polygons.Add(new Polygon(poly));
            return polygons;
        }

        public int NumPoints {
            get {
                return points.Count;
            }
        }

        public IEnumerable<Point> Points {
            get {
                List<Point> p = new List<Point>(points.Count);
                foreach (var point in points)
                    p.Add(new Point((double)point.X / 10000.0, (double)point.Y / 10000.0));
                return p;
            }
        }
    }
}
