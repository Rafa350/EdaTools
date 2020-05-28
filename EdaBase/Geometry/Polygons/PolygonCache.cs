namespace MikroPic.EdaTools.v1.Base.Geometry.Polygons {

    using System.Collections.Generic;

    public static class PolygonCache {

        private static readonly Dictionary<int, Polygon> cache = new Dictionary<int, Polygon>();

        public static Polygon Get(int hash) {

            if (cache.TryGetValue(hash, out Polygon polygon))
                return polygon;
            else
                return null;
        }

        public static void Save(int hash, Polygon polygon) {

            if (!cache.ContainsKey(hash))
                cache.Add(hash, polygon);
        }
    }
}
