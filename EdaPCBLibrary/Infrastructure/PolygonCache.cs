namespace MikroPic.EdaTools.v1.Pcb.Infrastructure {

    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using System;
    using System.Collections.Generic;

    public static class PolygonCache {

        private static readonly Dictionary<string, Polygon> cache = new Dictionary<string, Polygon>();

        public static void AddPolygon(string signature, Polygon polygon) {

            if (String.IsNullOrEmpty(signature))
                throw new ArgumentNullException("signature");

            if (polygon == null)
                throw new ArgumentNullException("polygon");

            cache.Add(signature, polygon);
        }

        public static Polygon GetPolygon(string signature) {
            
            Polygon polygon;
            if (cache.TryGetValue(signature, out polygon))
                return polygon;
            else
                return null;
        }
    }
}
