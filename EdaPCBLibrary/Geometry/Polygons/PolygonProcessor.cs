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

        /// <summary>
        /// Retalla un poligon.
        /// </summary>
        /// <param name="sourcePolygon">Poligon a retallar.</param>
        /// <param name="clipPolygon">Poligon de retall.</param>
        /// <param name="op">Operacio de retall.</param>
        /// <returns>La sequencia de poligons resultants de l'operacio.</returns>
        /// 
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

        public static Polygon Offset(Polygon sourcePoligon, double offset) {

            cpOffset.Clear();
            cpOffset.AddPath(PolygonToPointList(sourcePoligon), JoinType.jtRound, EndType.etClosedPolygon);

            List<List<IntPoint>> solutions = new List<List<IntPoint>>();
            cpOffset.Execute(ref solutions, offset * 10000);

            return PointListToPolygon(solutions[0]);
        }

        public static IEnumerable<Polygon> Offset(IEnumerable<Polygon> sourcePoligons, double offset) {

            cpOffset.Clear();
            foreach (Polygon sourcePolygon in sourcePoligons)
                cpOffset.AddPath(PolygonToPointList(sourcePolygon), JoinType.jtRound, EndType.etClosedPolygon);

            List<List<IntPoint>> solutions = new List<List<IntPoint>>();
            cpOffset.Execute(ref solutions, offset * 10000);

            List<Polygon> results = new List<Polygon>(solutions.Count);
            foreach (List<IntPoint> solution in solutions)
                results.Add(PointListToPolygon(solution));

            return results;
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
