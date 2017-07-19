namespace Eda.PCBViewer.DrawEditor {

    using System.Collections.Generic;
    using System.Windows.Media;

    internal sealed class BrushCache {

        private static BrushCache instance;
        private Dictionary<Color, Brush> cache;

        private BrushCache() {

            cache = new Dictionary<Color, Brush>();
        }

        public void Clear() {

            cache.Clear();
        }

        public Brush GetBrush(Color color) {

            Brush brush;

            if (!cache.TryGetValue(color, out brush)) {
                brush = new SolidColorBrush(color);
                brush.Freeze();
                cache.Add(color, brush);
            }
            return brush;
        }

        public static BrushCache Instance {
            get {
                if (instance == null)
                    instance = new BrushCache();
                return instance;
            }
        }
    }
}
