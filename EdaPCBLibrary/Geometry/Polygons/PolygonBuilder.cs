namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Windows;
    using System.Windows.Media;

    public static class PolygonBuilder {

        /// <summary>
        /// Crea un poligon en forma de linia ample amb finals arrodonits
        /// </summary>
        /// <param name="start">Posicio inicial.</param>
        /// <param name="end">Posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildLine(Point start, Point end, double thickness) {

            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double length = Math.Sqrt((dx * dx) + (dy * dy));
            Point[] points = PointsFromLine(length, thickness);

            // Realitza la transformacio dels punts, a la posicio i 
            // orientacio finals.
            //
            Matrix m = new Matrix();
            m.Translate(start.X, start.Y);
            double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
            m.RotateAt(angle, start.X, start.Y);
            m.Transform(points);

            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon en forma de cercle.
        /// </summary>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildCircle(Point position, double radius) {

            Point[] points = PointsFromCircle(position, radius);
            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon en forma de rectangle.
        /// </summary>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="size">Tamany.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildRectangle(Point position, Size size, double radius, Angle rotation) {

            if (radius == 0)
                return new Polygon(PointsFromRectangle(position, size, rotation.Degrees));
            else
                return new Polygon(PointsFromRoundRectangle(position, size, radius, rotation.Degrees));
        }

        /// <summary>
        /// Crea un poligon regular.
        /// </summary>
        /// <param name="sides">Nombre de cares.</param>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="radius">Radi del cercle maxim.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildRegularPolygon(int sides, Point position, double radius, Angle rotation) {

            return new Polygon(PointsFromRegularPolygon(sides, position, radius, rotation.Degrees));
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
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="size">Tamany del rectangle.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private static Point[] PointsFromRectangle(Point position, Size size, double rotation) {

            // Crea el rectangle unitari
            //
            Point[] points = new Point[4];
            points[0].X = -0.5;
            points[0].Y = 0.5;
            points[1].X = 0.5;
            points[1].Y = 0.5;
            points[2].X = 0.5;
            points[2].Y = -0.5;
            points[3].X = -0.5;
            points[3].Y = -0.5;

            // El transforma al rectangle a la posicio, tamany i orientacio final.
            //
            Matrix m = new Matrix();
            m.Scale(size.Width, size.Height);
            m.Rotate(rotation);
            m.Translate(position.X, position.Y);
            m.Transform(points);

            return points;
        }

        /// <summary>
        /// Crea una sequencia de punts per un rectangle arrodonit.
        /// </summary>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="size">Tamany del rectangle.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La kkista de punts.</returns>
        /// 
        private static Point[] PointsFromRoundRectangle(Point position, Size size, double radius, double rotation) {

            double cx = (size.Width / 2) - radius;
            double cy = (size.Height / 2) - radius;

            Point[] q0Points = PointsFromQuadrant(new Point(cx, cy), radius, 0);
            Point[] q1Points = PointsFromQuadrant(new Point(-cx, cy), radius, 1);
            Point[] q2Points = PointsFromQuadrant(new Point(-cx, -cy), radius, 2);
            Point[] q3Points = PointsFromQuadrant(new Point(cx, -cy), radius, 3);

            Point[] points = new Point[4 * 9];
            q0Points.CopyTo(points, 0);
            q1Points.CopyTo(points, 9);
            q2Points.CopyTo(points, 18);
            q3Points.CopyTo(points, 27);

            // Transforma a la posicio i orientacio final
            //
            Matrix m = new Matrix();
            m.Rotate(rotation);
            m.Translate(position.X, position.Y);
            m.Transform(points);

            return points;
        }

        /// <summary>
        /// Crea una sequencia de punts per un poligon en forma de cercle, format
        /// per 64 segments de recta i centrat en l'origen de coordinades.
        /// </summary>
        /// <param name="radius">Radi del cercle.</param>
        /// <returns>La sequencia de punts.</returns>
        /// 
        private static Point[] PointsFromCircle(Point position, double radius) {

            return PointsFromRegularPolygon(32, position, radius, 0);
        }

        /// <summary>
        /// Crea una sequencia de punts per un poligon regular, on la base es una aresta.
        /// </summary>
        /// <param name="sides">Numero de arestes.</param>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="radius">El radi del cercle exterior.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La sequencia de punts.</returns>
        /// 
        private static Point[] PointsFromRegularPolygon(int sides, Point position, double radius, double rotation) {

            Point[] points = new Point[sides];

            double x = position.X;
            double y = position.Y;
            double delta = (360.0 * Math.PI / 180.0) / sides;
            double angle = (rotation * Math.PI / 180.0) + (delta / 2);

            for (int i = 0; i < sides; i++) {
                points[i] = new Point(x + (radius * Math.Cos(angle)), y + (radius * Math.Sin(angle)));
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
