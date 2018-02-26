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
            Angle angle = Angle.FromRadiants(Math.Atan2(dy, dx));

            Polygon polygon = new Polygon();
            polygon.AddPoints(ArcPoints(end, thickness / 2, angle + Angle.Deg270, Angle.FromDegrees(180)));
            polygon.AddPoints(ArcPoints(start, thickness / 2, angle + Angle.Deg90, Angle.FromDegrees(180)));
            polygon.Pack();

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
        public static Polygon BuildArcSegment(Point center, double radius, Angle startAngle, Angle angle, double thickness, bool capRounded = true) {

            double innerRadius = radius - (thickness / 2.0);
            double outerRadius = radius + (thickness / 2.0);

            Polygon polygon = new Polygon();
            polygon.AddPoints(ArcPoints(center, outerRadius, startAngle, angle));
            polygon.AddPoints(ArcPoints(center, innerRadius, startAngle + angle, -angle));
            polygon.Pack();

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

            Polygon polygon = new Polygon(PolygonPoints(32, center, radius, Angle.Zero));
            polygon.Pack();

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
        /// 
        public static Polygon BuildRectangle(Point position, Size size, double radius, Angle rotation) {

            Polygon polygon = new Polygon();

            double x = position.X;
            double y = position.Y;
            double dx = (size.Width / 2) - radius;
            double dy = (size.Height / 2) - radius;

            // En aquestes rotacions, nomes cal intercanviar amplada per alçada
            //
            if (rotation.IsVertical) {
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
                polygon.AddPoints(ArcPoints(new Point(x + dx, y + dy), radius, Angle.Zero, Angle.Deg90));
                polygon.AddPoints(ArcPoints(new Point(x - dx, y + dy), radius, Angle.Deg90, Angle.Deg90));
                polygon.AddPoints(ArcPoints(new Point(x - dx, y - dy), radius, Angle.Deg180, Angle.Deg90));
                polygon.AddPoints(ArcPoints(new Point(x + dx, y - dy), radius, Angle.Deg270, Angle.Deg90));
            }

            // Si es una rotacio arbitraria, fa servir calcul amb matrius
            //
            if (!rotation.IsOrthogonal) {
                Matrix m = new Matrix();
                m.RotateAt(rotation.Degrees, x, y);
                polygon.Transform(m);
            }

            polygon.Pack();

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
        public static Polygon BuildCross(Point position, Size size, double thickness, Angle rotation) {

            double dx = size.Width / 2;
            double dy = size.Height / 2;

            // En aquestes rotacions, nomes cal intercanviar amplada per alçada
            //
            if (rotation.IsVertical) {
                double temp = dx;
                dx = dy;
                dy = temp;
            }

            double dt = thickness / 2;

            double x = position.X;
            double y = position.Y;

            double xMin = x - dx;
            double yMin = y - dy;

            double xMax = x + dx;
            double yMax = y + dy;

            Point[] points = new Point[12];

            points[0].X = xMax;
            points[0].Y = y - dt;
            points[1].X = x + dt;
            points[1].Y = y - dt;
            points[2].X = x + dt;
            points[2].Y = yMin;

            points[3].X = x - dt;
            points[3].Y = yMin;
            points[4].X = x - dt;
            points[4].Y = y - dt;
            points[5].X = xMin;
            points[5].Y = y - dt;

            points[6].X = xMin;
            points[6].Y = y + dt;
            points[7].X = x - dt;
            points[7].Y = y + dt;
            points[8].X = x - dt;
            points[8].Y = yMax;

            points[9].X = x + dt;
            points[9].Y = yMax;
            points[10].X = x + dt;
            points[10].Y = y + dt;
            points[11].X = xMax;
            points[11].Y = y + dt;

            Polygon polygon = new Polygon(points);

            // Si es una rotacio arbitraria, fa servir calcul amb matrius
            //
            if (!rotation.IsOrthogonal) {
                Matrix m = new Matrix();
                m.RotateAt(rotation.Degrees, x, y);
                polygon.Transform(m);
            }

            polygon.Pack();

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
        public static Polygon BuildRegularPolygon(int sides, Point position, double radius, Angle rotation) {

            Polygon polygon = new Polygon(PolygonPoints(sides, position, radius, rotation));
            polygon.Pack();

            return polygon;
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
        public static Point[] PolygonPoints(int sides, Point position, double radius, Angle rotation) {

            // Calcula el punt inicial
            //
            double x = radius * Math.Cos(rotation.Radiants);
            double y = radius * Math.Sin(rotation.Radiants);

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
        public static Point[] ArcPoints(Point center, double radius, Angle startAngle, Angle angle, bool discardLast = false) {

            int numSegments = (int) Math.Floor((Math.Abs(angle.Degrees)) * 32.0 / 360.0);
            int numPoints = numSegments + (discardLast ? 0 : 1);

            // Calcula el punt inicial
            //
            double x = radius * Math.Cos(startAngle.Radiants);
            double y = radius * Math.Sin(startAngle.Radiants);

            // Crea l'array de punts
            //
            Point[] points = new Point[numPoints];

            // Genera els punts, aplicant un gir a l'anterior
            //
            double cos = Math.Cos(angle.Radiants / (double) numSegments);
            double sin = Math.Sin(angle.Radiants / (double) numSegments);

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
