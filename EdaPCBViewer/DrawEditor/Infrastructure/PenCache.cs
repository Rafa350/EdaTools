namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Infrastructure {

    using System.Collections.Generic;
    using System.Windows.Media;

    public sealed class PenCache {

        private readonly Dictionary<int, Pen> cache = new Dictionary<int, Pen>();

        /// <summary>
        /// Obte un pen
        /// </summary>
        /// <param name="brush">El brush.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="lineCap">Tipus d'extrem de linia.</param>
        /// <returns>El pen.</returns>
        /// 
        public Pen GetPen(Brush brush, double thickness, PenLineCap lineCap) {

            Pen pen;

            int key = MakeHash(brush, thickness, lineCap);

            if (!cache.TryGetValue(key, out pen)) {

                pen = new Pen(brush, thickness);
                pen.StartLineCap = lineCap;
                pen.EndLineCap = lineCap;
                pen.LineJoin = PenLineJoin.Round;
                pen.Freeze();

                cache.Add(key, pen);
            }

            return pen;
        }

        /// <summary>
        /// Obte el hash del pen.
        /// </summary>
        /// <param name="brush">El brush.</param>
        /// <param name="thickness">L'amplada de linia.</param>
        /// <param name="lineCap">El tipus de final de linia.</param>
        /// <returns>El codi hash.</returns>
        /// 
        private static int MakeHash(Brush brush, double thickness, PenLineCap lineCap = PenLineCap.Round) {

            return brush.GetHashCode() + (thickness.GetHashCode() << 3) + (lineCap.GetHashCode() << 7);
        }
    }
}
