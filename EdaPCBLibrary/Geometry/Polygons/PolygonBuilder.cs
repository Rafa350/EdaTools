namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Clase que permet la construccio de diferent poligons.
    /// </summary>
    public static class PolygonBuilder {

        /// <summary>
        /// Crea un poligon en forma de segment de linia amb finals arrodonits
        /// </summary>
        /// <param name="start">Posicio inicial.</param>
        /// <param name="end">Posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="capRounded">True si els extrems son arrodinits.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildLineSegment(Point start, Point end, double thickness, bool capRounded = true) {

            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;

            Polygon polygon = new Polygon();
            polygon.AddPoints(ArcPoints(end, thickness / 2, 270 + angle, 180));
            polygon.AddPoints(ArcPoints(start, thickness / 2, 90 + angle, 180));

            return polygon;
        }

        /// <summary>
        /// Crea un poligon en forma de segment d'arc amb finals de linia rectes.
        /// </summary>
        /// <param name="center">Centre de l'arc.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="startAngle">Angle inicial.</param>
        /// <param name="angle">Angle de recoregut.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="capRounded">True si els extrems son arrodonits.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildArcSegment(Point center, double radius, double startAngle, double angle, double thickness, bool capRounded = true) {

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

            // Rectangle sense cantonades arrodonides
            //
            if (radius == 0) {
                polygon.AddPoint(new Point(x - dx, y + dy));
                polygon.AddPoint(new Point(x + dx, y + dy));
                polygon.AddPoint(new Point(x + dx, y - dy));
                polygon.AddPoint(new Point(x - dx, y - dy));
            }

            // Rectangle amb cantonades arrodonides
            //
            else {
                polygon.AddPoints(ArcPoints(new Point(x + dx, y + dy), radius, 0, 90));
                polygon.AddPoints(ArcPoints(new Point(x - dx, y + dy), radius, 90, 90));
                polygon.AddPoints(ArcPoints(new Point(x - dx, y - dy), radius, 180, 90));
                polygon.AddPoints(ArcPoints(new Point(x + dx, y - dy), radius, 270, 90));
            }

            // Si es una rotacio arbitraria, fa servir calcul amb matrius
            //
            if ((rotation % 90) != 0) {
                Matrix m = new Matrix();
                m.RotateAt(rotation, x, y);
                polygon.Transform(m);
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

            // En aquestes rotacions, nomes cal intercanviar amplada per alçada
            //
            if ((rotation == 90) || (rotation == 270)) {
                double temp = dx;
                dx = dy;
                dy = temp;
            }

            double dt = thickness / 2;

            double x = position.X;
            double y = position.Y;

            double x1 = x - dx;
            double y1 = y - dy;

            double x2 = x + dx;
            double y2 = y + dy;

            Point[] points = new Point[12];

            points[0].X = x2;
            points[0].Y = y - dt;
            points[1].X = x + dt;
            points[1].Y = y - dt;
            points[2].X = x + dt;
            points[2].Y = y1;

            points[3].X = x - dt;
            points[3].Y = y1;
            points[4].X = x - dt;
            points[4].Y = y - dt;
            points[5].X = x1;
            points[5].Y = y - dt;

            points[6].X = x1;
            points[6].Y = y - dt;
            points[7].X = x - dt;
            points[7].Y = y - dt;
            points[8].X = x - dt;
            points[8].Y = y2;

            points[9].X = x + dt;
            points[9].Y = y2;
            points[10].X = x + dt;
            points[10].Y = y - dt;
            points[11].X = x2;
            points[11].Y = y - dt;

            Polygon polygon = new Polygon(points);

            // Si es una rotacio arbitraria, fa servir calcul amb matrius
            //
            if ((rotation % 90) != 0) {
                Matrix m = new Matrix();
                m.RotateAt(rotation, x, y);
                polygon.Transform(m);
            }

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
        /// Crea un array de punts per un arc.
        /// </summary>
        /// <param name="center">Coordinades del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <param name="startAngle">Angle inicial</param>
        /// <param name="angle">Angle del arc.</param>
        /// <param name="discardLast">Si es true, descarta l'ultim punt.</param>
        /// <returns>L'array de punts. Com es obert, el numero de punts es
        /// el numero de segments mes u.
        /// </returns>
        /// 
        private static Point[] ArcPoints(Point center, double radius, double startAngle, double angle, bool discardLast = false) {

            int numSegments = (int) Math.Floor((Math.Abs(angle)) * 32.0 / 360.0);
            int numPoints = numSegments + (discardLast ? 0 : 1);

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
    }
}
