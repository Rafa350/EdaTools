namespace MikroPic.EdaTools.v1.Geometry.Polygons.Infrastructure {

    using System;
    using System.Collections.Generic;
    
    public static class Polygonizer {
        
        static public Point[] Poligonize(IEnumerable<Segment> segments) {

            int vertexCount = 0;
            Point firstVertex = new Point();
            Dictionary<Point, Point> conectionMap = new Dictionary<Point, Point>();
            List<Point> vertexList = new List<Point>();

            foreach (var segment in segments) {
                if (!vertexList.Contains(segment.Start))
                    vertexList.Add(segment.Start);
                if (!vertexList.Contains(segment.End))
                    vertexList.Add(segment.End);
            }

            foreach (var segment in segments) {

                if (!conectionMap.ContainsKey(segment.Start)) {
                    conectionMap.Add(segment.Start, segment.End);
                    if (vertexCount++ == 0)
                        firstVertex = segment.Start;
                }

                else if (!conectionMap.ContainsKey(segment.End)) {
                    conectionMap.Add(segment.End, segment.Start);
                    if (vertexCount++ == 0) 
                        firstVertex = segment.End;
                }
            }

            List<Point> points = new List<Point>();
            Point p = firstVertex;
            for (int i = 0; i < vertexCount; i++) { 
                points.Add(p);
                p = conectionMap[p];
            }

            return points.ToArray();
        }
    }
}