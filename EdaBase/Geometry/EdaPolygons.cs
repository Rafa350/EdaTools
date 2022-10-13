using System.Collections;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    public sealed class EdaPolygons : IEnumerable<EdaPolygon> {

        private readonly List<EdaPolygon> _polygons;

        public EdaPolygons(params EdaPolygon[] polygons) :
            this((IEnumerable<EdaPolygon>) polygons){

        }
        public EdaPolygons(IEnumerable<EdaPolygon> polygons) {

            _polygons = new List<EdaPolygon>(polygons);
        }

        public IEnumerator<EdaPolygon> GetEnumerator() {
            return ((IEnumerable<EdaPolygon>)_polygons).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_polygons).GetEnumerator();
        }
    }
}
