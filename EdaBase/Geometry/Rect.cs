using System;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Objecte que representa un rectangle aliniat amb els eixos X i Y.
    /// </summary>
    /// 
    public readonly struct Rect: IEquatable<Rect> {

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
        public Rect(int x = 0, int y = 0, int width = 0, int height = 0) {

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
        public Rect(Point position, Size size) {

            _x = position.X;
            _y = position.Y;
            _width = size.Width;
            _height = size.Height;
        }

        /// <summary>
        /// Obte un rectangle desplaçat.
        /// </summary>
        /// <param name="dx">Desplaçament X</param>
        /// <param name="dy">Desplaçament Y</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public Rect Offset(int dx, int dy) =>
            new Rect(_x + dx, _y + dy, _width, _height);

        /// <summary>
        /// Obte un rectangle inflat.
        /// </summary>
        /// <param name="dx">Increment X</param>
        /// <param name="dy">Increment Y</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public Rect Inflated(int dx, int dy) =>
            new Rect(_x - dx, _y - dy, _width + dx + dx, _height + dy + dy);
        
        /// <summary>
        /// Obte la unio amb un altre rectangle
        /// </summary>
        /// <param name="rect">Rectangle a unir.</param>
        /// <returns>El resultat de la unio.</returns>
        /// 
        public Rect Union(Rect rect) {

            int l = Math.Min(Left, rect.Left);
            int r = Math.Max(Right, rect.Right);
            int t = Math.Min(Top, rect.Top);
            int b = Math.Max(Bottom, rect.Bottom);

            return new Rect(l, t, r - l + 1, b - t + 1);
        }

        /// <summary>
        /// Comprova si intersecta amb un rectangle
        /// </summary>
        /// <param name="r">El rectangle a verificar.</param>
        /// <returns>True si intersecten, false en cas contrari.</returns>
        /// 
        public bool IntersectsWith(Rect r) {

            return (r.Left <= Right) && (r.Right >= Left) &&
                   (r.Bottom <= Top) && (r.Top >= Bottom);
        }

        /// <summary>
        /// Operacio de comparacio amb un altre rectangle.
        /// </summary>
        /// <param name="other">L'altre rectangle.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public bool Equals(Rect other) => 
            (_x, _y, _width, _height) == (other._x, other._y, other._width, other._height);

        /// <summary>
        /// Operacio de comparacio amb un altre objecte.
        /// </summary>
        /// <param name="obj">L'altre objecte.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public override bool Equals(object obj) =>
            (obj is Rect other) && Equals(other);

        public static bool operator ==(Rect r1, Rect r2) => 
            r1.Equals(r2); 

        public static bool operator !=(Rect r1, Rect r2) => 
            !r1.Equals(r2);

        /// <summary>
        /// Obte el codi hask de l'objecte.
        /// </summary>
        /// <returns>El codi hash</returns>
        /// 
        public override int GetHashCode() =>
            _x + (_y * 1327) + (_width * 59) + (_height * 1293);

        /// <summary>
        /// Obte la posicio del rectangle
        /// </summary>
        /// 
        public Point Position => 
            new Point(_x, _y);

        /// <summary>
        /// Obte el tamany del rectangle
        /// </summary>
        /// 
        public Size Size => 
            new Size(_width, _height);

        /// <summary>
        /// Obte la coordinada X
        /// </summary>
        /// 
        public int X => _x;

        /// <summary>
        /// Obte la coordinada Y.
        /// </summary>
        /// 
        public int Y => _y;

        /// <summary>
        /// Obte l'amplada.
        /// </summary>
        /// 
        public int Width => _width;

        /// <summary>
        /// Obte l'alçada.
        /// </summary>
        /// 
        public int Height => _height;

        public int Left => _x;

        public int Bottom => _y;

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
