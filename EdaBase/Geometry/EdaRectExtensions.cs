using System;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    public static class EdaRectExtensions {

        /// <summary>
        /// Obte un rectangle desplaçat.
        /// </summary>
        /// <param name="dx">Desplaçament X</param>
        /// <param name="dy">Desplaçament Y</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static EdaRect Offset(this EdaRect r, int dx, int dy) =>
            new(r.X + dx, r.Y + dy, r.Width, r.Height);

        /// <summary>
        /// Obte un rectangle inflat.
        /// </summary>
        /// <param name="dx">Increment X</param>
        /// <param name="dy">Increment Y</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static EdaRect Inflated(this EdaRect r, int dx, int dy) =>
            new(r.X - dx, r.Y - dy, r.Width + dx + dx, r.Height + dy + dy);

        /// <summary>
        /// Obte un rectangle inflat.
        /// </summary>
        /// <param name="ratio">Percentatge d'inflat.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static EdaRect Inflated(this EdaRect r, EdaRatio ratio) {
            var delta = Math.Min(r.Width, r.Height) * ratio / 2;
            return new(r.X - delta, r.Y - delta, r.Width + delta + delta, r.Height + delta + delta);
        }

        /// <summary>
        /// Obte un rectangle desinflat.
        /// </summary>
        /// <param name="ratio">Percentatge de desinflat.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static EdaRect Deflated(this EdaRect r, EdaRatio ratio) {
            var delta = -Math.Min(r.Width, r.Height) * ratio / 2;
            return new(r.X - delta, r.Y - delta, r.Width + delta + delta, r.Height + delta + delta);
        }

        /// <summary>
        /// Obte la unio amb un altre rectangle
        /// </summary>
        /// <param name="rect">Rectangle a unir.</param>
        /// <returns>El resultat de la unio.</returns>
        /// 
        public static EdaRect Union(this EdaRect r, EdaRect rect) {

            int left = Math.Min(r.Left, rect.Left);
            int right = Math.Max(r.Right, rect.Right);
            int top = Math.Min(r.Top, rect.Top);
            int bottom = Math.Max(r.Bottom, rect.Bottom);

            return new(left, top, right - left + 1, bottom - top + 1);
        }

        /// <summary>
        /// Comprova si intersecta amb un rectangle
        /// </summary>
        /// <param name="rect">El rectangle a verificar.</param>
        /// <returns>True si intersecten, false en cas contrari.</returns>
        /// 
        public static bool IntersectsWith(this EdaRect r, EdaRect rect) {

            return (rect.Left <= r.Right) && (rect.Right >= r.Left) &&
                   (rect.Bottom <= r.Top) && (rect.Top >= r.Bottom);
        }

    }
}
