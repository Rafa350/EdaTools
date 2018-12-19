namespace MikroPic.EdaTools.v1.Base.Geometry {

    using System.Windows.Media;

    public sealed class Transform {

        private readonly Matrix m = new Matrix();

        /// <summary>
        /// Afegeix una tarnslacio a la transformacio.
        /// </summary>
        /// <param name="v">Coordinades de translacio.</param>
        /// 
        public void AppendTranslation(Point v) {

            m.Translate(v.X, v.Y);
        }

        /// <summary>
        /// Afegeig una translacio a la transformacio.
        /// </summary>
        /// <param name="offsetX">Translacio X</param>
        /// <param name="offsetY">Translacio Y</param>
        ///
        public void AppendTranslation(int offsetX, int offsetY) {

            m.Translate(offsetX, offsetY);
        }

        /// <summary>
        /// Afegeix un canvi d'escala a la transformacio.
        /// </summary>
        /// <param name="scaleX">Escala X</param>
        /// <param name="scaleY">Escala Y</param>
        /// 
        public void AppendScale(int scaleX, int scaleY) {

            m.Scale(scaleX, scaleY);
        }

        /// <summary>
        /// Afegeix un canvi d'escala a la transformacio.
        /// </summary>
        /// <param name="center">Centre d'escalat.</param>
        /// <param name="scaleX">Escala X</param>
        /// <param name="scaleY">Escala Y</param>
        /// 
        public void AppendScaleAt(Point center, int scaleX, int scaleY) {

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
        public void AppendScaleAt(int centerX, int centerY, int scaleX, int scaleY) {

            m.ScaleAt(scaleX, scaleY, centerX, centerY);
        }

        /// <summary>
        /// Afegeix una rotacio a la transformacio
        /// </summary>
        /// <param name="rotation">Angle de rotacio.</param>
        /// 
        public void AppendRotate(Angle rotation) {

            m.Rotate(rotation.Degrees / 1000.0);
        }

        /// <summary>
        /// Afegeix una rotacio a la transformacio.
        /// </summary>
        /// <param name="center">Coordinades del centre de rotacio.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// 
        public void AppendRotate(Point center, Angle rotation) {

            m.RotateAt(rotation.Degrees / 1000.0, center.X, center.Y);
        }

        /// <summary>
        /// Afegeix una rotacio a la transformacio.
        /// </summary>
        /// <param name="centerX">Coordinada X del centre de rotacio.</param>
        /// <param name="centerY">Coordinada Y del centre de rotacio.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// 
        public void AppendRotate(int centerX, int centerY, Angle rotation) {

            m.RotateAt(rotation.Degrees / 1000.0, centerX, centerY);
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
    }
}
