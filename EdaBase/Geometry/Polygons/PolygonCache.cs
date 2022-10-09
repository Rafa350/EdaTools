namespace MikroPic.EdaTools.v1.Base.Geometry.Polygons
{

    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Base.Geometry;

    public static class PolygonCache {

        private static readonly Dictionary<int, EdaPolygon> cache = new Dictionary<int, EdaPolygon>();

        public static EdaPolygon Get(int hash) {

            if (cache.TryGetValue(hash, out EdaPolygon polygon))
                return polygon;
            else
                return null;
        }

        public static void Save(int hash, EdaPolygon polygon) {

            if (!cache.ContainsKey(hash))
                cache.Add(hash, polygon);
        }
    }
}
