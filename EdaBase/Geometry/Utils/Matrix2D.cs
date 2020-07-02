namespace MikroPic.EdaTools.v1.Base.Geometry.Utils {

    using System;

    /// <summary>
    /// Estructura que representa una matriu per transformacions afins en 2D.
    /// </summary>
    /// <remarks>Basat en Matrix de AvaloniaUI.</remarks>
    /// 
    public readonly struct Matrix2D : IEquatable<Matrix2D> {

        private readonly double _m11;
        private readonly double _m12;
        private readonly double _m21;
        private readonly double _m22;
        private readonly double _tx;
        private readonly double _ty;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="m11">Parametre m11</param>
        /// <param name="m12">Parametre m12</param>
        /// <param name="m21">Parametre m21</param>
        /// <param name="m22">Parametre m22</param>
        /// <param name="tx">Parametre tx</param>
        /// <param name="ty">Parametre ty</param>
        /// 
        public Matrix2D(double m11, double m12, double m21, double m22, double tx, double ty) {

            _m11 = m11;
            _m12 = m12;
            _m21 = m21;
            _m22 = m22;
            _tx = tx;
            _ty = ty;
        }

        /// <summary>
        /// Crea una matriu identitat.
        /// </summary>
        /// <returns>La matriu.</returns>
        /// 
        public static Matrix2D CreateIdentity() => 
            new Matrix2D(1, 0, 0, 1, 0, 0);

        /// <summary>
        /// Crea una matriu de translacio.
        /// </summary>
        /// <param name="tx">Translacio en l'eix X.</param>
        /// <param name="ty">Translacio en l'eix Y.</param>
        /// <returns>La matriu.</returns>
        /// 
        public static Matrix2D CreateTranslation(double tx, double ty) => 
            new Matrix2D(1, 0, 0, 1, tx, ty);

        /// <summary>
        /// Crea una matriu de escalat.
        /// </summary>
        /// <param name="sx">Factor d'escala X.</param>
        /// <param name="sy">Factor d'escala Y.</param>
        /// <returns>La matriu.</returns>
        /// 
        public static Matrix2D CreateScale(double sx, double sy) => 
            new Matrix2D(sx, 0, 0, sy, 0, 0);

        /// <summary>
        /// Crea una matriu d'escalat.
        /// </summary>
        /// <param name="sx">Factor d'escala X.</param>
        /// <param name="sy">Factor d'escala Y.</param>
        /// <param name="ox">Origen X del escalat.</param>
        /// <param name="oy">Origen Y del escalat.</param>
        /// <returns>La matriu.</returns>
        /// 
        public static Matrix2D CreateScale(double sx, double sy, double ox, double oy) => 
            new Matrix2D(sx, 0, 0, sy, ox - (sx * ox), oy - (sy * oy));

        /// <summary>
        /// Crea una matriu de rotacio.
        /// </summary>
        /// <param name="a">Angle de rotacio en graus.</param>
        /// <returns>La matriu.</returns>
        /// 
        public static Matrix2D CreateRotation(double a) {

            double rad = (a % 360.0) * Math.PI / 180.0;
            double sin = Math.Sin(rad);
            double cos = Math.Cos(rad);

            return new Matrix2D(cos, sin, -sin, cos, 0, 0);
        }

        /// <summary>
        /// Crea una matriu de rotacio.
        /// </summary>
        /// <param name="a">Angle de rotacio en graus.</param>
        /// <param name="ox">Coordinada X del centre de rotacio.</param>
        /// <param name="oy">Coordinada Y del centre de rotacio.</param>
        /// <returns>La matriu.</returns>
        /// 
        public static Matrix2D CreateRotation(double a, double ox, double oy) {

            double rad = (a % 360.0) * Math.PI / 180.0;
            double sin = Math.Sin(rad);
            double cos = Math.Cos(rad);
            double tx = (ox * (1.0 - cos)) + (oy * sin);
            double ty = (oy * (1.0 - cos)) - (ox * sin);

            return new Matrix2D(cos, sin, -sin, cos, tx, ty);
        }

        /// <summary>
        /// Calculates the determinant for this matrix.
        /// </summary>
        /// <returns>The determinant.</returns>
        /// <remarks>
        /// The determinant is calculated by expanding the matrix with a third column whose
        /// values are (0,0,1).
        /// </remarks>
        /// 
        public double GetDeterminant() => 
            (_m11 * _m22) - (_m12 * _m21);

        /// <summary>
        /// Inverts the Matrix.
        /// </summary>
        /// <returns>The inverted matrix.</returns>
        /// 
        public Matrix2D Invert() {

            double d = GetDeterminant();
            if (d == 0)
                throw new InvalidOperationException("Transform is not invertible.");

            return new Matrix2D(
                _m22 / d,
                -_m12 / d,
                -_m21 / d,
                _m11 / d,
                ((_m21 * _ty) - (_m22 * _tx)) / d,
                ((_m12 * _tx) - (_m11 * _ty)) / d);
        }

        /// <summary>
        /// Multiplies two matrices together and returns the resulting matrix.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>The product matrix.</returns>
        /// 
        public static Matrix2D operator *(Matrix2D value1, Matrix2D value2) =>
            new Matrix2D(
                (value1._m11 * value2._m11) + (value1._m12 * value2._m21),
                (value1._m11 * value2._m12) + (value1._m12 * value2._m22),
                (value1._m21 * value2._m11) + (value1._m22 * value2._m21),
                (value1._m21 * value2._m12) + (value1._m22 * value2._m22),
                (value1._tx * value2._m11) + (value1._ty * value2._m21) + value2._tx,
                (value1._tx * value2._m12) + (value1._ty * value2._m22) + value2._ty);

        /// <summary>
        /// Negates the given matrix by multiplying all values by -1.
        /// </summary>
        /// <param name="value">The source matrix.</param>
        /// <returns>The negated matrix.</returns>
        /// 
        public static Matrix2D operator -(Matrix2D value) => 
            value.Invert();

        /// <summary>
        /// Returns a boolean indicating whether the given matrices are equal.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>True if the matrices are equal; False otherwise.</returns>
        /// 
        public static bool operator ==(Matrix2D value1, Matrix2D value2) => 
            value1.Equals(value2);

        /// <summary>
        /// Returns a boolean indicating whether the given matrices are not equal.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>True if the matrices are not equal; False if they are equal.</returns>
        /// 
        public static bool operator !=(Matrix2D value1, Matrix2D value2) => 
            !value1.Equals(value2);

        /// <summary>
        /// Operacio de comparacio entre dos objectes.
        /// </summary>
        /// <param name="other">L'altre objecte a comparar.</param>
        /// <returns>True si son iguals.</returns>
        /// 
        public bool Equals(Matrix2D other) =>
            (_m11, _m12, _m21, _m22, _tx, _ty) == (other._m11, other._m12, other._m21, other._m22, other._tx, other._ty);

        /// <summary>
        /// Returns a boolean indicating whether the given Object is equal to this matrix instance.
        /// </summary>
        /// <param name="obj">The Object to compare against.</param>
        /// <returns>True if the Object is equal to this matrix; False otherwise.</returns>
        /// 
        public override bool Equals(object obj) => 
            obj is Matrix2D other && Equals(other);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        /// 
        public override int GetHashCode() =>
            _m11.GetHashCode() + _m12.GetHashCode() + _m21.GetHashCode() + _m22.GetHashCode() +
            _tx.GetHashCode() + _ty.GetHashCode();

        /// <summary>
        /// Obte una matriu identitat.
        /// </summary>
        /// 
        public static Matrix2D Identity => 
            CreateIdentity();

        public double M11 => _m11;

        public double M12 => _m12;

        public double M21 => _m21;

        public double M22 => _m22;

        public double Tx => _tx;

        public double Ty => _ty;
    }
}
