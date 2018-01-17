namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System;
    using System.Windows;
    using System.Windows.Media;

    public static class PolygonBuilder {

        private const int circleSegments = 32;

        private static readonly Point[] circlePoints;

        /// <summary>
        /// Constructor estatic de l'objecte.
        /// </summary>
        /// 
        static PolygonBuilder() {

            // Inicialitza la taula de punts pel calcul de cercles. Els
            // punts corresponen a un cercle unitari centrat en l'origen.
            // D'aquesta manera ens estalviem de calcular els sinus i
            // cosinus cada cop que calgui dibuixar un cercle.
            //
            circlePoints = new Point[circleSegments + 1];
            double angle = 0;
            double delta = (360.0 * Math.PI / 180.0) / circleSegments;
            for (int i = 0; i < circleSegments; i++) {
                circlePoints[i] = new Point(Math.Cos(angle), Math.Sin(angle));
                angle += delta;
            }

            // Tanca el cercle per facilitar els calculs
            //
            circlePoints[circleSegments] = circlePoints[0];
        }

        /// <summary>
        /// Crea un poligon en forma de segment de linia amb finals arrodonits
        /// </summary>
        /// <param name="start">Posicio inicial.</param>
        /// <param name="end">Posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildLineSegment(Point start, Point end, double thickness) {

            // Crea el segment en la direccio X+ i Y=0
            //
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
        /// Crea un poligon en forma de segment d'arc amb finals de linia rectes.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static Polygon BuildArcSegment(Point center, double radius, double startAngle, double endAngle, double thickness) {

            return new Polygon(PointsFromArc(center, radius, startAngle, endAngle, thickness));
        }

        /// <summary>
        /// Crea un poligon en forma de cercle.
        /// </summary>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildCircle(Point position, double radius) {

            Polygon polygon = new Polygon();

            for (int i = 0; i < circleSegments; i++)
                polygon.AddPoint(new Point(position.X + circlePoints[i].X * radius, position.Y + circlePoints[i].Y * radius));

            return polygon;
        }

        /// <summary>
        /// Crea un poligon en forma de rectangle.
        /// </summary>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="size">Tamany.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>El poligon.</returns>
        /// <remarks> 
        /// Optimitza les orientacions ortogonals. Nomes fa servir matrius en les rotacions 
        /// en àngles arbitraris
        /// </remarks>
        /// 
        public static Polygon BuildRectangle(Point position, Size size, double radius, double rotation) {

            Polygon polygon = new Polygon();

            double x = position.X;
            double y = position.Y;
            double dx = (size.Width / 2) - radius;
            double dy = (size.Height / 2) - radius;

            // En aquestes rotacions, nomes cal intercanviar amplada per alçada
            //
            if ((rotation == 90) || (rotation == 270)) {
                double temp = dx;
                dx = dy;
                dy = temp;
            }

            if (radius == 0) {

                polygon.AddPoint(new Point(x - dx, y + dy));
                polygon.AddPoint(new Point(x + dx, y + dy));
                polygon.AddPoint(new Point(x + dx, y - dy));
                polygon.AddPoint(new Point(x - dx, y - dy));

                // Si es una rotacio arbitraria, fa servir calcul amb matrius
                //
                if ((rotation % 90) != 0) {
                    Matrix m = new Matrix();
                    m.RotateAt(rotation, x, y);
                    polygon.Transform(m);
                }
            }

            else {

                for (int i = 0; i <= 8; i++) {
                    polygon.AddPoint(
                        new Point(
                            x + circlePoints[i].X * radius + dx,
                            y + circlePoints[i].Y * radius + dy));
                }

                for (int i = 8; i <= 16; i++) {
                    polygon.AddPoint(
                        new Point(
                            x + circlePoints[i].X * radius - dx,
                            y + circlePoints[i].Y * radius + dy));
                }

                for (int i = 16; i <= 24; i++) {
                    polygon.AddPoint(
                        new Point(
                            x + circlePoints[i].X * radius - dx,
                            y + circlePoints[i].Y * radius - dy));
                }

                for (int i = 24; i <= 32; i++) {
                    polygon.AddPoint(
                        new Point(
                            x + circlePoints[i].X * radius + dx,
                            y + circlePoints[i].Y * radius - dy));
                }

                // Si es una rotacio arbitraria, fa servir calcul amb matrius
                //
                if ((rotation % 90) != 0) {
                    Matrix m = new Matrix();
                    m.RotateAt(rotation, x, y);
                    polygon.Transform(m);
                }
            }

            return polygon;
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
        public static Polygon BuildRegularPolygon(int sides, Point position, double radius, double rotation) {

            double delta = (360.0 * Math.PI / 180.0) / sides;
            double angle = (rotation * Math.PI / 180.0) + (delta / 2);

            Polygon polygon = new Polygon();

            for (int i = 0; i < sides; i++) {
                polygon.AddPoint(
                    new Point(
                        position.X + (radius * Math.Cos(angle)), 
                        position.Y + (radius * Math.Sin(angle))));
                angle += delta;
            }

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

        private static Point[] PointsFromArc(Point center, double radius, double startAngle, double endAngle, double thickness) {

            int intervals = 10;
            double deltaAngle = (endAngle - startAngle) / (intervals - 1);

            double x = center.X * radius * Math.Cos(startAngle * Math.PI / 180.0);
            double y = center.Y * radius * Math.Sin(startAngle * Math.PI / 180.0);

            Point[] points = new Point[intervals];
            for (int i = 0; i < intervals; i++) {
                points[i] = new Point(x, y);
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

            // Crea el cercle unitari
            //
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
