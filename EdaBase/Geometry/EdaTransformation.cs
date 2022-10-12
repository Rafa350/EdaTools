using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MikroPic.EdaTools.v1.Base.Geometry.Utils;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    public sealed class EdaTransformation {

        private Matrix2D _m;

        public EdaTransformation() {

            _m = Matrix2D.Identity;
        }

        public EdaTransformation(Matrix2D m) {

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
        /// Aplica la transformacio a un punt.
        /// </summary>
        /// <param name="point">El punt.</param>
        /// <returns>El punt transformat.</returns>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EdaPoint Transform(EdaPoint point) {

            double x = (point.X * _m.M11) + (point.Y * _m.M21) + _m.Tx;
            double y = (point.X * _m.M12) + (point.Y * _m.M22) + _m.Ty;

            return new EdaPoint((int)x, (int)y);
        }

        /// <summary>
        /// Aplica la transformacio a una coleccio de punts.
        /// </summary>
        /// <param name="point">La coleccio de punts.</param>
        /// <returns>La coleccio de punts del resultat de la transformacio.</returns>
        /// 
        public IEnumerable<EdaPoint> Transform(IEnumerable<EdaPoint> points) {

            var result = new List<EdaPoint>();

            foreach (var point in points) {
                double x = (point.X * _m.M11) + (point.Y * _m.M21) + _m.Tx;
                double y = (point.X * _m.M12) + (point.Y * _m.M22) + _m.Ty;
                result.Add(new EdaPoint((int)x, (int)y));
            }

            return result;
        }

        /// <summary>
        /// Obte la matriu de la transformacio.
        /// </summary>
        /// 
        public Matrix2D Matrix =>
            _m;
    }
}
