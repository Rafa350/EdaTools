namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Windows;
    using System.Windows.Media;

    public static class PolygonBuilder {

        public static Polygon Build(CircleElement circle, Part part, double clearance) {

            Point[] points = PointsFromCircle(circle.Radius + clearance);

            Matrix m = new Matrix();
            m.Translate(circle.Position.X, circle.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
            }
            m.Transform(points);

            return new Polygon(points);
        }

        public static Polygon Build(RectangleElement rectangle, Part part, double clearance) {

            Point[] points = PointsFromRectangle(rectangle.Size);

            Matrix m = new Matrix();
            m.Translate(rectangle.Position.X, rectangle.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
            }
            m.Transform(points);

            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="via">El element.</param>
        /// <param name="part">El conmponent al que pertany.</param>
        /// <param name="clearance">Increment de tamany.</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public static Polygon Build(ViaElement via, Part part, double clearance) {

            Point[] points;

            double size = via.OuterSize;
            double radius = size / 2;

            switch (via.Shape) {
                default:
                case ViaElement.ViaShape.Circular:
                    points = PointsFromCircle(radius + clearance);
                    break;

                case ViaElement.ViaShape.Square:
                    points = PointsFromRectangle(new Size(size + (clearance * 2), size + (clearance * 2)));
                    break;

                case ViaElement.ViaShape.Octogonal:
                    points = PointsFromRegularPolygon(8, radius + clearance);
                    break;
            }

            Matrix m = new Matrix();
            m.Translate(via.Position.X, via.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
            }
            m.Transform(points);

            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="pad">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="clearance">Increment de tamany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon Build(ThPadElement pad, Part part, double clearance) {

            Point[] points;

            double size = pad.Size;
            double radius = size / 2;

            switch (pad.Shape) {
                default:
                case ThPadElement.ThPadShape.Circular:
                    points = PointsFromCircle(radius + clearance);
                    break;

                case ThPadElement.ThPadShape.Square:
                    points = PointsFromRectangle(new Size(size + (clearance * 2), size + (clearance * 2)));
                    break;

                case ThPadElement.ThPadShape.Octogonal:
                    points = PointsFromRegularPolygon(8, radius + clearance);
                    break;
            }

            Matrix m = new Matrix();
            m.Translate(pad.Position.X, pad.Position.Y);
            m.RotateAt(pad.Rotation, pad.Position.X, pad.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
            }
            m.Transform(points);

            return new Polygon(points);
        }

        public static Polygon Build(SmdPadElement pad, Part part, double clearance) {

            Point[] points = PointsFromRectangle(new Size(pad.Size.Width + (clearance * 2), pad.Size.Height + (clearance * 2)));

            Matrix m = new Matrix();
            m.Translate(pad.Position.X, pad.Position.Y);
            m.RotateAt(pad.Rotation, pad.Position.X, pad.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
            }
            m.Transform(points);

            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="region">El element</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public static Polygon Build(RegionElement region) {

            Polygon polygon = new Polygon();

            foreach (RegionElement.Segment segment in region.Segments)
                polygon.Add(segment.Position);

            return polygon;
        }

        private static Point[] PointsFromLine(Point start, Point end) {

            Point[] points = new Point[2];

            points[0] = start;
            points[1] = end;

            return points;
        }

        private static Point[] PointsFromArc(Point start, Point end, double angle) {

            if (angle == 0)
                return PointsFromLine(start, end);
            else
                return null;
        }

        /// <summary>
        /// Crea una sequencia de punts per un poligon en forma de rectangle, centrat
        /// en l'origen de coordinades.
        /// </summary>
        /// <param name="size">Tamany del rectangle.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private static Point[] PointsFromRectangle(Size size) {

            Point[] points = new Point[4];

            double w = size.Width / 2;
            double h = size.Height / 2;

            points[0] = new Point(-w, h);
            points[1] = new Point(w, h);
            points[2] = new Point(w, -h);
            points[3] = new Point(-w, -h);

            return points;
        }

        /// <summary>
        /// Crea una sequencia de punts per un poligon en forma de cercle, format
        /// per 64 segments de recta i centrat en l'origen de coordinades.
        /// </summary>
        /// <param name="radius">Radi del cercle.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private static Point[] PointsFromCircle(double radius) {

            return PointsFromRegularPolygon(32, radius);
        }

        /// <summary>
        /// Crea una sequencia de punts per un poligon regular centrat en 
        /// l'origen, on la base es una aresta.
        /// </summary>
        /// <param name="sides">Numero de arestes.</param>
        /// <param name="radius">El radi del cercle exterior.</param>
        /// <param name="m">Matriu de transformacio.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private static Point[] PointsFromRegularPolygon(int sides, double radius) {

            Point[] points = new Point[sides];

            double delta = (360.0 * Math.PI / 180.0) / sides;
            double angle = delta / 2;
            for (int i = 0; i < sides; i++) {
                points[i] = new Point(radius * Math.Cos(angle), radius * Math.Sin(angle));
                angle += delta;
            }

            return points;
        }
    }
}
