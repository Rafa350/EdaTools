namespace MikroPic.EdaTools.v1.Base.Geometry.Utils {

    using System;

    public struct Matrix2D {

        [Flags]
        private enum MatrixType {
            Identity = 0,
            Translation = 1,
            Scale = 2,
            Unknown = 4
        }

        private double[,] m;
        private MatrixType type;

        /// <summary>
        /// Afegeix una operacio de translacio.
        /// </summary>
        /// <param name="tx">Coordinada X de la traslacio.</param>
        /// <param name="ty">Coordinada Y de la traslacio.</param>
        /// 
        public void Translate(double tx, double ty) {

            switch (type) {

                case MatrixType.Identity: {
                    m[2, 0] = tx;
                    m[2, 1] = ty;
                    type = MatrixType.Translation;
                    break;
                }

                case MatrixType.Unknown: {
                    m[2, 0] += tx;
                    m[2, 1] += ty;
                    break;
                }

                default: {
                    m[2, 0] += tx;
                    m[2, 1] += ty;
                    type |= MatrixType.Translation;
                    break;
                }
            }
        }

        /// <summary>
        /// Afegeix una operacio d'escalat.
        /// </summary>
        /// <param name="sx">Factor d'escala X.</param>
        /// <param name="sy">Factor d'escala Y.</param>
        /// 
        public void Scale(double sx, double sy) {

            ScaleAt(sx, sy, 0, 0);
        }

        /// <summary>
        /// Afegeix una operacio d'escalat.
        /// </summary>
        /// <param name="sx">Factor d'escala X.</param>
        /// <param name="sy">Factor d'escala Y.</param>
        /// <param name="ox">Coordinada X de l'origen.</param>
        /// <param name="oy">Coordinada Y de l'origen.</param>
        /// 
        public void ScaleAt(double sx, double sy, double ox, double oy) {

            double[,] sm = new double[3, 3];

            sm[0, 0] = sx;
            sm[0, 1] = 0;
            sm[0, 2] = 0;

            sm[1, 0] = 0;
            sm[1, 1] = sy;
            sm[1, 2] = 0;

            if (ox == 0) // Optimitza el cas ox == 0
                sm[2, 0] = 0;
            else
                sm[2, 0] = (1 - sx) * ox;
            if (oy == 0) // Optimitza el cas oy == 0
                sm[2, 1] = 0;
            else
                sm[2, 1] = (1 - sy) * oy;
            sm[2, 2] = 1;

            CombineMatrix(sm, MatrixType.Scale);
        }

        /// <summary>
        /// Afegeix una operacio de rotacio.
        /// </summary>
        /// <param name="a">Angle de rotacio.</param>
        /// 
        public void Rotate(double a) {

            RotateAt(a, 0, 0);
        }

        /// <summary>
        /// Afegeix una operacio de rotacio.
        /// </summary>
        /// <param name="a">Angle de rotacio.</param>
        /// <param name="ox">Coordinada X de l'origen.</param>
        /// <param name="oy">Coordinada Y de l'origen.</param>
        /// 
        public void RotateAt(double a, double ox, double oy) {

            double[,] rm = new double[3, 3];

            double rad = (a % 360.0) * Math.PI / 180.0;
            double sin = Math.Sin(rad);
            double cos = Math.Cos(rad);

            rm[0, 0] = cos;
            rm[0, 1] = sin;
            rm[0, 2] = 0;

            rm[1, 0] = -sin;
            rm[1, 1] = cos;
            rm[1, 2] = 0;

            if ((ox == 0) && (oy == 0)) {
                rm[2, 0] = 0;
                rm[2, 1] = 0;
            }
            else {
                rm[2, 0] = ox * (1 - cos) + oy * sin;
                rm[2, 1] = oy * (1 - cos) - ox * sin;
            }
            rm[2, 2] = 1;

            CombineMatrix(rm, MatrixType.Unknown);
        }


        public void Multiply(Matrix2D matrix) {

            if (matrix.type == MatrixType.Identity)
                return;

            if (type == MatrixType.Identity) {
                m = matrix.m;
                return;
            }

            CombineMatrix(matrix.m, matrix.type);
        }

        /// <summary>
        /// Aplica la transformacio a un parell de valors X, Y
        /// </summary>
        /// <param name="x">Coordinada X.</param>
        /// <param name="y">Coordinada Y.</param>
        /// 
        public void Apply(ref double x, ref double y) {

            switch (type) {
                
                case MatrixType.Identity:
                    break;

                case MatrixType.Translation:
                    x += m[2, 0];
                    y += m[2, 1];
                    break;

                case MatrixType.Scale:
                    x *= m[0, 0];
                    y *= m[1, 1];
                    break;

                case MatrixType.Translation | MatrixType.Scale:
                    x *= m[0, 0];
                    x += m[2, 0];
                    y *= m[1, 1];
                    y += m[2, 1];
                    break;

                default: {
                    double xx = x;
                    x = xx * m[0, 0] + y * m[1, 0] + m[2, 0];
                    y = xx * m[0, 1] + y * m[1, 1] + m[2, 1];
                    break;
                }
            }
        }

        /// <summary>
        /// Combina amb un altre matriu.
        /// </summary>
        /// <param name="cm">La matriu per combinar.</param>
        /// <param name="ct">Tipus de matriu a combinar.</param>
        /// 
        private void CombineMatrix(double[,] cm, MatrixType ct) {

            double[,] rm = new double[3, 3];

            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++) {
                    double sum = 0;
                    for (int z = 0; z < 3; z++)
                        sum += m[r, z] * cm[z, c];
                    rm[r, c] = sum;
                }

            m = rm;

            switch (type) {
                case MatrixType.Identity:
                    type = ct;
                    break;

                case MatrixType.Translation:
                case MatrixType.Scale:
                case MatrixType.Translation | MatrixType.Scale:
                    if (ct == MatrixType.Unknown)
                        type = MatrixType.Unknown;
                    else
                        type |= ct;
                    break;

                case MatrixType.Unknown:
                    break;
            }
        }

        /// <summary>
        /// Asigna els parametres de la matriu.
        /// </summary>
        /// <param name="m11">Parametre m11.</param>
        /// <param name="m12">Parametre m12.</param>
        /// <param name="m21">Parametre m21.</param>
        /// <param name="m22">Parametre m22.</param>
        /// <param name="tx">Parametre de translacio X.</param>
        /// <param name="ty">Parametre de traslacio Y.</param>
        /// <param name="type">Tipus de matriu.</param>
        /// 
        private void SetMatrix(double m11, double m12, double m21, double m22, double tx, double ty, MatrixType type) {

            m = new double[3, 3];

            m[0, 0] = m11;
            m[0, 1] = m12;
            m[0, 2] = 0;

            m[1, 0] = m21;
            m[1, 1] = m22;
            m[1, 2] = 0;

            m[2, 0] = tx;
            m[2, 1] = ty;
            m[2, 2] = 1;

            this.type = type;
        }

        /// <summary>
        /// Crea una matriu identitat.
        /// </summary>
        /// <returns>La matriu.</returns>
        /// 
        private static Matrix2D CreateIdentity() {

            Matrix2D m = new Matrix2D();
            m.SetMatrix(1, 0, 0, 1, 0, 0, MatrixType.Identity);

            return m;
        }

        /// <summary>
        /// Crea una matriu d'escalat.
        /// </summary>
        /// <param name="sx">Factor d'escala X.</param>
        /// <param name="sy">Factor d'escala Y.</param>
        /// <param name="ox">Coordinada X de l'origen.</param>
        /// <param name="oy">Coordinada Y de l'origen.</param>
        /// <returns>La matriu.</returns>
        /// 
        private static Matrix2D CreateScale(double sx, double sy, double ox, double oy) {

            Matrix2D m = new Matrix2D();
            m.SetMatrix(sx, 0, 0, sy, ox - sx * ox, oy - sy * oy, MatrixType.Scale | MatrixType.Translation);

            return m;
        }

        /// <summary>
        /// Crea una matriu de rotacio.
        /// </summary>
        /// <param name="a">Angle de rotacio.</param>
        /// <param name="ox">Coordinada X del centre de rotacio.</param>
        /// <param name="oy">Coordinada Y del centre de rotacio.</param>
        /// <returns>La matriu.</returns>
        /// 
        private static Matrix2D CreateRotate(double a, double ox, double oy) {

            double rad = (a % 360.0) * Math.PI / 180.0;
            double sin = Math.Sin(rad);
            double cos = Math.Cos(rad);
            double tx = (ox * (1.0 - cos)) + (oy * sin);
            double ty = (oy * (1.0 - cos)) - (ox * sin);

            Matrix2D m = new Matrix2D();
            m.SetMatrix(cos, sin, -sin, cos, tx, ty, MatrixType.Unknown);
            
            return m;
        }

        /// <summary>
        /// Obte una matriu identitat.
        /// </summary>
        /// 
        public static Matrix2D Identity {
            get {
                return CreateIdentity();
            }
        }

        public double M11 {
            get {
                return m[0, 0];
            }
        }

        public double M12 {
            get {
                return m[0, 1];
            }
        }

        public double M21 {
            get {
                return m[1, 0];
            }
        }

        public double M22 {
            get {
                return m[1, 1];
            }
        }

        public double OffsetX {
            get {
                return m[2, 0];
            }
        }

        public double OffsetY {
            get {
                return m[2, 1];
            }
        }
    }

}

