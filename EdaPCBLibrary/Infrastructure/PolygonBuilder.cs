namespace MikroPic.EdaTools.v1.Pcb.Infrastructure {

    using System;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Geometry;

    /// <summary>
    /// Clase que permet la construccio llistes de punts que representen diferentes figures,
    /// utilitzades comunment pel les plaques.
    /// </summary>
    public static class PolygonBuilder {

        /// <summary>
        /// Crea una llista de punts en forma de segment de linia amb finals arrodonits
        /// </summary>
        /// <param name="start">Posicio inicial.</param>
        /// <param name="end">Posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="capRounded">True si els extrems son arrodinits.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static Point[] BuildTrace(Point start, Point end, double thickness, bool capRounded = true) {

            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            Angle angle = Angle.FromRadiants(Math.Atan2(dy, dx));

            Point[] a1 = BuildArc(end, thickness / 2, angle + Angle.Deg270, Angle.FromDegrees(180));
            Point[] a2 = BuildArc(start, thickness / 2, angle + Angle.Deg90, Angle.FromDegrees(180));

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
        public static Point[] BuildTrace(Point center, double radius, Angle startAngle, Angle angle, double thickness, bool capRounded = true) {

            double innerRadius = radius - (thickness / 2.0);
            double outerRadius = radius + (thickness / 2.0);

            Point[] a1 = BuildArc(center, outerRadius, startAngle, angle);
            Point[] a2 = BuildArc(center, innerRadius, startAngle + angle, -angle);

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
        public static Point[] BuildCircle(Point center, double radius) {

            return BuildPolygon(32, center, radius, Angle.Zero);
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
        public static Point[] BuildRectangle(Point position, Size size, double radius, Angle rotation) {

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
            Point[] points;
            if (radius == 0) {

                points = new Point[4];
                points[0] = new Point(x - dx, y + dy);
                points[1] = new Point(x + dx, y + dy);
                points[2] = new Point(x + dx, y - dy);
                points[3] = new Point(x - dx, y - dy);
            }

            // Rectangle amb cantonades arrodonides
            //
            else {

                Point[] a1 = BuildArc(new Point(x + dx, y + dy), radius, Angle.Zero, Angle.Deg90);
                Point[] a2 = BuildArc(new Point(x - dx, y + dy), radius, Angle.Deg90, Angle.Deg90);
                Point[] a3 = BuildArc(new Point(x - dx, y - dy), radius, Angle.Deg180, Angle.Deg90);
                Point[] a4 = BuildArc(new Point(x + dx, y - dy), radius, Angle.Deg270, Angle.Deg90);

                points = new Point[a1.Length + a2.Length + a3.Length + a4.Length];
                a1.CopyTo(points, 0);
                a2.CopyTo(points, a1.Length);
                a3.CopyTo(points, a1.Length + a2.Length);
                a4.CopyTo(points, a1.Length + a2.Length + a3.Length);
            }

            // Si es una rotacio arbitraria, fa servir calcul amb matrius
            //
            if (!rotation.IsOrthogonal) {
                Matrix m = new Matrix();
                m.RotateAt(rotation.Degrees, x, y);
                m.Transform(points);
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
        public static Point[] BuildCross(Point position, Size size, double thickness, Angle rotation) {

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

            // Si es una rotacio arbitraria, fa servir calcul amb matrius
            //
            if (!rotation.IsOrthogonal) {
                Matrix m = new Matrix();
                m.RotateAt(rotation.Degrees, x, y);
                m.Transform(points);
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
        public static Point[] BuildPolygon(int sides, Point position, double radius, Angle rotation) {

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
        public static Point[] BuildArc(Point center, double radius, Angle startAngle, Angle angle, bool discardLast = false) {

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
