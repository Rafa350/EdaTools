namespace MikroPic.EdaTools.v1.Base.Geometry {

    using System.Windows.Media;

    public sealed class Transformation {

        private Matrix m = Matrix.Identity;

        /// <summary>
        /// Afegeix una translacio.
        /// </summary>
        /// <param name="v">Vector de translacio.</param>
        /// 
        public void Translate(Point v) {

            m.Translate(v.X, v.Y);
        }

        /// <summary>
        /// Afegeig una translacio.
        /// </summary>
        /// <param name="offsetX">Translacio X</param>
        /// <param name="offsetY">Translacio Y</param>
        ///
        public void Translate(int offsetX, int offsetY) {

            m.Translate(offsetX, offsetY);
        }

        /// <summary>
        /// Afegeix un canvi d'escala.
        /// </summary>
        /// <param name="scaleX">Escala X</param>
        /// <param name="scaleY">Escala Y</param>
        /// 
        public void Scale(int scaleX, int scaleY) {

            m.Scale(scaleX, scaleY);
        }

        /// <summary>
        /// Afegeix un canvi d'escala.
        /// </summary>
        /// <param name="center">Centre d'escalat.</param>
        /// <param name="scaleX">Escala X</param>
        /// <param name="scaleY">Escala Y</param>
        /// 
        public void Scale(Point center, int scaleX, int scaleY) {

            m.ScaleAt(scaleX, scaleY, center.X, center.Y);
        }

        /// <summary>
        /// Afegeix un canvi d'escala.
        /// </summary>
        /// <param name="centerX">Coordinada X del centre d'escalat.</param>
        /// <param name="centerY">Coordinada Y del centre d'escalat.</param>
        /// <param name="scaleX">Escala X</param>
        /// <param name="scaleY">Escala Y</param>
        /// 
        public void Scale(int centerX, int centerY, int scaleX, int scaleY) {

            m.ScaleAt(scaleX, scaleY, centerX, centerY);
        }

        /// <summary>
        /// Afegeix una rotacio.
        /// </summary>
        /// <param name="rotation">Angle de rotacio.</param>
        /// 
        public void Rotate(Angle rotation) {

            m.Rotate(rotation.ToDegrees);
        }

        /// <summary>
        /// Afegeix una rotacio.
        /// </summary>
        /// <param name="center">Coordinades del centre de rotacio.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// 
        public void Rotate(Point center, Angle rotation) {

            m.RotateAt(rotation.ToDegrees, center.X, center.Y);
        }

        /// <summary>
        /// Afegeix una rotacio.
        /// </summary>
        /// <param name="centerX">Coordinada X del centre de rotacio.</param>
        /// <param name="centerY">Coordinada Y del centre de rotacio.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// 
        public void Rotate(int centerX, int centerY, Angle rotation) {

            m.RotateAt(rotation.ToDegrees, centerX, centerY);
        }

        /// <summary>
        /// Aplica la transfornmacio a un punt.
        /// </summary>
        /// <param name="point">El punt.</param>
        /// <returns>El punt transformat.</returns>
        /// 
        public Point ApplyTo(Point point) {

            double x = point.X;
            double y = point.Y;
            double xadd = y * m.M21 + m.OffsetX;
            double yadd = x * m.M12 + m.OffsetY;

            x *= m.M11;
            x += xadd;
            y *= m.M22;
            y += yadd;

            return new Point((int)x, (int)y);
        }

        /// <summary>
        /// Aplica la transformacio a un array de punts.
        /// </summary>
        /// <param name="points">El array de punts.</param>
        /// 
        public void ApplyTo(Point[] points) {

            for (int i = 0; i < points.Length; i++) {

                double x = points[i].X;
                double y = points[i].Y;
                double xadd = y * m.M21 + m.OffsetX;
                double yadd = x * m.M12 + m.OffsetY;

                x *= m.M11;
                x += xadd;
                y *= m.M22;
                y += yadd;

                points[i] = new Point((int)x, (int)y);
            }
        }

        /// <summary>
        /// Obte la matriu de la transformacio.
        /// </summary>
        /// 
        public Matrix Matrix {
            get {
                return m;
            }
        }
    }
}
