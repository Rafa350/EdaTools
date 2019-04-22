namespace MikroPic.EdaTools.v1.Core.Infrastructure.Polygons {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using System;

    /// <summary>
    /// Clase que permet la construccio llistes de punts que representen diferentes figures,
    /// utilitzades comunment pel les plaques. Important: Tots els poligons cal que tinguin
    /// l'orientacio CCW.
    /// </summary>
    internal static class PolygonBuilder {

        /// <summary>
        /// Crea una llista de punts en forma de segment de linia amb finals arrodonits
        /// </summary>
        /// <param name="start">Posicio inicial.</param>
        /// <param name="end">Posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="capRounded">True si els extrems son arrodinits.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static Point[] MakeLineTrace(Point start, Point end, int thickness, bool capRounded) {

            int dx = end.X - start.X;
            int dy = end.Y - start.Y;
            Angle angle = Angle.FromRadiants(Math.Atan2(dy, dx));

            Point[] a1 = MakeArc(end, thickness / 2, angle + Angle.Deg270, Angle.Deg180);
            Point[] a2 = MakeArc(start, thickness / 2, angle + Angle.Deg90, Angle.Deg180);

            Point[] points = new Point[a1.Length + a2.Length];
            a1.CopyTo(points, 0);
            a2.CopyTo(points, a1.Length);

            return points;
        }

        /// <summary>
        /// Crea una llista de punts en forma de segment d'arc amb finals de linia rectes.
        /// </summary>
        /// <param name="center">Centre de l'arc.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="startAngle">Angle inicial.</param>
        /// <param name="angle">Angle de recoregut.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="capRounded">True si els extrems son arrodonits.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static Point[] MakeArcTrace(Point center, int radius, Angle startAngle, Angle angle, int thickness, bool capRounded) {

            int innerRadius = radius - (thickness / 2);
            int outerRadius = radius + (thickness / 2);

            Point[] a1 = MakeArc(center, outerRadius, startAngle, angle);
            Point[] a2 = MakeArc(center, innerRadius, startAngle + angle, -angle);

            Point[] points = new Point[a1.Length + a2.Length];
            a1.CopyTo(points, 0);
            a2.CopyTo(points, a1.Length);

            return points;
        }

        /// <summary>
        /// Crea una llista de punts en forma de cercle.
        /// </summary>
        /// <param name="center">Posicio del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static Point[] MakeCircle(Point center, int radius) {

            return MakeRegularPolygon(32, center, radius, Angle.Zero);
        }

        /// <summary>
        /// Crea un llista de punts en forma de rectangle. Opcionsalment les
        /// cantonades poder ser arrodonides.
        /// </summary>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="size">Tamany.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static Point[] MakeRectangle(Point position, Size size, int radius, Angle rotation) {

            int x = position.X;
            int y = position.Y;
            int dx = (size.Width / 2) - radius;
            int dy = (size.Height / 2) - radius;

            // En aquestes rotacions, nomes cal intercanviar amplada per alçada
            //
            if (rotation.IsVertical) {
                int temp = dx;
                dx = dy;
                dy = temp;
            }

            // Rectangle sense cantonades arrodonides
            //
            Point[] points;
            if (radius == 0) {

                points = new Point[4];
                points[0] = new Point(x + dx, y + dy);
                points[1] = new Point(x - dx, y + dy);
                points[2] = new Point(x - dx, y - dy);
                points[3] = new Point(x + dx, y - dy);
            }

            // Rectangle amb cantonades arrodonides
            //
            else {

                Point[] a1 = MakeArc(new Point(x + dx, y + dy), radius, Angle.Zero, Angle.Deg90);
                Point[] a2 = MakeArc(new Point(x - dx, y + dy), radius, Angle.Deg90, Angle.Deg90);
                Point[] a3 = MakeArc(new Point(x - dx, y - dy), radius, Angle.Deg180, Angle.Deg90);
                Point[] a4 = MakeArc(new Point(x + dx, y - dy), radius, Angle.Deg270, Angle.Deg90);

                points = new Point[a1.Length + a2.Length + a3.Length + a4.Length];
                a1.CopyTo(points, 0);
                a2.CopyTo(points, a1.Length);
                a3.CopyTo(points, a1.Length + a2.Length);
                a4.CopyTo(points, a1.Length + a2.Length + a3.Length);
            }

            // Si es una rotacio arbitraria, fa servir calcul amb matrius
            //
            if (!rotation.IsOrthogonal) {
                Transformation t = new Transformation();
                t.Rotate(position, rotation);
                t.ApplyTo(points);
            }

            return points;
        }

        /// <summary>
        /// Crea una llista de punts en forma de creu.
        /// </summary>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="size">Tamany del rectangle exterior.</param>
        /// <param name="thickness">Amplada dels braços</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static Point[] MakeCross(Point position, Size size, int thickness, Angle rotation) {

            int dx = size.Width / 2;
            int dy = size.Height / 2;

            // En aquestes rotacions, nomes cal intercanviar amplada per alçada
            //
            if (rotation.IsVertical) {
                int temp = dx;
                dx = dy;
                dy = temp;
            }

            int dt = thickness / 2;

            int x = position.X;
            int y = position.Y;

            int xMin = x - dx;
            int yMin = y - dy;

            int xMax = x + dx;
            int yMax = y + dy;

            Point[] points = new Point[12];

            points[0] = new Point(xMax, y + dt);
            points[1] = new Point(x + dt, y + dt);
            points[2] = new Point(x + dt, yMax);

            points[3] = new Point(x - dt, yMax);
            points[4] = new Point(x - dt, y + dt);
            points[5] = new Point(xMin, y + dt);

            points[6] = new Point(xMin, y - dt);
            points[7] = new Point(x - dt, y - dt);
            points[8] = new Point(x - dt, yMin);

            points[9] = new Point(x + dt, yMin);
            points[10] = new Point(x + dt,  y - dt);
            points[11] = new Point(xMax, y - dt);

            // Si es una rotacio arbitraria, fa servir calcul amb matrius
            //
            if (!rotation.IsOrthogonal) {
                Transformation t = new Transformation();
                t.Rotate(position, rotation);
                t.ApplyTo(points);
            }

            return points;
        }

        /// <summary>
        /// Crea una llista de punts per un poligon regular. El primer punt
        /// es sobre l'eix X, en cas que la rotacio sigui zero
        /// </summary>
        /// <param name="sides">Numero de cares.</param>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="radius">Radi exterior.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La llista de punts. Com es tancat, el numero de punts 
        /// coincideix amb en numero de segments.
        /// </returns>
        /// 
        public static Point[] MakeRegularPolygon(int sides, Point position, int radius, Angle rotation) {

            // Calcula el punt inicial
            //
            double x = radius * Math.Cos(rotation.ToRadiants);
            double y = radius * Math.Sin(rotation.ToRadiants);

            // Calcula els sinus i cosinus del gir a aplicar a cada iteracio
            //
            double angle = 2 * Math.PI / sides;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            // Crea l'array de punts
            //
            Point[] points = new Point[sides];

            // Ompla l'array amb els punts calculats.
            //
            for (int i = 0; i < sides; i++) {

                points[i] = new Point(
                    (int)x + position.X, 
                    (int)y + position.Y);

                double tx = x;
                x = (cos * tx) - (sin * y);
                y = (sin * tx) + (cos * y);
            }

            return points;
        }

        /// <summary>
        /// Crea una llista de punts per un arc.
        /// </summary>
        /// <param name="center">Coordinades del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <param name="startAngle">Angle inicial</param>
        /// <param name="angle">Angle del arc.</param>
        /// <param name="discardLast">Si es true, descarta l'ultim punt.</param>
        /// <returns>La llista de punts. Com es obert, el numero de punts es
        /// el numero de segments mes un.
        /// </returns>
        /// 
        public static Point[] MakeArc(Point center, int radius, Angle startAngle, Angle angle, bool discardLast = false) {

            // Calcula el numero de segments
            //
            int numSegments = (int) Math.Abs(Math.Floor((angle.ToDegrees * 32.0) / 360.0));
            int numPoints = numSegments + (discardLast ? 0 : 1);

            // Calcula l'angle de cada segment
            //
            double radSegment = angle.ToRadiants / numSegments;

            // Calcula el centre
            //
            double cx = center.X;
            double cy = center.Y;

            // Calcula el punt inicial
            //
            double x = radius * Math.Cos(startAngle.ToRadiants);
            double y = radius * Math.Sin(startAngle.ToRadiants);

            // Calcula el sinus i el cosinus del gir a aplicar a cada iteracio
            //
            double cos = Math.Cos(radSegment);
            double sin = Math.Sin(radSegment);

            // Crea l'array de punts
            //
            Point[] points = new Point[numPoints];

            for (int i = 0; i < numPoints; i++) {

                points[i] = new Point((int)(x + cx), (int)(y + cy));

                double tx = x;
                x = (cos * tx) - (sin * y);
                y = (sin * tx) + (cos * y);
            }

            return points;
        }
    }
}
