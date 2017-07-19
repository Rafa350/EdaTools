namespace Eda.PCBViewer.DrawEditor {

    using System.Collections.Generic;
    using System.Windows.Media;

    internal sealed class PenCache {

        private static PenCache instance;
        private Dictionary<int, Pen> cache;

        private PenCache() {

            cache = new Dictionary<int, Pen>();
        }

        public void Clear() {

            cache.Clear();
        }

        public Pen GetPen(Color color, double thickness, PenLineCap lineCap = PenLineCap.Round) {

            int hash = color.A * 1 + color.R * 33 + color.G * 71 + color.B * 111 +  thickness.GetHashCode() + lineCap.GetHashCode();

            Pen pen;
            if (!cache.TryGetValue(hash, out pen)) {
                pen = new Pen();
                pen.Brush = BrushCache.Instance.GetBrush(color);
                pen.Thickness = thickness;
                pen.StartLineCap = lineCap;
                pen.EndLineCap = lineCap;
                pen.Freeze();
                cache.Add(hash, pen);
            }
            return pen;
        }

        public static PenCache Instance {
            get {
                if (instance == null)
                    instance = new PenCache();
                return instance;
            }
        }
    }
}
