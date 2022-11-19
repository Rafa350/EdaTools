using System;
using System.ComponentModel;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Estructura que representa un punt.
    /// </summary>
    /// 
    [TypeConverter(typeof(EdaPointConverter))]
    public readonly struct EdaPoint: IEquatable<EdaPoint> {

        private readonly int _x;
        private readonly int _y;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">Coordinada X.</param>
        /// <param name="y">Coordinada Y.</param>
        /// 
        public EdaPoint(int x = 0, int y = 0) =>
            (_x, _y) = (x, y);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="other">El punt a copiar.</param>
        /// 
        public EdaPoint(EdaPoint other) => (_x, _y) =
            (other._x, other._y);

        /// <summary>
        /// Obte un punt desplaçat.
        /// </summary>
        /// <param name="dx">Desplaçament X.</param>
        /// <param name="dy">Desplaçament Y.</param>
        /// <returns>El nou punt resultant.</returns>
        /// 
        public EdaPoint Offset(int dx, int dy) =>
            new(_x + dx, _y + dy);

        /// <inheritdoc/>
        /// 
        public override int GetHashCode() =>
            HashCode.Combine(_x, _y);

        /// <inheritdoc/>
        /// 
        public override string ToString() =>
            String.Format("X: {0}; Y: {1}",
                Math.Round(_x / 1000000.0, 3),
                Math.Round(_y / 1000000.0, 3));

        /// <summary>
        /// Operacio de comparacio entre dos objectes..
        /// </summary>
        /// <param name="other">L'altre objecte a comparar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public bool Equals(EdaPoint other) =>
            (_x, _y) == (other._x, other._y);

        /// <summary>
        /// Operacio de comparacio entrer objectes
        /// </summary>
        /// <param name="obj">L'altre objecte a comparar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public override bool Equals(object obj) =>
            (obj is EdaPoint other) && Equals(other);

        /// <summary>
        /// Operador ==
        /// </summary>
        /// <param name="p1">Primer element a comparar.</param>
        /// <param name="p2">Segon element a comparar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public static bool operator ==(EdaPoint p1, EdaPoint p2) =>
            p1.Equals(p2);

        /// <summary>
        /// Operador !=
        /// </summary>
        /// <param name="p1">Primer element a comparar.</param>
        /// <param name="p2">Segon element a comparar.</param>
        /// <returns>True si son diferenmts.</returns>
        /// 
        public static bool operator !=(EdaPoint p1, EdaPoint p2) =>
            !p1.Equals(p2);

        /// <summary>
        /// Operador +
        /// </summary>
        /// <param name="p1">Primer element.</param>
        /// <param name="p2">Segon element.</param>
        /// <returns>El resultat.</returns>
        /// 
        public static EdaPoint operator +(EdaPoint p1, EdaPoint p2) =>
            new(p1.X + p2.X, p1.Y + p2.Y);

        /// <summary>
        /// Operador -
        /// </summary>
        /// <param name="p1">Primer element.</param>
        /// <param name="p2">Segon element.</param>
        /// <returns>El resultat.</returns>
        /// 
        public static EdaPoint operator -(EdaPoint p1, EdaPoint p2) =>
            new(p1.X - p2.X, p1.Y - p2.Y);

        /// <summary>
        /// Obte el valor de la coordinada X
        /// </summary>
        /// 
        public int X =>
            _x;

        /// <summary>
        /// Obte el valor de la coordinada Y
        /// </summary>
        /// 
        public int Y =>
            _y;
    }
}
