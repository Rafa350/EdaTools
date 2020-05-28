namespace MikroPic.EdaTools.v1.Base.Geometry {

    using System;

    /// <summary>
    /// Objecte que representa un rectangle aliniat amb els eixos X i Y.
    /// </summary>
    /// 
    public readonly struct Rect: IEquatable<Rect> {

        private readonly int x;
        private readonly int y;
        private readonly int width;
        private readonly int height;

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

            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio.</param>
        /// <param name="size">Tamany</param>
        /// 
        public Rect(Point position, Size size) {

            x = position.X;
            y = position.Y;
            width = size.Width;
            height = size.Height;
        }

        /// <summary>
        /// Obte un rectangle desplaçat.
        /// </summary>
        /// <param name="dx">Desplaçament X</param>
        /// <param name="dy">Desplaçament Y</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public Rect Offset(int dx, int dy) {

            return new Rect(x + dx, y + dy, width, height);
        }

        /// <summary>
        /// Obte un rectangle inflat.
        /// </summary>
        /// <param name="dx">Increment X</param>
        /// <param name="dy">Increment Y</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public Rect Inflated(int dx, int dy) {

            return new Rect(x - dx, y - dy, width + dx + dx, height + dy + dy);
        }

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
            (x, y, width, height) == (other.x, other.y, other.width, other.height);

        /// <summary>
        /// Operacio de comparacio amb un altre objecte.
        /// </summary>
        /// <param name="obj">L'altre objecte.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public override bool Equals(object obj) {

            if (obj is Rect other)
                return Equals(other);
            else
                return false;
        }

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
            x + (y * 1327) + (width * 59) + (height * 1293);

        /// <summary>
        /// Obte la posicio del rectangle
        /// </summary>
        /// 
        public Point Position => 
            new Point(x, y);

        /// <summary>
        /// Obte el tamany del rectangle
        /// </summary>
        /// 
        public Size Size => 
            new Size(width, height);

        /// <summary>
        /// Obte la coordinada X
        /// </summary>
        /// 
        public int X => x;

        /// <summary>
        /// Obte la coordinada Y.
        /// </summary>
        /// 
        public int Y => y;

        /// <summary>
        /// Obte l'amplada.
        /// </summary>
        /// 
        public int Width => width;

        /// <summary>
        /// Obte l'alçada.
        /// </summary>
        /// 
        public int Height => height;

        public int Left => x;

        public int Bottom => y;

        public int Right => 
            x + width - 1;

        public int Top => 
            y + height - 1;

        /// <summary>
        /// Comprova si el rectangle es buit.
        /// </summary>
        /// 
        public bool IsEmpty =>
            (width == 0) && (height == 0);
    }
}
