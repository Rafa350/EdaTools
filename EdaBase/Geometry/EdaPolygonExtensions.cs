using System.Collections.Generic;
using System.Linq;
using Clipper2Lib;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    public static class EdaPolygonExtensions {

        /// <summary>
        /// Clona un poligon.
        /// </summary>
        /// <param name="polygon">El poligon a clonar.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static EdaPolygon Clone(this EdaPolygon polygon) {

            return new EdaPolygon(polygon.Outline, polygon.Holes);
        }

        /// <summary>
        /// Transforma un poligon.
        /// </summary>
        /// <param name="polygon">El poligon a transformar.</param>
        /// <param name="transformation">La transformacio.</param>
        /// <returns>El poligon transformat.</returns>
        /// 
        public static EdaPolygon Transform(this EdaPolygon polygon, EdaTransformation transformation) {

            // Transforma el contorn.
            //
            var outline = transformation.Transform(polygon.Outline);

            // Transforma els forats.
            //
            List<IEnumerable<EdaPoint>> holes = null;
            if (polygon.HasHoles) {
                holes = new List<IEnumerable<EdaPoint>>();
                foreach (var hole in polygon.Holes)
                    holes.Add(transformation.Transform(hole));
            }

            return new EdaPolygon(outline, holes);
        }

        /// <summary>
        /// Transforma una coleccio de poligons.
        /// </summary>
        /// <param name="polygons">Els poligons.</param>
        /// <param name="transformation">La transformacio.</param>
        /// <returns>Els poligons transformats.</returns>
        /// 
        public static IEnumerable<EdaPolygon> Transform(this IEnumerable<EdaPolygon> polygons, EdaTransformation transformation) {

            var result = new List<EdaPolygon>();
            foreach (var polygon in polygons)
                result.Add(polygon.Transform(transformation));
            return result;
        }

        public static IEnumerable<EdaPolygon> Combine(this EdaPolygon polygon, params EdaPolygon[] combinePolygons) {

            return Combine(polygon, (IEnumerable<EdaPolygon>)combinePolygons);
        }

        public static IEnumerable<EdaPolygon> Combine(this EdaPolygon polygon, IEnumerable<EdaPolygon> combinePolygons) {

            var cp = new Clipper64();

            cp.AddSubject(ToPath64(polygon.Outline));
            foreach (var combinePolygon in combinePolygons)
                cp.AddClip(ToPath64(combinePolygon.Outline));

            var tree = new PolyTree64();
            cp.Execute(ClipType.Union, FillRule.NonZero, tree);

            return ToPolygons(tree);
        }

        /// <summary>
        /// Subtraccio d'un o diversos poligons.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <param name="substractPolygons">Els poligons a substreure.</param>
        /// <returns>El resultat de l'operacio. Pot ser mes d'un poligon.</returns>
        /// 
        public static IEnumerable<EdaPolygon> Substract(this EdaPolygon polygon, params EdaPolygon[] substractPolygons) {

            return polygon.Substract((IEnumerable<EdaPolygon>)substractPolygons);
        }

        /// <summary>
        /// Subtraccio d'un o diversos poligons.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <param name="substractPolygons">Els poligons a substreure.</param>
        /// <returns>El resultat de l'operacio. Pot ser mes d0un poligon.</returns>
        /// 
        public static IEnumerable<EdaPolygon> Substract(this EdaPolygon polygon, IEnumerable<EdaPolygon> substractPolygons) {

            var cp = new Clipper64();

            cp.AddSubject(ToPath64(polygon.Outline));
            foreach (var substractPolygon in substractPolygons)
                cp.AddClip(ToPath64(substractPolygon.Outline));

            var tree = new PolyTree64();
            cp.Execute(ClipType.Difference, FillRule.NonZero, tree);

            return ToPolygons(tree);
        }

        /// <summary>
        /// Canvia el tamany d'un poligon.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <param name="delta">El increment de tamany.</param>
        /// <returns>El resultat.</returns>
        /// 
        public static EdaPolygon Offset(this EdaPolygon polygon, int delta) {

            var cpo = new ClipperOffset();

            var path = new Paths64();
            path.Add(ToPath64(polygon.Outline));
            if (polygon.HasHoles)
                foreach (var hole in polygon.Holes)
                    path.Add(ToPath64(hole, true));

            cpo.AddPaths(path, JoinType.Round, EndType.Polygon);
            var solution = cpo.Execute(delta);

            List<EdaPoint> outline = ToPoints(solution[0]);
            List<List<EdaPoint>> holes = null;
            if (solution.Count > 1) {
                holes = new List<List<EdaPoint>>(solution.Count - 1);
                for (int i = 1; i < solution.Count; i++)
                    holes.Add(ToPoints(solution[i], true));
            }

            return new EdaPolygon(outline, holes);
        }

        public static IEnumerable<EdaPolygon> Offset(this IEnumerable<EdaPolygon> polygons, int delta) {

            var result = new List<EdaPolygon>();
            foreach (var polygon in polygons)
                result.Add(Offset(polygon, delta));
            return result;
        }

        public static EdaPolygon Clean(this EdaPolygon polygon) {

            double epsilon = 2500;

            var outline = ToPoints(Clipper.RamerDouglasPeucker(ToPath64(polygon.Outline), epsilon));

            List<List<EdaPoint>> holes = null;
            if (polygon.HasHoles) {
                holes = new List<List<EdaPoint>>();
                foreach (var hole in polygon.Holes)
                    holes.Add(ToPoints(Clipper.RamerDouglasPeucker(ToPath64(hole), epsilon)));
            }

            return new EdaPolygon(outline, holes);
        }

        public static IEnumerable<EdaPolygon> Clean(this IEnumerable<EdaPolygon> polygons) {

            var result = new List<EdaPolygon>();
            foreach (var polygon in polygons)
                result.Add(Clean(polygon));

            return result;
        }

        private static Path64 ToPath64(IEnumerable<EdaPoint> points, bool reverse = false) {

            var path = new Path64(points.Select(i => new Point64(i.X, i.Y)));
            if (reverse)
                path.Reverse();

            return path;
        }

        private static List<EdaPoint> ToPoints(Path64 path, bool reverse = false) {

            var points = new List<EdaPoint>(path.Select(i => new EdaPoint((int)i.X, (int)i.Y)));
            if (reverse)
                points.Reverse();

            return points;
        }

        private static IEnumerable<EdaPolygon> ToPolygons(PolyPath64 tree) {

            var polygons = new List<EdaPolygon>(tree.Count);
            if (tree.Count > 0)
                foreach (PolyPath64 item in tree) {

                    List<List<EdaPoint>> holes = null;
                    if (item.Count > 0) {
                        holes = new List<List<EdaPoint>>();
                        foreach (PolyPath64 subItem in item) {
                            var hole = new List<EdaPoint>(subItem.Polygon.Select(i => new EdaPoint((int)i.X, (int)i.Y)));
                            hole.Reverse();
                            holes.Add(hole);
                        }
                    }

                    var contour = new List<EdaPoint>(item.Polygon.Select(i => new EdaPoint((int)i.X, (int)i.Y)));
                    polygons.Add(new EdaPolygon(contour, holes));
                }

            return polygons;
        }
    }
}
