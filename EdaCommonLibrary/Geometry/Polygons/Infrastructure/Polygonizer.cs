namespace MikroPic.EdaTools.v1.Geometry.Polygons.Infrastructure {

    using System;
    using System.Collections.Generic;
    
    public static class Polygonizer {
        
        static public Point[] Poligonize(IEnumerable<Segment> segments) {

            if (segments == null)
                throw new ArgumentNullException("segments");

            Point p = default(Point);
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

                List<Point> polygon = new List<Point>();

                while (segmentPool.Count > 0) {
                    Segment s;
                    if (FindSegmentWidthVertex(segmentPool, p, out s)) {
                        if (Compare(p, s.Start))
                            p = s.End;
                        else
                            p = s.Start;
                        polygon.Add(p);
                        segmentPool.Remove(s);
                    }
                    else
                        return null;
                }
                return polygon.ToArray();
            }
            else
                return null;
        }

        private static bool Compare(Point p1, Point p2) {

            return (p1.X == p2.X) && (p1.Y == p2.Y);
        }

        private static bool FindSegmentWidthVertex(IEnumerable<Segment> segments, Point point, out Segment result) {

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