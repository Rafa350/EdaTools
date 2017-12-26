namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Windows;
    using System.Windows.Media;

    public static class PolygonBuilder {

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="line">L'element</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="clearance">Espai de separacio.</param>
        /// <returns>El poligon creat.</returns>
        /// 
        public static Polygon Build(LineElement line, Part part, double clearance) {

            double length =
                Math.Sqrt(
                    Math.Pow(line.EndPosition.X - line.StartPosition.X, 2) +
                    Math.Pow(line.EndPosition.Y - line.StartPosition.Y, 2));
            Point[] points = PointsFromLine(length, line.Thickness + (clearance * 2));

            // Realitza la transformacio dels punts, a la posicio i 
            // orientacio finals.
            //
            Matrix m = new Matrix();
            m.Translate(line.StartPosition.X, line.StartPosition.Y);
            double angle = Math.Atan2(line.EndPosition.Y - line.StartPosition.Y , line.EndPosition.X - line.StartPosition.X) * 180.0 / Math.PI;
            m.RotateAt(angle, line.StartPosition.X, line.StartPosition.Y);
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
        /// <param name="circle">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="clearance">Espai de separacio.</param>
        /// <returns>El poligon creat.</returns>
        /// 
        public static Polygon Build(CircleElement circle, Part part, double clearance) {

            // Crea els punts per un cercle centrat el l'origen.
            //
            Point[] points = PointsFromCircle(circle.Radius + clearance);

            // Realitza la transformacio dels punts, a la posicio i 
            // orientacio finals.
            //
            Matrix m = new Matrix();
            m.Translate(circle.Position.X, circle.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
            }
            m.Transform(points);

            // Crea el poligon amb la llista de punts.
            //
            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="rectangle">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="clearance">Espai de separacio.</param>
        /// <returns>El poligon creat.</returns>
        /// 
        public static Polygon Build(RectangleElement rectangle, Part part, double clearance) {

            // Crea els punts d'un rectangle centrat el l'origen.
            //
            Point[] points = PointsFromRectangle(rectangle.Size);

            // Realitza la transformacio dels punts, a la posicio i 
            // orientacio finals.
            //
            Matrix m = new Matrix();
            m.Translate(rectangle.Position.X, rectangle.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
            }
            m.Transform(points);

            // Crea el poligon amb la llista de punts.
            //
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

                case ThPadElement.ThPadShape.Square: {
                        Rect rect = new Rect(0, 0, pad.Size, pad.Size);
                        if (clearance == 0)
                            points = PointsFromRectangle(rect.Size);
                        else {
                            rect.Inflate(clearance, clearance);
                            points = PointsFromRoundRectangle(rect.Size, clearance);
                        }
                    }
                    break;

                case ThPadElement.ThPadShape.Octogonal:
                    points = PointsFromRegularPolygon(8, radius + clearance);
                    break;
            }

            // Realitza la transformacio dels punts, a la posicio i 
            // orientacio finals.
            //
            Matrix m = new Matrix();
            m.Translate(pad.Position.X, pad.Position.Y);
            m.RotateAt(pad.Rotation, pad.Position.X, pad.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
            }
            m.Transform(points);

            // Crea el poligon amb la llista de punts.
            //
            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="pad">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="clearance">increment de tamany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon Build(SmdPadElement pad, Part part, double clearance) {

            Point[] points;

            double radius = Math.Min(pad.Size.Width, pad.Size.Height) * pad.Roundnes / 2;
            Rect rect = new Rect(0, 0, pad.Size.Width, pad.Size.Height);

            if ((radius == 0) && (clearance == 0))
                points = PointsFromRectangle(rect.Size);
            else {
                rect.Inflate(clearance, clearance);
                points = PointsFromRoundRectangle(rect.Size, clearance + radius);
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

        /// <summary>
        /// Genera una sequencia de puns per un poligon d'una traç amb origen 0,0,
        /// i extrems de linia arrodonits. 
        /// </summary>
        /// <param name="length">Longitut de la linia.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <returns>La sequencia de punts.</returns>
        /// 
        private static Point[] PointsFromLine(double length, double thickness) {

            double radius = thickness / 2;

            Point[] q0Points = PointsFromQuadrant(new Point(length, 0), radius, 0);
            Point[] q1Points = PointsFromQuadrant(new Point(0, 0), radius, 1);
            Point[] q2Points = PointsFromQuadrant(new Point(0, 0), radius, 2);
            Point[] q3Points = PointsFromQuadrant(new Point(length, 0), radius, 3);

            Point[] points = new Point[4 * 9];
            q0Points.CopyTo(points, 0);
            q1Points.CopyTo(points, 9);
            q2Points.CopyTo(points, 18);
            q3Points.CopyTo(points, 27);

            return points;
        }

        /// <summary>
        /// Genera una sequencia de punts per un poligon en forma de rectangle, centrat
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
        /// Crea una sequencia de punts per un rectangle arrodonit.
        /// </summary>
        /// <param name="size">Tamany del rectangle.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <returns>La kkista de punts.</returns>
        /// 
        private static Point[] PointsFromRoundRectangle(Size size, double radius) {

            double dx = size.Width / 2;
            double dy = size.Height / 2;
            double dxc = dx - radius;
            double dyc = dy - radius;

            Point[] q0Points = PointsFromQuadrant(new Point(dxc, dyc), radius, 0);
            Point[] q1Points = PointsFromQuadrant(new Point(-dxc, dyc), radius, 1);
            Point[] q2Points = PointsFromQuadrant(new Point(-dxc, -dyc), radius, 2);
            Point[] q3Points = PointsFromQuadrant(new Point(dxc, -dyc), radius, 3);

            Point[] points = new Point[4 * 9];
            q0Points.CopyTo(points, 0);
            q1Points.CopyTo(points, 9);
            q2Points.CopyTo(points, 18);
            q3Points.CopyTo(points, 27);

            return points;
        }

        /// <summary>
        /// Crea una sequencia de punts per un poligon en forma de cercle, format
        /// per 64 segments de recta i centrat en l'origen de coordinades.
        /// </summary>
        /// <param name="radius">Radi del cercle.</param>
        /// <returns>La sequencia de punts.</returns>
        /// 
        private static Point[] PointsFromCircle(double radius) {

            return PointsFromRegularPolygon(32, radius);
        }

        /// <summary>
        /// Crea una sequencia de punts per un poligon regular centrat en 
        /// l'origen, on la base es una aresta. El poligon es tancat.
        /// </summary>
        /// <param name="sides">Numero de arestes.</param>
        /// <param name="radius">El radi del cercle exterior.</param>
        /// <returns>La sequencia de punts.</returns>
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

        /// <summary>
        /// Crea una sequencia de punts per un quadrant. 
        /// </summary>
        /// <param name="center">Centre del quandrant.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="quadrant">Numero de quadrant (0..3)</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private static Point[] PointsFromQuadrant(Point center, double radius, int quadrant) {

            Point[] points = new Point[9];

            double delta = (360.0 * Math.PI / 180.0) / 32;
            double angle = quadrant * 90 * Math.PI / 180;
            for (int i = 0; i < 9; i++) {
                points[i] = new Point(center.X + (radius * Math.Cos(angle)), center.Y + (radius * Math.Sin(angle)));
                angle += delta;
            }

            return points;
        }
    }
}
