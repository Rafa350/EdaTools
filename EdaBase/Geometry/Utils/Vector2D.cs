using System;

namespace MikroPic.EdaTools.v1.Base.Geometry.Utils {

    /// <summary>
    /// Estructura que representa un vector 2D
    /// </summary>
    /// 
    public readonly struct Vector2D {

        private readonly double _x;
        private readonly double _y;

        /// <summary>
        /// Controctor.
        /// </summary>
        /// <param name="x">Coordinada X.</param>
        /// <param name="y">Coordinada Y.</param>
        /// 
        public Vector2D(double x, double y) {

            _x = x;
            _y = y;
        }

        /// <summary>
        /// Converteix l'objecte a text.
        /// </summary>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public override string ToString() =>
            String.Format("{0}, {1}", _x, _y);

        /// <summary>
        /// Multiplica el vectgor per una matriu.
        /// </summary>
        /// <param name="v">El vector.</param>
        /// <param name="m">La matriu.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static Vector2D operator *(Vector2D v, Matrix2D m) {

            double x = (v.X * m.M11) + (v.Y * m.M21) + m.Tx;
            double y = (v.X * m.M12) + (v.Y * m.M22) + m.Ty;

            return new Vector2D(x, y);
        }

        public double X =>
            _x;

        public double Y =>
            _y;
    }
}
