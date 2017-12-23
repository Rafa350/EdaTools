namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System.Windows;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons.Infrastructure;

    public static class PolygonProcessor {

        public enum ClipOperation {
            Intersection,
            Union,
            Diference,
            Xor
        }

        private static readonly Clipper cp = new Clipper();
        private static readonly ClipperOffset cpOffset = new ClipperOffset();

        public static IEnumerable<Polygon> Clip(Polygon sourcePolygon, Polygon clipPolygon, ClipOperation op) {

            ClipType ct = ClipType.ctDifference;
            switch  (op) {
                case ClipOperation.Intersection:
                    ct = ClipType.ctIntersection;
                    break;

                case ClipOperation.Union:
                    ct = ClipType.ctUnion;
                    break;

                case ClipOperation.Xor:
                    ct = ClipType.ctXor;
                    break;
            }

            cp.Clear();
            cp.AddPath(PolygonToPointList(sourcePolygon), PolyType.ptSubject, true);
            cp.AddPath(PolygonToPointList(clipPolygon), PolyType.ptClip, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(ct, solution);

            List<Polygon> polygons = new List<Polygon>();
            foreach (List<IntPoint> intPoints in solution)
                polygons.Add(PointListToPolygon(intPoints));
            return polygons;
        }

        private static List<IntPoint> PolygonToPointList(Polygon polygon) {

            List<IntPoint> intPoints = new List<IntPoint>();
            foreach (Point point in polygon.Points)
                intPoints.Add(new IntPoint(point.X * 10000.0, point.Y * 10000.0));
            return intPoints;
        }

        private static Polygon PointListToPolygon(List<IntPoint> points) {

            Polygon polygon = new Polygon();
            foreach (IntPoint intPoint in points)
                polygon.Add(new Point((double)intPoint.X / 10000.0, (double)intPoint.Y / 10000.0));
            return polygon;
        }
    }
}
