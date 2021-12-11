namespace MikroPic.EdaTools.v1.Base.Geometry.Polygons {

    using System.Collections.Generic;

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons.Infrastructure;

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
            cp.AddPath(ToPointList(sourcePolygon), PolyType.ptSubject, true);
            cp.AddPath(ToPointList(clipPolygon), PolyType.ptClip, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(GetClipType(op), solution, PolyFillType.pftNonZero);

            List<Polygon> result = new List<Polygon>();
            foreach (List<IntPoint> intPoints in solution)
                result.Add(ToPolygon(intPoints));
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
            cp.AddPath(ToPointList(sourcePolygon), PolyType.ptSubject, true);
            foreach (Polygon clipPolygon in clipPolygons)
                cp.AddPath(ToPointList(clipPolygon), PolyType.ptClip, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(GetClipType(op), solution, PolyFillType.pftNonZero);

            List<Polygon> result = new List<Polygon>();
            foreach (List<IntPoint> intPoints in solution)
                result.Add(ToPolygon(intPoints));
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
        public static Polygon ClipExtended(Polygon sourcePolygon, IEnumerable<Polygon> clipPolygons, ClipOperation op) {

            cp.Clear();
            cp.AddPath(ToPointList(sourcePolygon), PolyType.ptSubject, true);
            foreach (Polygon clipPolygon in clipPolygons)
                cp.AddPath(ToPointList(clipPolygon), PolyType.ptClip, true);

            PolyTree solution = new PolyTree();
            cp.Execute(GetClipType(op), solution, PolyFillType.pftNonZero);

            return ToPolygon(solution);
        }

        /// <summary>
        /// Fusiona els poligons d'una llista que estiguin en contacte.
        /// </summary>
        /// <param name="polygons">El poligons a fusionar.</param>
        /// <returns>La coleccio de poligons resultants de la fusio.</returns>
        /// 
        public static IEnumerable<Polygon> Union(IEnumerable<Polygon> polygons) {

            cp.Clear();
            foreach (Polygon polygon in polygons)
                cp.AddPath(ToPointList(polygon), PolyType.ptSubject, true);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            cp.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero);

            List<Polygon> result = new List<Polygon>();
            foreach (List<IntPoint> intPoints in solution)
                result.Add(ToPolygon(intPoints));
            return result;
        }

        public static Polygon Offset(Polygon sourcePoligon, double offset, OffsetJoin oj = OffsetJoin.Mitter) {

            cpOffset.Clear();
            cpOffset.AddPath(ToPointList(sourcePoligon), GetJoinType(oj), EndType.etClosedPolygon);

            List<List<IntPoint>> solutions = new List<List<IntPoint>>();
            cpOffset.Execute(ref solutions, offset);

            return ToPolygon(solutions[0]);
        }

        public static IEnumerable<Polygon> Offset(IEnumerable<Polygon> sourcePoligons, double offset, OffsetJoin oj = OffsetJoin.Mitter) {

            cpOffset.Clear();
            foreach (Polygon sourcePolygon in sourcePoligons)
                cpOffset.AddPath(ToPointList(sourcePolygon), GetJoinType(oj), EndType.etClosedPolygon);

            List<List<IntPoint>> solutions = new List<List<IntPoint>>();
            cpOffset.Execute(ref solutions, offset);

            List<Polygon> results = new List<Polygon>(solutions.Count);
            foreach (List<IntPoint> solution in solutions)
                results.Add(ToPolygon(solution));

            return results;
        }

        public static Polygon CreateFromSegments(IEnumerable<Segment> lines) {

            EdaPoint[] points = Polygonizer.Poligonize(lines);
            return points == null ? null : new Polygon(points);
        }

        /// <summary>
        /// Conversio a llista de punts.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private static List<IntPoint> ToPointList(Polygon polygon) {

            if (polygon.Points != null) {

                int i = 0;
                IntPoint[] dstArray = new IntPoint[polygon.NumPoints];

                foreach (var point in polygon.Points) {
                    dstArray[i].X = point.X;
                    dstArray[i].Y = point.Y;
                    i++;
                }

                return new List<IntPoint>(dstArray);
            }
            else
                return null;
        }

        /// <summary>
        /// Conversio a poligon
        /// </summary>
        /// <param name="points">La llista de punts.</param>
        /// <returns>El poligon.</returns>
        /// 
        private static Polygon ToPolygon(List<IntPoint> points) {

            IntPoint[] srcArray = points.ToArray();
            EdaPoint[] dstArray = new EdaPoint[srcArray.Length];
            for (int i = 0; i < srcArray.Length; i++)
                dstArray[i] = new EdaPoint((int)srcArray[i].X, (int)srcArray[i].Y);
            return new Polygon(dstArray);
        }

        /// <summary>
        /// Conversio a poligon
        /// </summary>
        /// <param name="polyNode">El PolyNode d'entrada.</param>
        /// <returns>El poligon.</returns>
        /// 
        private static Polygon ToPolygon(PolyNode polyNode) {

            EdaPoint[] points;
            if (polyNode.Contour.Count > 0) {
                points = new EdaPoint[polyNode.Contour.Count];
                for (int i = 0; i < polyNode.Contour.Count; i++)
                    points[i] = new EdaPoint((int)polyNode.Contour[i].X, (int)polyNode.Contour[i].Y);
            }
            else
                points = null;

            Polygon[] childs;
            if (polyNode.ChildCount > 0) {
                childs = new Polygon[polyNode.ChildCount];
                for (int i = 0; i < polyNode.ChildCount; i++)
                    childs[i] = ToPolygon(polyNode.Childs[i]);
            }
            else
                childs = null;

            return new Polygon(points, childs);
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
