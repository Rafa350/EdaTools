using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry.Polygons.Infrastructure {


    public static class Polygonizer {

        static public IEnumerable<EdaPoint> Poligonize(IEnumerable<EdaSegment> segments) {

            if (segments == null)
                throw new ArgumentNullException(nameof(segments));

            EdaPoint p = default;

            bool first = true;
            var segmentPool = new HashSet<EdaSegment>();
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
                    EdaSegment s;
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

        private static bool FindSegmentWidthVertex(IEnumerable<EdaSegment> segments, EdaPoint point, out EdaSegment result) {

            result = default;
            foreach (var segment in segments) {
                if (Compare(segment.Start, point)) {
                    result = segment;
                    return true;
                }

                else if (Compare(segment.End, point)) {
                    result = segment;
                    return true;
                }
            }

            return false;
        }

        private static bool Compare(EdaPoint p1, EdaPoint p2) {

            return (Math.Abs(p1.X - p2.X) < 10) && (Math.Abs(p1.Y - p2.Y) < 10);
        }
    }
}