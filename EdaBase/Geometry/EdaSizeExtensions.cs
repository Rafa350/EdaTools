using System;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    public static class EdaSizeExtensions {

        public static EdaSize Inflated(this EdaSize s, int dx, int dy) =>
            new (s.Width + dx, s.Height + dy);

        public static EdaSize Inflated(this EdaSize s, int d) =>
            new (s.Width + d, s.Height + d);

        public static EdaSize Inflated(this EdaSize s, EdaRatio ratio) {
            var delta = Math.Min(s.Width, s.Height) * ratio;
            return new EdaSize(s.Width + delta, s.Height + delta);
        }

        public static EdaSize Deflated(this EdaSize s, EdaRatio ratio) {
            var delta = Math.Min(s.Width, s.Height) * ratio;
            return new EdaSize(s.Width - delta, s.Height - delta);
        }
    }
}
