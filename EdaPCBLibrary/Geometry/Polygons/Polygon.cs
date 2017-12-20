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
        /// <returns>El poligon generat.</returns>
        /// 
        public static Polygon FromElement(CircleElement circle, Part part) {

            Matrix m = new Matrix();
            m.Translate(circle.Position.X, circle.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotate, part.Position.X, part.Position.Y);
            }

            return FromCircle(circle.Diameter, m);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="rectangle">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public static Polygon FromElement(RectangleElement rectangle, Part part) {

            Matrix m = new Matrix();
            m.Translate(rectangle.Position.X, rectangle.Position.Y);
            m.RotateAt(rectangle.Angle, rectangle.Position.X, rectangle.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotate, part.Position.X, part.Position.Y);
            }

            return FromRectangle(new Size(rectangle.Size.Width, rectangle.Size.Height), m);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="via">El element.</param>
        /// <param name="part">El conmponent al que pertany.</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public static Polygon FromElement(ViaElement via, Part part) {

            Matrix m = new Matrix();
            m.Translate(via.Position.X, via.Position.Y);
            if (via.Shape == ViaElement.ViaShape.Octogonal)
                m.RotateAt(22.5, via.Position.X, via.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotate, part.Position.X, part.Position.Y);
            }

            double size = via.OuterSize;
            switch (via.Shape) {
                default:
                case ViaElement.ViaShape.Circular:
                    return FromCircle(size, m);

                case ViaElement.ViaShape.Square:
                    return FromRectangle(new Size(size, size), m);

                case ViaElement.ViaShape.Octogonal:
                    return FromPolygon(8, size, m);
            }
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="pad">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon FromElement(ThPadElement pad, Part part) {

            Matrix m = new Matrix();
            m.Translate(pad.Position.X, pad.Position.Y);
            m.RotateAt(pad.Angle, pad.Position.X, pad.Position.Y);
            if (pad.Shape == ThPadElement.ThPadShape.Octogonal)
                m.RotateAt(22.5, pad.Position.X, pad.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotate, part.Position.X, part.Position.Y);
            }

            switch (pad.Shape) {
                default:
                case ThPadElement.ThPadShape.Circular:
                    return FromCircle(pad.Size, m);

                case ThPadElement.ThPadShape.Square:
                    return FromRectangle(new Size(pad.Size, pad.Size), m);

                case ThPadElement.ThPadShape.Octogonal:
                    return FromPolygon(8, pad.Size, m);
            }
        }

        public static Polygon FromElement(SmdPadElement pad, Part part) {

            Polygon polygon = new Polygon();

            return polygon;
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="region">El element</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public static Polygon FromElement(RegionElement region) {

            Polygon polygon = new Polygon();

            foreach (RegionElement.Segment segment in region.Segments) 
                polygon.AddPoint(segment.Vertex);

            return polygon;
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

            Polygon polygon = new Polygon();

            double w = size.Width / 2;
            double h = size.Height / 2;
            polygon.AddPoint(m.Transform(new Point(-w, -h)));
            polygon.AddPoint(m.Transform(new Point(w, -h)));
            polygon.AddPoint(m.Transform(new Point(w, h)));
            polygon.AddPoint(m.Transform(new Point(-w, h)));

            return polygon;
        }

        /// <summary>
        /// Crea un poligon regular centrat el l'origen.
        /// </summary>
        /// <param name="sides">Numero de arestes.</param>
        /// <param name="diameter">Diametre extern.</param>
        /// <param name="m">Matriu de transformacio.</param>
        /// <returns>El poligon.</returns>
        /// 
        private static Polygon FromPolygon(int sides, double diameter, Matrix m) {

            Polygon polygon = new Polygon();

            double radius = diameter / 2;
            double angle = 0;
            double delta = 360.0 / sides * Math.PI / 180.0;
            while (sides-- > 0) {
                polygon.AddPoint(m.Transform(new Point(radius * Math.Cos(angle), radius * Math.Sin(angle))));
                angle += delta;
            }

            return polygon;
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

        public Polygon Offset(double delta) {

            ClipperOffset co = new ClipperOffset();
            co.AddPath(points, JoinType.jtRound, EndType.etClosedPolygon);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            co.Execute(ref solution, delta * 10000);

            return new Polygon(solution[0]);
        }

        public IList<Polygon> Clip(IEnumerable<Polygon> polygons) {

            Clipper cp = new Clipper();

            cp.AddPath(points, PolyType.ptSubject, true);
            foreach (Polygon polygon in polygons)
                cp.AddPath(polygon.points, PolyType.ptClip, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(ClipType.ctDifference, solution);

            List<Polygon> result = new List<Polygon>();
            foreach (var poly in solution)
                result.Add(new Polygon(poly));
            return result;
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
