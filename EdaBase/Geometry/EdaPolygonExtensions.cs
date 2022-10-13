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

            return new EdaPolygon(polygon.Contour, polygon.Holes);
        }

        /// <summary>
        /// Transforma un poligon.
        /// </summary>
        /// <param name="polygon">El poligon a transformat.</param>
        /// <param name="transformation">La transformacio.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static EdaPolygon Transform(this EdaPolygon polygon, EdaTransformation transformation) {

            // Transaforma el contorn.
            //
            var contour = new EdaPoints(transformation.Transform(polygon.Contour));

            // Transforma els forats.
            //
            List<EdaPoints> holes = null;
            if (polygon.Holes != null) {
                holes = new List<EdaPoints>();
                foreach (var hole in polygon.Holes)
                    holes.Add(new EdaPoints(transformation.Transform(hole)));
            }

            return new EdaPolygon(contour, holes);
        }

        /// <summary>
        /// Subtraccio d'un o diversos poligons.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <param name="toSubstract">Els poligons a substreure.</param>
        /// <returns>El resultat de l'operacio. Pot ser mes d'un poligon.</returns>
        /// 
        public static IEnumerable<EdaPolygon> Substract(this EdaPolygon polygon, params EdaPolygon[] toSubstract) {

            return polygon.Substract((IEnumerable<EdaPolygon>)toSubstract);
        }

        /// <summary>
        /// Subtraccio d'un o diversos poligons.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <param name="toSubstract">Els poligons a substreure.</param>
        /// <returns>El resultat de l'operacio. Pot ser mes d0un poligon.</returns>
        /// 
        public static IEnumerable<EdaPolygon> Substract(this EdaPolygon polygon, IEnumerable<EdaPolygon> toSubstract) {

            var cp = new Clipper64();
            cp.AddSubject(ToPath64(polygon.Contour));
            foreach (var p in toSubstract)
                cp.AddClip(ToPath64(p.Contour));
            var solution = new PolyTree64();
            cp.Execute(ClipType.Difference, FillRule.NonZero, solution);

            return ToPolygons(solution);
        }

        /// <summary>
        /// Canvia el tamany d'un poligon.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <param name="delta">El increment de tamany.</param>
        /// <returns>El resultat.</returns>
        /// 
        public static EdaPolygon Offset(this EdaPolygon polygon, int delta) {
            /*
                        List<EdaPolygon> childs = null;
                        if (polygon.Holes != null) {
                            childs = new List<EdaPolygon>();
                            foreach (var child in polygon.Holes)
                                childs.Add(Offset(child, delta));
                        }

                        var cpo = new ClipperOffset();
                        cpo.AddPath(ToPath64(polygon.Contour), JoinType.Round, EndType.Round);
                        var solution = cpo.Execute(delta);
                        //if (solution.Count < 2) {
                            var points = ToPoints(solution[0]);
                            return new EdaPolygon(points, childs);
                        //}*/

            return null;
        }

        private static Path64 ToPath64(IEnumerable<EdaPoint> points) {

            return new Path64(points.Select(i => new Point64(i.X, i.Y)));
        }

        private static IEnumerable<EdaPoint> ToPoints(Path64 path) {
            
            return path.Select(i => new EdaPoint((int)i.X, (int)i.Y));
        }

        private static IEnumerable<EdaPolygon> ToPolygons(PolyPath64 tree) {

            /*List<EdaPoint>  = null;
            if (tree.Polygon != null) {
                points = new List<EdaPoint>(tree.Polygon.Count);
                foreach (var item in tree.Polygon)
                    points.Add(new EdaPoint((int)item.X, (int)item.Y));
            }

            List<List<EdaPoint>> childs = null;
            if (tree.Count > 0) {
                childs = new List<EdaPolygon>(tree.Count);
                foreach (var item in tree.OfType<PolyPath64>())
                    childs.Add(ToPolygon(item));
            }

            return new EdaPolygon(points, childs);
            */
            return null;
        }
    }
}
