namespace MikroPic.EdaTools.v1.Base.Geometry.Polygons.Infrastructure {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using System;
    using System.Collections.Generic;

    public static class Polygonizer {

        static public EdaPoints Poligonize(IEnumerable<Segment> segments) {

            if (segments == null)
                throw new ArgumentNullException(nameof(segments));

            EdaPoint p = default(EdaPoint);
            bool first = true;
            HashSet<Segment> segmentPool = new HashSet<Segment>();
            foreach (var segment in segments) {
                if (first) {
                    first = false;
                    p = segment.Start;
                }
                segmentPool.Add(segment);
            }

            if (segmentPool.Count > 2) {

                EdaPoints polygon = EdaPoints.Create();

                while (segmentPool.Count > 0) {
                    Segment s;
                    if (FindSegmentWidthVertex(segmentPool, p, out s)) {
                        if (Compare(p, s.Start))
                            p = s.End;
                        else
                            p = s.Start;
                        polygon.AddPoint(p);
                        segmentPool.Remove(s);
                    }
                    else
                        break;
                }
                return polygon;
            }
            else
                return null;
        }

        private static bool Compare(EdaPoint p1, EdaPoint p2) {

            return (p1.X == p2.X) && (p1.Y == p2.Y);
        }

        private static bool FindSegmentWidthVertex(IEnumerable<Segment> segments, EdaPoint point, out Segment result) {

            result = default(Segment);
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
    }
}