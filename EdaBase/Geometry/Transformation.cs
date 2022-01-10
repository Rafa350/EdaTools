﻿using MikroPic.EdaTools.v1.Base.Geometry.Utils;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    public sealed class Transformation {

        private Matrix2D _m;

        public Transformation() {

            _m = Matrix2D.Identity;
        }

        public Transformation(Matrix2D m) {

            _m = m;
        }

        /// <summary>
        /// Afegeix una translacio.
        /// </summary>
        /// <param name="v">Vector de translacio.</param>
        /// 
        public void Translate(EdaPoint v) {

            Translate(v.X, v.Y);
        }

        /// <summary>
        /// Afegeig una translacio.
        /// </summary>
        /// <param name="offsetX">Translacio X</param>
        /// <param name="offsetY">Translacio Y</param>
        ///
        public void Translate(int offsetX, int offsetY) {

            if ((offsetX != 0) || (offsetY != 0))
                _m *= Matrix2D.CreateTranslation(offsetX, offsetY);
        }

        /// <summary>
        /// Afegeix un canvi d'escala.
        /// </summary>
        /// <param name="scaleX">Escala X</param>
        /// <param name="scaleY">Escala Y</param>
        /// 
        public void Scale(int scaleX, int scaleY) {

            if ((scaleX != 1) || (scaleY != 1))
                _m *= Matrix2D.CreateScale(scaleX, scaleY);
        }

        /// <summary>
        /// Afegeix un canvi d'escala.
        /// </summary>
        /// <param name="center">Centre d'escalat.</param>
        /// <param name="scaleX">Escala X</param>
        /// <param name="scaleY">Escala Y</param>
        /// 
        public void Scale(EdaPoint center, int scaleX, int scaleY) {

            if ((scaleX != 1) || (scaleY != 1))
                _m *= Matrix2D.CreateScale(scaleX, scaleY, center.X, center.Y);
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

            if ((scaleX != 1) || (scaleY != 1))
                _m *= Matrix2D.CreateScale(scaleX, scaleY, centerX, centerY);
        }

        /// <summary>
        /// Afegeix una rotacio.
        /// </summary>
        /// <param name="rotation">Angle de rotacio.</param>
        /// 
        public void Rotate(EdaAngle rotation) {

            if (!rotation.IsZero)
                _m *= Matrix2D.CreateRotation(rotation.AsDegrees);
        }

        /// <summary>
        /// Afegeix una rotacio.
        /// </summary>
        /// <param name="center">Coordinades del centre de rotacio.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// 
        public void Rotate(EdaPoint center, EdaAngle rotation) {

            if (!rotation.IsZero)
                _m *= Matrix2D.CreateRotation(rotation.AsDegrees, center.X, center.Y);
        }

        /// <summary>
        /// Afegeix una rotacio.
        /// </summary>
        /// <param name="centerX">Coordinada X del centre de rotacio.</param>
        /// <param name="centerY">Coordinada Y del centre de rotacio.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// 
        public void Rotate(int centerX, int centerY, EdaAngle rotation) {

            if (!rotation.IsZero)
                _m *= Matrix2D.CreateRotation(rotation.AsDegrees, centerX, centerY);
        }

        /// <summary>
        /// Aplica la transfornmacio a un punt.
        /// </summary>
        /// <param name="point">El punt.</param>
        /// <returns>El punt transformat.</returns>
        /// 
        public EdaPoint Transform(EdaPoint point) {

            double x = (point.X * _m.M11) + (point.Y * _m.M21) + _m.Tx;
            double y = (point.X * _m.M12) + (point.Y * _m.M22) + _m.Ty;

            return new EdaPoint((int)x, (int)y);
        }

        /// <summary>
        /// Aplica la transformacio a un array de punts.
        /// </summary>
        /// <param name="points">El array de punts.</param>
        /// 
        public void ApplyTo(EdaPoint[] points) {

            for (int i = 0; i < points.Length; i++) {

                double x = (points[i].X * _m.M11) + (points[i].Y * _m.M21) + _m.Tx;
                double y = (points[i].X * _m.M12) + (points[i].Y * _m.M22) + _m.Ty;

                points[i] = new EdaPoint((int)x, (int)y);
            }
        }

        public Matrix2D Matrix =>
            _m;
    }
}
