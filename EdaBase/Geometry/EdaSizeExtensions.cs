namespace MikroPic.EdaTools.v1.Base.Geometry {

    public static class EdaSizeExtensions {

        public static EdaSize Inflated(this EdaSize size, int dx, int dy) {

            return new EdaSize(size.Width + dx, size.Height + dy);
        }

        public static EdaSize Inflated(this EdaSize size, int d) {

            return new EdaSize(size.Width + d, size.Height + d);
        }
    }
}
