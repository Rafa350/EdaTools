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

            List<EdaPolygon> childs = null;
            if (polygon.Childs != null) {
                childs = new List<EdaPolygon>(polygon.NumChilds);
                foreach (var child in polygon.Childs)
                    childs.Add(child.Clone());
            }

            List<EdaPoint> points = null;
            if (polygon.Points != null) {
                points = new List<EdaPoint>(polygon.NumPoints);
                foreach (EdaPoint point in polygon.Points)
                    points.Add(point);
            }

            return new EdaPolygon(points, childs);
        }

        /// <summary>
        /// Transforma un poligon.
        /// </summary>
        /// <param name="polygon">El poligon a transformat.</param>
        /// <param name="transformation">La transformacio.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static EdaPolygon Transform(this EdaPolygon polygon, EdaTransformation transformation) {

            List<EdaPolygon> childs = null;
            if (polygon.HasChilds) {
                childs = new List<EdaPolygon>(polygon.NumChilds);
                foreach (var child in polygon.Childs)
                    childs.Add(child.Transform(transformation));
            }

            List<EdaPoint> points = null;
            if (polygon.HasPoints)
                points = new List<EdaPoint>(transformation.Transform(polygon.Points));

            return new EdaPolygon(points, childs);
        }

        /// <summary>
        /// Subtraccio d'un poligon.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <param name="toSubstract">Els poligons a substreure.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static EdaPolygon Substract(this EdaPolygon polygon, params EdaPolygon[] toSubstract) {

            return polygon.Substract((IEnumerable<EdaPolygon>)toSubstract);
        }

        /// <summary>
        /// Subtraccio d'un poligon.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <param name="toSubstract">Els poligons a substreure.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static EdaPolygon Substract(this EdaPolygon polygon, IEnumerable<EdaPolygon> toSubstract) {

            var cp = new Clipper64();
            cp.AddSubject(ToPath64(polygon.Points));
            foreach (var p in toSubstract)
                cp.AddClip(ToPath64(p.Points));
            var solution = new PolyTree64();
            cp.Execute(ClipType.Difference, FillRule.NonZero, solution);

            return ToPolygon(solution);
        }

        /// <summary>
        /// Canvia el tamany d'un poligon.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <param name="delta">El increment de tamany.</param>
        /// <returns>El resultat.</returns>
        /// 
        public static EdaPolygon Offset(this EdaPolygon polygon, int delta) {

            List<EdaPolygon> childs = null;
            if (polygon.HasChilds) {
                childs = new List<EdaPolygon>();
                foreach (var child in polygon.Childs)
                    childs.Add(Offset(child, delta));
            }

            var cpo = new ClipperOffset();
            cpo.AddPath(ToPath64(polygon.Points), JoinType.Round, EndType.Round);
            var solution = cpo.Execute(delta);
            //if (solution.Count < 2) {
                var points = ToPoints(solution[0]);
                return new EdaPolygon(points, childs);
            //}
        }

        private static Path64 ToPath64(IEnumerable<EdaPoint> points) {

            var path = new Path64();
            foreach (var point in points)
                path.Add(new Point64(point.X, point.Y));
            return path;
        }

        private static IEnumerable<EdaPoint> ToPoints(Path64 path) {
            
            var points = new List<EdaPoint>(path.Count);
            foreach (var item in path) 
                points.Add(new EdaPoint((int)item.X, (int)item.Y));

            return points;
        }

        private static EdaPolygon ToPolygon(PolyPath64 tree) {

            List<EdaPoint> points = null;
            if (tree.Polygon != null) {
                points = new List<EdaPoint>(tree.Polygon.Count);
                foreach (var item in tree.Polygon)
                    points.Add(new EdaPoint((int)item.X, (int)item.Y));
            }

            List<EdaPolygon> childs = null;
            if (tree.Count > 0) {
                childs = new List<EdaPolygon>(tree.Count);
                foreach (var item in tree.OfType<PolyPath64>())
                    childs.Add(ToPolygon(item));
            }

            return new EdaPolygon(points, childs);
        }
    }
}
