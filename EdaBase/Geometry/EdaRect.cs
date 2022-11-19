using System;
using System.ComponentModel;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Objecte que representa un rectangle aliniat amb els eixos X i Y.
    /// </summary>
    /// 
    [TypeConverter(typeof(EdaRectConverter))]
    public readonly struct EdaRect: IEquatable<EdaRect> {

        private readonly int _x;
        private readonly int _y;
        private readonly int _width;
        private readonly int _height;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="x">Coordinada X</param>
        /// <param name="y">Coordinada Y</param>
        /// <param name="width">Amplada</param>
        /// <param name="height">Alçada</param>
        /// 
        public EdaRect(int x = 0, int y = 0, int width = 0, int height = 0) {

            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio.</param>
        /// <param name="size">Tamany</param>
        /// 
        public EdaRect(EdaPoint position, EdaSize size) {

            _x = position.X;
            _y = position.Y;
            _width = size.Width;
            _height = size.Height;
        }

        /// <summary>
        /// Operacio de comparacio amb un altre rectangle.
        /// </summary>
        /// <param name="other">L'altre rectangle.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public bool Equals(EdaRect other) =>
            (_x, _y, _width, _height) == (other._x, other._y, other._width, other._height);

        /// <summary>
        /// Operacio de comparacio amb un altre objecte.
        /// </summary>
        /// <param name="obj">L'altre objecte.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public override bool Equals(object obj) =>
            (obj is EdaRect other) && Equals(other);

        public static bool operator ==(EdaRect r1, EdaRect r2) =>
            r1.Equals(r2);

        public static bool operator !=(EdaRect r1, EdaRect r2) =>
            !r1.Equals(r2);

        /// <inheritdoc/>
        /// 
        public override string ToString() =>
            String.Format("X: {0}; Y:{1}; W:{2}; H:{3}",
                Math.Round(_x / 1000000.0, 3),
                Math.Round(_y / 1000000.0, 3),
                Math.Round(_width / 1000000.0, 3),
                Math.Round(_height / 1000000.0, 3));

        /// <summary>
        /// Obte el codi hask de l'objecte.
        /// </summary>
        /// <returns>El codi hash</returns>
        /// 
        public override int GetHashCode() =>
            HashCode.Combine(_x, _y, _width, _height);

        /// <summary>
        /// Obte la posicio del rectangle
        /// </summary>
        /// 
        public EdaPoint Position =>
            new(_x, _y);

        /// <summary>
        /// Obte el tamany del rectangle
        /// </summary>
        /// 
        public EdaSize Size =>
            new(_width, _height);

        /// <summary>
        /// Obte la coordinada X
        /// </summary>
        /// 
        public int X =>
            _x;

        /// <summary>
        /// Obte la coordinada Y.
        /// </summary>
        /// 
        public int Y =>
            _y;

        /// <summary>
        /// Obte l'amplada.
        /// </summary>
        /// 
        public int Width =>
            _width;

        /// <summary>
        /// Obte l'alçada.
        /// </summary>
        /// 
        public int Height =>
            _height;

        public int Left =>
            _x;

        public int Bottom =>
            _y;

        public int Right =>
            _x + _width - 1;

        public int Top =>
            _y + _height - 1;

        /// <summary>
        /// Comprova si el rectangle es buit.
        /// </summary>
        /// 
        public bool IsEmpty =>
            (_width == 0) && (_height == 0);
    }
}
