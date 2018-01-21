namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System;
    using System.Windows;
    using System.Windows.Media;

    public static class PolygonBuilder {

        private const int circleSegments = 32;

        private const double minSegmentAngle = 5.0;
        private const double minSegmentSize = 0.05;
        private const double numSegments = 32;

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
        /// <param name="deltaAngle"></param>
        /// <param name="deltaAngle"></param>
        /// <param name="thickness"></param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildArcSegment(Point center, double radius, double startAngle, double angle, double thickness) {

            double innerRadius = radius - (thickness / 2.0);
            double outerRadius = radius + (thickness / 2.0);

            Polygon polygon = new Polygon();
            polygon.AddPoints(ArcPoints(center, outerRadius, startAngle, angle));
            polygon.AddPoints(ArcPoints(center, innerRadius, startAngle + angle, -angle));

            return polygon;
        }

        /// <summary>
        /// Crea un poligon en forma de cercle.
        /// </summary>
        /// <param name="center">Posicio del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildCircle(Point center, double radius) {

            return new Polygon(PolygonPoints(32, center, radius, 0));
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
        /// Crea una polygon en forma de creu.
        /// </summary>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="size">Tamany del rectangle exterior.</param>
        /// <param name="thickness">Amplada dels braços</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildCross(Point position, Size size, double thickness, double rotation) {

            double dx = size.Width / 2;
            double dy = size.Height / 2;
            double dt = thickness / 2;

            double x0 = position.X;
            double y0 = position.Y;

            double x1 = x0 - dx;
            double y1 = y0 - dy;

            double x2 = x0 + dx;
            double y2 = y0 + dy;

            Point[] points = new Point[12];

            points[0].X = x1;
            points[0].Y = y0 - dt;
            points[1].X = x0 - dt;
            points[1].Y = y0 - dt;
            points[2].X = x0 - dt;
            points[2].Y = y1;

            points[3].X = x1;
            points[3].Y = y0 - dt;
            points[4].X = x0 - dt;
            points[4].Y = y0 - dt;
            points[5].X = x0 - dt;
            points[5].Y = y1;

            points[6].X = x1;
            points[6].Y = y0 - dt;
            points[7].X = x0 - dt;
            points[7].Y = y0 - dt;
            points[8].X = x0 - dt;
            points[8].Y = y1;

            points[9].X = x1;
            points[9].Y = y0 - dt;
            points[10].X = x0 - dt;
            points[10].Y = y0 - dt;
            points[11].X = x0 - dt;
            points[11].Y = y1;

            Polygon polygon = new Polygon(points);

            return polygon;
        }

        /// <summary>
        /// Crea un poligon regular. El primer punt es sobre l'eix X.
        /// </summary>
        /// <param name="sides">Nombre de cares.</param>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="radius">Radi del cercle maxim.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildRegularPolygon(int sides, Point position, double radius, double rotation) {

            return new Polygon(PolygonPoints(sides, position, radius, rotation));
        }

        /// <summary>
        /// Crea un array de punts per un poligon regular. El primer punt
        /// es sobre l'eix X (Y = 0)
        /// </summary>
        /// <param name="sides">Numero de cares.</param>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="radius">Radi exterior.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>L'array de punts. Com es tancat, el numero de punts 
        /// coincideix amb en numero de segments.
        /// </returns>
        /// 
        private static Point[] PolygonPoints(int sides, Point position, double radius, double rotation) {

            // Calcula el punt inicial
            //
            double startAngle = rotation * Math.PI / 180.0;
            double x = radius * Math.Cos(startAngle);
            double y = radius * Math.Sin(startAngle);

            // Crea l'array de punts
            //
            Point[] points = new Point[sides];

            // Genera els punts, aplicant un gir a l'anterior
            //
            double angle = 2 * Math.PI / sides;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            for (int i = 0; i < sides; i++) {
                points[i].X = x + position.X;
                points[i].Y = y + position.Y;

                double temp = x;
                x = cos * x - sin * y;
                y = sin * temp + cos * y;
            }

            return points;
        }

        /// <summary>
        /// Crea un array de puntsa per un arc.
        /// </summary>
        /// <param name="center">Coordinades del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <param name="startAngle">Angle inicial</param>
        /// <param name="angle">Angle del arc.</param>
        /// <returns>L'array de punts. Com es obert, el numero de punns es
        /// el numero de segments mes u.
        /// </returns>
        /// 
        private static Point[] ArcPoints(Point center, double radius, double startAngle, double angle) {

            int numSegments = (int) Math.Floor((Math.Abs(angle)) * 32.0 / 360.0);
            int numPoints = numSegments + 1;

            // Calcula el punt inicial
            //
            double x = radius * Math.Cos(startAngle * Math.PI / 180.0);
            double y = radius * Math.Sin(startAngle * Math.PI / 180.0);

            // Crea l'array de punts
            //
            Point[] points = new Point[numPoints];

            // Genera els punts, aplicant un gir a l'anterior
            //
            double cos = Math.Cos((angle * Math.PI / 180.0) / (double) numSegments);
            double sin = Math.Sin((angle * Math.PI / 180.0) / (double) numSegments);

            for (int i = 0; i < numPoints; i++) {
                points[i].X = x + center.X;
                points[i].Y = y + center.Y;

                double temp = x;
                x = cos * x - sin * y;
                y = sin * temp + cos * y;
            }

            return points;
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
