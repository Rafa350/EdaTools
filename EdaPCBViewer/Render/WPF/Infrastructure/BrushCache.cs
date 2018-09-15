namespace MikroPic.EdaTools.v1.Designer.Render.WPF.Infrastructure {

    using System.Collections.Generic;
    using System.Windows.Media;

    internal sealed class BrushCache {

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

        /// <summary>
        /// Obte el hash del brush
        /// </summary>
        /// <param name="color">El color del brussh.</param>
        /// <returns>El valor del hash.</returns>
        /// 
        private static int MakeHash(Color color) {

            return color.A << 24 | color.R << 16 | color.G << 8 | color.B;
        }
    }
}
