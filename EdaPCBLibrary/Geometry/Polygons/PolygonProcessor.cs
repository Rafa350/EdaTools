namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System.Windows;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons.Infrastructure;

    /// <summary>
    /// Clase per procesar poligons.
    /// </summary>
    /// 
    public static class PolygonProcessor {

        /// <summary>
        /// Codi d'operacio.
        /// </summary>
        /// 
        public enum ClipOperation {
            Intersection,
            Union,
            Diference,
            Xor
        }

        public enum OffsetJoin {
            Round,
            Square,
            Mitter
        }

        private const double scaleFactor = 1000000.0;

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

            cp.Clear();
            cp.AddPath(PolygonToPointList(sourcePolygon), PolyType.ptSubject, true);
            cp.AddPath(PolygonToPointList(clipPolygon), PolyType.ptClip, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(GetClipType(op), solution, PolyFillType.pftNonZero);

            List<Polygon> result = new List<Polygon>();
            foreach (List<IntPoint> intPoints in solution)
                result.Add(PointListToPolygon(intPoints));
            return result;
        }

        /// <summary>
        /// Retalla un poligon.
        /// </summary>
        /// <param name="sourcePolygon">Poligon a retallar.</param>
        /// <param name="clipPolygons">Poligon de retall.</param>
        /// <param name="op">Operacio de retall.</param>
        /// <returns>La sequencia de poligons resultants de l'operacio.</returns>
        /// 
        public static IEnumerable<Polygon> Clip(Polygon sourcePolygon, IEnumerable<Polygon> clipPolygons, ClipOperation op) {

            cp.Clear();
            cp.AddPath(PolygonToPointList(sourcePolygon), PolyType.ptSubject, true);
            foreach (Polygon clipPolygon in clipPolygons)
                cp.AddPath(PolygonToPointList(clipPolygon), PolyType.ptClip, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(GetClipType(op), solution, PolyFillType.pftNonZero);

            List<Polygon> result = new List<Polygon>();
            foreach (List<IntPoint> intPoints in solution)
                result.Add(PointListToPolygon(intPoints));
            return result;
        }

        /// <summary>
        /// Retalla un poligon.
        /// </summary>
        /// <param name="sourcePolygon">Poligon a retallar.</param>
        /// <param name="clipPolygons">Poligon de retall.</param>
        /// <param name="op">Operacio de retall.</param>
        /// <returns>Arbre de poligons.</returns>
        /// 
        public static PolygonNode ClipExtended(Polygon sourcePolygon, IEnumerable<Polygon> clipPolygons, ClipOperation op) {

            cp.Clear();
            cp.AddPath(PolygonToPointList(sourcePolygon), PolyType.ptSubject, true);
            foreach (Polygon clipPolygon in clipPolygons)
                cp.AddPath(PolygonToPointList(clipPolygon), PolyType.ptClip, true);

            PolyTree solution = new PolyTree();
            cp.Execute(GetClipType(op), solution, PolyFillType.pftNonZero);

            return PolyTreeToPolygonTree(solution);
        }

        /// <summary>
        /// Fusiona els poligons d'una llista que estiguin en contacte.
        /// </summary>
        /// <param name="polygons">El poligons a fisionar.</param>
        /// <returns>La coleccio de poligons resultants de la fusio.</returns>
        /// 
        public static IEnumerable<Polygon> Union(IEnumerable<Polygon> polygons) {

            cp.Clear();
            foreach (Polygon polygon in polygons)
                cp.AddPath(PolygonToPointList(polygon), PolyType.ptSubject, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero);

            List<Polygon> result = new List<Polygon>();
            foreach (List<IntPoint> intPoints in solution)
                result.Add(PointListToPolygon(intPoints));
            return result;
        }

        public static Polygon Offset(Polygon sourcePoligon, double offset, OffsetJoin oj = OffsetJoin.Mitter) {

            cpOffset.Clear();
            cpOffset.AddPath(PolygonToPointList(sourcePoligon), GetJoinType(oj), EndType.etClosedPolygon);

            List<List<IntPoint>> solutions = new List<List<IntPoint>>();
            cpOffset.Execute(ref solutions, offset * scaleFactor);

            return PointListToPolygon(solutions[0]);
        }

        public static IEnumerable<Polygon> Offset(IEnumerable<Polygon> sourcePoligons, double offset, OffsetJoin oj = OffsetJoin.Mitter) {

            cpOffset.Clear();
            foreach (Polygon sourcePolygon in sourcePoligons)
                cpOffset.AddPath(PolygonToPointList(sourcePolygon), GetJoinType(oj), EndType.etClosedPolygon);

            List<List<IntPoint>> solutions = new List<List<IntPoint>>();
            cpOffset.Execute(ref solutions, offset * scaleFactor);

            List<Polygon> results = new List<Polygon>(solutions.Count);
            foreach (List<IntPoint> solution in solutions)
                results.Add(PointListToPolygon(solution));

            return results;
        }

        /// <summary>
        /// Conversio a llista de punts.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private static List<IntPoint> PolygonToPointList(Polygon polygon) {

            List<IntPoint> intPoints = new List<IntPoint>();
            foreach (Point point in polygon)
                intPoints.Add(new IntPoint(point.X * scaleFactor, point.Y * scaleFactor));
            return intPoints;
        }

        /// <summary>
        /// Conversio a poligon
        /// </summary>
        /// <param name="points">La llista de punts.</param>
        /// <returns>El poligon.</returns>
        /// 
        private static Polygon PointListToPolygon(IEnumerable<IntPoint> points) {

            Polygon polygon = new Polygon();
            foreach (IntPoint intPoint in points)
                polygon.Add(new Point((double)intPoint.X / scaleFactor, (double)intPoint.Y / scaleFactor));
            return polygon;
        }

        /// <summary>
        /// Conversio a PolygonNode
        /// </summary>
        /// <param name="polyNode">En PolyNode d'entrada.</param>
        /// <returns>El PolygonTree de sortida.</returns>
        /// 
        private static PolygonNode PolyTreeToPolygonTree(PolyNode polyNode) {

            Polygon polygon = null;
            List<PolygonNode> childs = null;

            if (polyNode.Contour.Count > 0)
                polygon = PointListToPolygon(polyNode.Contour);

            if (polyNode.ChildCount > 0) {
                childs = new List<PolygonNode>(polyNode.ChildCount);
                foreach (PolyNode polyNodeChild in polyNode.Childs)
                    childs.Add(PolyTreeToPolygonTree(polyNodeChild));
            }

            return new PolygonNode(polygon, childs);
        }

        /// <summary>
        /// Obte el codi de retall que correspon a cada operacio.
        /// </summary>
        /// <param name="op">Operacio de retall.</param>
        /// <returns>El tipus de retall a aplicar.</returns>
        /// 
        private static ClipType GetClipType(ClipOperation op) {

            switch (op) {
                case ClipOperation.Intersection:
                    return ClipType.ctIntersection;

                case ClipOperation.Union:
                    return ClipType.ctUnion;

                case ClipOperation.Xor:
                    return ClipType.ctXor;

                default:
                case ClipOperation.Diference:
                    return ClipType.ctDifference;
            }
        }

        private static JoinType GetJoinType(OffsetJoin oj) {

            switch (oj) {
                case OffsetJoin.Round:
                    return JoinType.jtRound;

                case OffsetJoin.Square:
                    return JoinType.jtSquare;

                default:
                case OffsetJoin.Mitter:
                    return JoinType.jtMiter;
            }
        }
    }
}
