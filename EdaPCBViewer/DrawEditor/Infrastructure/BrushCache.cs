namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Infrastructure {

    using System.Collections.Generic;
    using System.Windows.Media;

    public sealed class BrushCache {

        private readonly Dictionary<int, Brush> cache = new Dictionary<int, Brush>();

        /// <summary>
        /// Obte un brush
        /// </summary>
        /// <param name="color">El color brush.</param>
        /// <returns>El brush.</returns>
        /// 
        public Brush GetBrush(Color color) {

            Brush brush;

            int key = MakeHash(color);

            if (!cache.TryGetValue(key, out brush)) {

                brush = new SolidColorBrush(color);
                brush.Freeze();

                cache.Add(key, brush);
            }

            return brush;
        }

        private static int MakeHash(Color color) {

            return color.GetHashCode();
        }
    }
}
