namespace MikroPic.EdaTools.v1.Core.Infrastructure.Polygons {

    using System;

    using MikroPic.EdaTools.v1.Base.Geometry;

    /// <summary>
    /// Clase que permet la construccio llistes de punts que representen diferentes figures,
    /// utilitzades comunment pel les plaques. Important: Tots els poligons cal que tinguin
    /// l'orientacio CCW.
    /// </summary>
    /// 
    internal static class PolygonBuilder {

        private const int circleFacets = 32;

        /// <summary>
        /// Crea una llista de punts en forma de segment de linia amb finals arrodonits
        /// </summary>
        /// <param name="start">Posicio inicial.</param>
        /// <param name="end">Posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="capRounded">True si els extrems son arrodonits.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static EdaPoint[] MakeLineTrace(EdaPoint start, EdaPoint end, int thickness, bool capRounded) {

            int dx = end.X - start.X;
            int dy = end.Y - start.Y;
            EdaAngle angle = EdaAngle.FromRadiants(Math.Atan2(dy, dx));

            EdaPoint[] a1 = MakeArc(end, thickness / 2, angle + EdaAngle.Deg270, EdaAngle.Deg180);
            EdaPoint[] a2 = MakeArc(start, thickness / 2, angle + EdaAngle.Deg90, EdaAngle.Deg180);

            EdaPoint[] points = new EdaPoint[a1.Length + a2.Length];
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
        public static EdaPoint[] MakeArcTrace(EdaPoint center, int radius, EdaAngle startAngle, EdaAngle angle, int thickness, bool capRounded) {

            int innerRadius = radius - (thickness / 2);
            int outerRadius = radius + (thickness / 2);

            EdaPoint[] a1 = MakeArc(center, outerRadius, startAngle, angle);
            EdaPoint[] a2 = MakeArc(center, innerRadius, startAngle + angle, -angle);

            EdaPoint[] points = new EdaPoint[a1.Length + a2.Length];
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
        public static EdaPoint[] MakeCircle(EdaPoint center, int radius) {

            return MakeRegularPolygon(circleFacets, center, radius, EdaAngle.Zero);
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
        public static EdaPoint[] MakeRectangle(EdaPoint position, EdaSize size, int radius, EdaAngle rotation) {

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
            EdaPoint[] points;
            if (radius == 0) {

                points = new EdaPoint[4];
                points[0] = new EdaPoint(x + dx, y + dy);
                points[1] = new EdaPoint(x - dx, y + dy);
                points[2] = new EdaPoint(x - dx, y - dy);
                points[3] = new EdaPoint(x + dx, y - dy);
            }

            // Rectangle amb cantonades arrodonides
            //
            else {

                EdaPoint[] a1 = MakeArc(new EdaPoint(x + dx, y + dy), radius, EdaAngle.Zero, EdaAngle.Deg90);
                EdaPoint[] a2 = MakeArc(new EdaPoint(x - dx, y + dy), radius, EdaAngle.Deg90, EdaAngle.Deg90);
                EdaPoint[] a3 = MakeArc(new EdaPoint(x - dx, y - dy), radius, EdaAngle.Deg180, EdaAngle.Deg90);
                EdaPoint[] a4 = MakeArc(new EdaPoint(x + dx, y - dy), radius, EdaAngle.Deg270, EdaAngle.Deg90);

                points = new EdaPoint[a1.Length + a2.Length + a3.Length + a4.Length];
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
        public static EdaPoint[] MakeCross(EdaPoint position, EdaSize size, int thickness, EdaAngle rotation) {

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

            EdaPoint[] points = new EdaPoint[12];

            points[0] = new EdaPoint(xMax, y + dt);
            points[1] = new EdaPoint(x + dt, y + dt);
            points[2] = new EdaPoint(x + dt, yMax);

            points[3] = new EdaPoint(x - dt, yMax);
            points[4] = new EdaPoint(x - dt, y + dt);
            points[5] = new EdaPoint(xMin, y + dt);

            points[6] = new EdaPoint(xMin, y - dt);
            points[7] = new EdaPoint(x - dt, y - dt);
            points[8] = new EdaPoint(x - dt, yMin);

            points[9] = new EdaPoint(x + dt, yMin);
            points[10] = new EdaPoint(x + dt, y - dt);
            points[11] = new EdaPoint(xMax, y - dt);

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
        public static EdaPoint[] MakeRegularPolygon(int sides, EdaPoint position, int radius, EdaAngle rotation) {

            // Calcula el punt inicial
            //
            double x = radius * Math.Cos(rotation.AsRadiants);
            double y = radius * Math.Sin(rotation.AsRadiants);

            // Calcula els sinus i cosinus del gir a aplicar a cada iteracio
            //
            double angle = 2 * Math.PI / sides;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            // Crea l'array de punts
            //
            EdaPoint[] points = new EdaPoint[sides];

            // Ompla l'array amb els punts calculats.
            //
            for (int i = 0; i < sides; i++) {

                points[i] = new EdaPoint(
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
        public static EdaPoint[] MakeArc(EdaPoint center, int radius, EdaAngle startAngle, EdaAngle angle, bool discardLast = false) {

            // Calcula el numero de segments
            //
            int numSegments = (int)Math.Abs(Math.Floor((angle.AsDegrees * 32.0) / 360.0));
            int numPoints = numSegments + (discardLast ? 0 : 1);

            // Calcula l'angle de cada segment
            //
            double radSegment = angle.AsRadiants / numSegments;

            // Calcula el centre
            //
            double cx = center.X;
            double cy = center.Y;

            // Calcula el punt inicial
            //
            double x = radius * Math.Cos(startAngle.AsRadiants);
            double y = radius * Math.Sin(startAngle.AsRadiants);

            // Calcula el sinus i el cosinus del gir a aplicar a cada iteracio
            //
            double cos = Math.Cos(radSegment);
            double sin = Math.Sin(radSegment);

            // Crea l'array de punts
            //
            EdaPoint[] points = new EdaPoint[numPoints];

            for (int i = 0; i < numPoints; i++) {

                points[i] = new EdaPoint((int)(x + cx), (int)(y + cy));

                double tx = x;
                x = (cos * tx) - (sin * y);
                y = (sin * tx) + (cos * y);
            }

            return points;
        }
    }
}
