using System.Collections;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry {
    public sealed class EdaPoints: IEnumerable<EdaPoint> {

        private readonly List<EdaPoint> _points;

        public EdaPoints(params EdaPoint[] points) {

            _points = new List<EdaPoint>(points);
        }

        public EdaPoints(IEnumerable<EdaPoint> points) {

            _points = new List<EdaPoint>(points);
        }

        public IEnumerator<EdaPoint> GetEnumerator() {

            return ((IEnumerable<EdaPoint>)_points).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            
            return ((IEnumerable)_points).GetEnumerator();
        }
    }
}
