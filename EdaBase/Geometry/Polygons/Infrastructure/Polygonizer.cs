using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry.Polygons.Infrastructure {


    public static class Polygonizer {

        static public IEnumerable<EdaPoint> Poligonize(IEnumerable<Segment> segments) {

            if (segments == null)
                throw new ArgumentNullException(nameof(segments));

            EdaPoint p = default;

            bool first = true;
            var segmentPool = new HashSet<Segment>();
            foreach (var segment in segments) {
                if (first) {
                    first = false;
                    p = segment.Start;
                }
                segmentPool.Add(segment);
            }

            if (segmentPool.Count > 2) {

                var points = new List<EdaPoint>();

                while (segmentPool.Count > 0) {
                    Segment s;
                    if (FindSegmentWidthVertex(segmentPool, p, out s)) {
                        if (p == s.Start)
                            p = s.End;
                        else
                            p = s.Start;
                        points.Add(p);
                        segmentPool.Remove(s);
                    }
                    else
                        break;
                }
                return points;
            }
            else
                return null;
        }

        private static bool FindSegmentWidthVertex(IEnumerable<Segment> segments, EdaPoint point, out Segment result) {

            result = default;
            foreach (var segment in segments) {
                if (segment.Start ==  point) {
                    result = segment;
                    return true;
                }

                else if (segment.End == point) {
                    result = segment;
                    return true;
                }
            }

            return false;
        }
    }
}