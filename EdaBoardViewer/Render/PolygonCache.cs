using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace EdaBoardViewer.Render {

    public sealed class PolygonCache {

        private readonly Dictionary<int, EdaPolygon> _elementCache = new Dictionary<int, EdaPolygon>();
        private readonly Dictionary<int, IEnumerable<EdaPolygon>> _regionCache = new Dictionary<int, IEnumerable<EdaPolygon>>();

        public PolygonCache() {
        }

        public void Clear() {

            _elementCache.Clear();
            _regionCache.Clear();
        }

        public EdaPolygon GetPolygon(EdaElementBase element, EdaLayerId layerId) {

            int key = HashCode.Combine(element.GetHashCode(), layerId);

            if (!_elementCache.TryGetValue(key, out EdaPolygon polygon)) {
                polygon = element.GetPolygon(layerId);
                _elementCache.Add(key, polygon);
            }

            return polygon;
        }

        public EdaPolygon GetDrillPolygon(EdaThtPadElement element) {

            int key = element.GetHashCode();

            if (!_elementCache.TryGetValue(key, out EdaPolygon polygon)) {
                polygon = element.GetDrillPolygon();
                _elementCache.Add(key, polygon);
            }

            return polygon;
        }

        public EdaPolygon GetDrillPolygon(EdaViaElement element) {

            int key = element.GetHashCode();

            if (!_elementCache.TryGetValue(key, out EdaPolygon polygon)) {
                polygon = element.GetDrillPolygon();
                _elementCache.Add(key, polygon);
            }

            return polygon;
        }

        public EdaPolygon GetOutlinePolygon(EdaElementBase element, EdaLayerId layerId, int spacing) {

            int key = HashCode.Combine(element.GetHashCode(), layerId, spacing);

            if (!_elementCache.TryGetValue(key, out EdaPolygon polygon)) {
                polygon = element.GetOutlinePolygon(layerId, spacing);
                _elementCache.Add(key, polygon);
            }

            return polygon;
        }

        public IEnumerable<EdaPolygon> GetPolygons(EdaBoard board, EdaRegionElement region, EdaLayerId layerId) {

            int key = HashCode.Combine(region.GetHashCode(), layerId);

            if (!_regionCache.TryGetValue(key, out IEnumerable<EdaPolygon> polygons)) {
                polygons = board.GetRegionPolygons(region, layerId, null);
                _regionCache.Add(key, polygons);
            }

            return polygons;
        }
    }
}
