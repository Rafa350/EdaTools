using System;
using System.Collections;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Gestiona una coleccio de punts.
    /// </summary>
    /// 
    public sealed class EdaPoints: IEnumerable<EdaPoint> {

        private const int _circleFacets = 32;

        private readonly List<EdaPoint> _points;
        private bool _isClosed = false;

        /// <summary>
        /// Constructor privat.
        /// </summary>
        /// 
        private EdaPoints() {

            _points = new List<EdaPoint>();
        }

        /// <summary>
        /// Contructor privat.
        /// </summary>
        /// <param name="points">Els punt inicials.</param>
        /// 
        private EdaPoints(IEnumerable<EdaPoint> points) {

            _points = new List<EdaPoint>(points);
            _isClosed = true;
        }

        /// <summary>
        /// Crea una llista buida de punts.
        /// </summary>
        /// <returns>La llista.</returns>
        /// 
        public static EdaPoints Create() {

            return new EdaPoints();
        }

        /// <summary>
        /// Crea una llista de punts.
        /// </summary>
        /// <param name="points">Els puns inicials.</param>
        /// <returns>La llista.</returns>
        /// 
        public static EdaPoints Create(IEnumerable<EdaPoint> points) {

            return new EdaPoints(points);
        }

        /// <summary>
        /// Crea una llista de punts en forma de segment de linia
        /// </summary>
        /// <param name="startposition">Posicio inicial.</param>
        /// <param name="endPosition">Posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="capRounded">True si els extrems son arrodonits.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static EdaPoints CreateLineTrace(EdaPoint startposition, EdaPoint endPosition, int thickness, bool capRounded) {

            int dx = endPosition.X - startposition.X;
            int dy = endPosition.Y - startposition.Y;
            EdaAngle angle = EdaAngle.FromRadiants(Math.Atan2(dy, dx));

            var points = new EdaPoints();
            points.AddArcPoints(endPosition, thickness / 2, angle + EdaAngle.Deg270, EdaAngle.Deg180);
            points.AddArcPoints(startposition, thickness / 2, angle + EdaAngle.Deg90, EdaAngle.Deg180);
            points.Close();

            return points;
        }

        /// <summary>
        /// Crea una llista de punts en forma de segment d'arc.
        /// </summary>
        /// <param name="center">Centre de l'arc.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="startAngle">Angle inicial.</param>
        /// <param name="angle">Angle de recoregut.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="capRounded">True si els extrems son arrodonits.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static EdaPoints CreateArcTrace(EdaPoint center, int radius, EdaAngle startAngle, EdaAngle angle, int thickness, bool capRounded) {

            int innerRadius = radius - (thickness / 2);
            int outerRadius = radius + (thickness / 2);

            int numSegments = (int)Math.Abs(Math.Floor(angle.AsDegrees * _circleFacets / 360.0));
            var points = new EdaPoints();
            points.AddArcPoints(center, outerRadius, startAngle, angle, false, numSegments);
            points.AddArcPoints(center, innerRadius, startAngle + angle, -angle, false, numSegments);
            points.Close();

            return points;
        }

        /// <summary>
        /// Crea un llista de punts en forma de rectangle. Opcionalment les
        /// cantonades poder ser arrodonides.
        /// </summary>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="size">Tamany.</param>
        /// <param name="ratio">Factor de curvatura/planitut.</param>
        /// <param name="rounded">Indica si les cantonades son arrodonides.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static EdaPoints CreateRectangle(EdaPoint position, EdaSize size, EdaRatio ratio, bool rounded, EdaAngle rotation) {

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

            var points = new EdaPoints();

            if (ratio.IsZero) {
                points.AddPoint(new EdaPoint(x4, y4));
                points.AddPoint(new EdaPoint(x1, y4));
                points.AddPoint(new EdaPoint(x1, y1));
                points.AddPoint(new EdaPoint(x4, y1));
            }
            else {
                if (rounded)
                    points.AddArcPoints(new EdaPoint(x3, y3), r, EdaAngle.Zero, EdaAngle.Deg90);
                else {
                    points.AddPoint(new EdaPoint(x4, y3));
                    points.AddPoint(new EdaPoint(x3, y4));
                }

                if (rounded)
                    points.AddArcPoints(new EdaPoint(x2, y3), r, EdaAngle.Deg90, EdaAngle.Deg90);
                else {
                    points.AddPoint(new EdaPoint(x2, y4));
                    points.AddPoint(new EdaPoint(x1, y3));
                }

                if (rounded)
                    points.AddArcPoints(new EdaPoint(x2, y2), r, EdaAngle.Deg180, EdaAngle.Deg90);
                else {
                    points.AddPoint(new EdaPoint(x1, y2));
                    points.AddPoint(new EdaPoint(x2, y1));
                }

                if (rounded)
                    points.AddArcPoints(new EdaPoint(x3, y2), r, EdaAngle.Deg270, EdaAngle.Deg90);
                else {
                    points.AddPoint(new EdaPoint(x3, y1));
                    points.AddPoint(new EdaPoint(x4, y2));
                }
            }
            points.Close();

            if (!rotation.IsZero)
                points.Rotate(position, rotation);

            return points;
        }

        /// <summary>
        /// Crea una llista de punts en forma de cercle.
        /// </summary>
        /// <param name="center">Posicio del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <param name="facets">Nombre de cares per definit el cercle.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        public static EdaPoints CreateCircle(EdaPoint center, int radius, int facets = _circleFacets) {

            return CreatePolygon(facets, center, radius, EdaAngle.Zero);
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
        public static EdaPoints CreateCross(EdaPoint position, EdaSize size, int thickness, EdaAngle rotation) {

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

            var points = new EdaPoints();

            points.AddPoint(new EdaPoint(x4, y3));
            points.AddPoint(new EdaPoint(x3, y3));
            points.AddPoint(new EdaPoint(x3, y4));
            points.AddPoint(new EdaPoint(x2, y4));
            points.AddPoint(new EdaPoint(x2, y3));
            points.AddPoint(new EdaPoint(x1, y3));
            points.AddPoint(new EdaPoint(x1, y2));
            points.AddPoint(new EdaPoint(x2, y2));
            points.AddPoint(new EdaPoint(x2, y1));
            points.AddPoint(new EdaPoint(x3, y1));
            points.AddPoint(new EdaPoint(x3, y2));
            points.AddPoint(new EdaPoint(x4, y2));
            points.Close();

            if (!rotation.IsZero)
                points.Rotate(position, rotation);

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
        public static EdaPoints CreatePolygon(int sides, EdaPoint position, int radius, EdaAngle rotation) {

            var points = new EdaPoints();

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

                points.AddPoint(new EdaPoint(
                    (int)(x + position.X),
                    (int)(y + position.Y)));

                double tx = x;
                x = (cos * tx) - (sin * y);
                y = (sin * tx) + (cos * y);
            }

            points.Close();

            return points;
        }

        /// <summary>
        /// Tanca la llista i impedeix afeigir mes punts.
        /// </summary>
        /// <returns>El propi objecte.</returns>
        /// 
        public EdaPoints Close() {

            _isClosed = true;

            return this;
        }

        /// <summary>
        /// Afegeix un punt.
        /// </summary>
        /// <param name="point">El punt.</param>
        /// <returns>El propi objecte.</returns>
        /// 
        public EdaPoints AddPoint(EdaPoint point) {

            if (_isClosed)
                throw new InvalidOperationException("Imposible agregar mas puntos, la lista esta cerrada.");

            _points.Add(point);

            return this;
        }

        /// <summary>
        /// Afegeix una serie de punts.
        /// </summary>
        /// <param name="points">Els punts a afeigir.</param>
        /// <returns>El propi objecte.</returns>
        /// 
        public EdaPoints AddPoints(IEnumerable<EdaPoint> points) {

            if (_isClosed)
                throw new InvalidOperationException("Imposible agregar mas puntos, la lista esta cerrada.");

            _points.AddRange(points);

            return this;
        }

        /// <summary>
        /// Afegeix punts per formar un arc.
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
        public EdaPoints AddArcPoints(EdaPoint center, int radius, EdaAngle startAngle, EdaAngle angle, bool discardLast = false, int numSegments = 0) {

            if (radius <= 0)
                throw new ArgumentOutOfRangeException(nameof(radius));

            if (numSegments < 0)
                throw new ArgumentOutOfRangeException(nameof(numSegments));

            if (_isClosed)
                throw new InvalidOperationException("Imposible agregar mas puntos, la lista esta cerrada.");

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
            for (int i = 0; i < numPoints; i++) {

                _points.Add(new EdaPoint((int)(x + cx), (int)(y + cy)));

                double tx = x;
                x = (cos * tx) - (sin * y);
                y = (sin * tx) + (cos * y);
            }

            return this;
        }

        /// <summary>
        /// Aplica una transformacio a la llista.
        /// </summary>
        /// <param name="transformation">La transformacio.</param>
        /// 
        public void Transform(Transformation transformation) {

            for (int i = 0; i < _points.Count; i++)
                _points[i] = transformation.Transform(_points[i]);
        }

        /// <summary>
        /// Rota la llista de punts.
        /// </summary>
        /// <param name="center">Centre de rotacio.</param>
        /// <param name="angle">Angle de rotacio.</param>
        /// 
        private void Rotate(EdaPoint center, EdaAngle angle) {

            var t = new Transformation();
            t.Rotate(center, angle);
            Transform(t);
        }

        /// <summary>
        /// Obte el enumerador de la coleccio.
        /// </summary>
        /// <returns>El enumerador.</returns>
        /// 
        public IEnumerator<EdaPoint> GetEnumerator() =>
            _points.GetEnumerator();

        /// <summary>
        /// Obte el enumerador de la coleccio.
        /// </summary>
        /// <returns>El enumerador.</returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator() =>
            _points.GetEnumerator();

        public EdaPoint[] ToArray() =>
            _points.ToArray();

        /// <summary>
        /// El nombre d'elements en la coleccio.
        /// </summary>
        /// 
        public int Count =>
            _points.Count;

        /// <summary>
        /// El element per index.
        /// </summary>
        /// <param name="index">El index.</param>
        /// <returns>El element.</returns>
        /// 
        public EdaPoint this[int index] {
            get => _points[index];
            set => _points[index] = value;
        }
    }
}
