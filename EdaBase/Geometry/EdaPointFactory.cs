using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    public static class EdaPointFactory {

        private const int _circleFacets = 32;

        /// <summary>
        /// Obte una llista de punts en forma de segment de linia
        /// </summary>
        /// <param name="startposition">Posicio inicial.</param>
        /// <param name="endPosition">Posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="capRounded">True si els extrems son arrodonits.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static IEnumerable<EdaPoint> CreateLineTrace(EdaPoint startposition, EdaPoint endPosition, int thickness, bool capRounded) {

            int dx = endPosition.X - startposition.X;
            int dy = endPosition.Y - startposition.Y;
            EdaAngle angle = EdaAngle.FromRadiants(Math.Atan2(dy, dx));

            var points = new List<EdaPoint>();
            points.AddRange(CreateArc(endPosition, thickness / 2, angle + EdaAngle.Deg270, EdaAngle.Deg180));
            points.AddRange(CreateArc(startposition, thickness / 2, angle + EdaAngle.Deg90, EdaAngle.Deg180));

            return points;
        }

        /// <summary>
        /// Obte una llista de punts en forma de segment d'arc.
        /// </summary>
        /// <param name="center">Centre de l'arc.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="startAngle">Angle inicial.</param>
        /// <param name="angle">Angle de recoregut.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="capRounded">True si els extrems son arrodonits.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static IEnumerable<EdaPoint> CreateArcTrace(EdaPoint center, int radius, EdaAngle startAngle, EdaAngle angle, int thickness, bool capRounded) {

            int innerRadius = radius - (thickness / 2);
            int outerRadius = radius + (thickness / 2);

            int numSegments = (int)Math.Abs(Math.Floor(angle.AsDegrees * _circleFacets / 360.0));
            var points = new List<EdaPoint>();
            points.AddRange(CreateArc(center, outerRadius, startAngle, angle, false, numSegments));
            points.AddRange(CreateArc(center, innerRadius, startAngle + angle, -angle, false, numSegments));

            return points;
        }

        /// <summary>
        /// Obte un llista de punts en forma de rectangle. Opcionalment les
        /// cantonades poder ser arrodonides.
        /// </summary>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="size">Tamany.</param>
        /// <param name="ratio">Factor de curvatura/planitut.</param>
        /// <param name="rounded">Indica si les cantonades son arrodonides.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static IEnumerable<EdaPoint> CreateRectangle(EdaPoint position, EdaSize size, EdaRatio ratio, bool rounded, EdaAngle rotation) {

            int x = position.X;
            int y = position.Y;
            int r = (Math.Min(size.Width, size.Height) / 2) * ratio;

            int hw = size.Width / 2;
            int hh = size.Height / 2;

            int x1 = x - hw;
            int x2 = x - hw + r;
            int x3 = x + hw - r;
            int x4 = x + hw;

            int y1 = y - hh;
            int y2 = y - hh + r;
            int y3 = y + hh - r;
            int y4 = y + hh;

            var points = new List<EdaPoint>();

            if (ratio.IsZero) {
                points.Add(new EdaPoint(x4, y4));
                points.Add(new EdaPoint(x1, y4));
                points.Add(new EdaPoint(x1, y1));
                points.Add(new EdaPoint(x4, y1));
            }
            else {
                if (rounded)
                    points.AddRange(CreateArc(new EdaPoint(x3, y3), r, EdaAngle.Zero, EdaAngle.Deg90));
                else {
                    points.Add(new EdaPoint(x4, y3));
                    points.Add(new EdaPoint(x3, y4));
                }

                if (rounded)
                    points.AddRange(CreateArc(new EdaPoint(x2, y3), r, EdaAngle.Deg90, EdaAngle.Deg90));
                else {
                    points.Add(new EdaPoint(x2, y4));
                    points.Add(new EdaPoint(x1, y3));
                }

                if (rounded)
                    points.AddRange(CreateArc(new EdaPoint(x2, y2), r, EdaAngle.Deg180, EdaAngle.Deg90));
                else {
                    points.Add(new EdaPoint(x1, y2));
                    points.Add(new EdaPoint(x2, y1));
                }

                if (rounded)
                    points.AddRange(CreateArc(new EdaPoint(x3, y2), r, EdaAngle.Deg270, EdaAngle.Deg90));
                else {
                    points.Add(new EdaPoint(x3, y1));
                    points.Add(new EdaPoint(x4, y2));
                }
            }

            if (rotation.IsZero)
                return points;

            else {
                var t = new EdaTransformation();
                t.Rotate(position, rotation);
                return t.Transform(points);
            }
        }

        /// <summary>
        /// Obte una llista de punts en forma de cercle.
        /// </summary>
        /// <param name="center">Posicio del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <param name="facets">Nombre de cares per definit el cercle.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static IEnumerable<EdaPoint> CreateCircle(EdaPoint center, int radius) {

            return CreatePolygon(_circleFacets, center, radius, EdaAngle.Zero);
        }

        /// <summary>
        /// Obte una llista de punts en forma de creu.
        /// </summary>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="size">Tamany del rectangle exterior.</param>
        /// <param name="thickness">Amplada dels braços</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static IEnumerable<EdaPoint> CreateCross(EdaPoint position, EdaSize size, int thickness, EdaAngle rotation) {

            int x = position.X;
            int y = position.Y;

            int hw = size.Width / 2;
            int hh = size.Height / 2;
            int ht = thickness / 2;

            int x1 = x - hw;
            int x2 = x - ht;
            int x3 = x + ht;
            int x4 = x + hw;

            int y1 = y - hh;
            int y2 = y - ht;
            int y3 = y + ht;
            int y4 = y + hh;

            var points = new List<EdaPoint>();

            points.Add(new EdaPoint(x4, y3));
            points.Add(new EdaPoint(x3, y3));
            points.Add(new EdaPoint(x3, y4));
            points.Add(new EdaPoint(x2, y4));
            points.Add(new EdaPoint(x2, y3));
            points.Add(new EdaPoint(x1, y3));
            points.Add(new EdaPoint(x1, y2));
            points.Add(new EdaPoint(x2, y2));
            points.Add(new EdaPoint(x2, y1));
            points.Add(new EdaPoint(x3, y1));
            points.Add(new EdaPoint(x3, y2));
            points.Add(new EdaPoint(x4, y2));

            if (rotation.IsZero)
                return points;

            else {
                var t = new EdaTransformation();
                t.Rotate(position, rotation);
                return t.Transform(points);
            }
        }

        /// <summary>
        /// Obte una llista de punts per un poligon regular. El primer punt
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
        public static IEnumerable<EdaPoint> CreatePolygon(int sides, EdaPoint position, int radius, EdaAngle rotation) {

            var points = new List<EdaPoint>();

            // Calcula el punt inicial
            //
            double x = radius * Math.Cos(rotation.AsRadiants);
            double y = radius * Math.Sin(rotation.AsRadiants);

            // Calcula els sinus i cosinus del gir a aplicar a cada iteracio
            //
            double angle = 2 * Math.PI / sides;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            // Ompla l'array amb els punts calculats.
            //
            for (int i = 0; i < sides; i++) {

                points.Add(new EdaPoint(
                    (int)(x + position.X),
                    (int)(y + position.Y)));

                double tx = x;
                x = (cos * tx) - (sin * y);
                y = (sin * tx) + (cos * y);
            }

            return points;
        }

        /// <summary>
        /// Obte els punts per formar un arc.
        /// </summary>
        /// <param name="center">El centre.</param>
        /// <param name="radius">El radi.</param>
        /// <param name="startAngle">Angle inicial.</param>
        /// <param name="angle">Angle del arc.</param>
        /// <param name="discardLast">Descarta l'ultim punt.</param>
        /// <param name="numSegments">Nombre se segments del arc. Si es zero el calcula</param>
        /// <returns>El propi objecte.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Arguments fora de rang.</exception>
        /// 
        public static IEnumerable<EdaPoint> CreateArc(EdaPoint center, int radius, EdaAngle startAngle, EdaAngle angle, bool discardLast = false, int numSegments = 0) {

            if (radius <= 0)
                throw new ArgumentOutOfRangeException(nameof(radius));

            if (numSegments < 0)
                throw new ArgumentOutOfRangeException(nameof(numSegments));

            // Calcula el nombre de segments
            //
            if (numSegments == 0)
                numSegments = (int)Math.Abs(Math.Floor(angle.AsDegrees * _circleFacets / 360.0));

            // Calcula el numero de punts
            //
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
            var points = new List<EdaPoint>();
            for (int i = 0; i < numPoints; i++) {

                points.Add(new EdaPoint((int)(x + cx), (int)(y + cy)));

                double tx = x;
                x = (cos * tx) - (sin * y);
                y = (sin * tx) + (cos * y);
            }

            return points;
        }
    }
}
