using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
            var contour = new List<EdaPoint>(transformation.Transform(polygon.Contour));

            // Transforma els forats.
            //
            List<List<EdaPoint>> holes = null;
            if (polygon.HasHoles) {
                holes = new List<List<EdaPoint>>();
                foreach (var hole in polygon.Holes)
                    holes.Add(new List<EdaPoint>(transformation.Transform(hole)));
            }

            return new EdaPolygon(contour, holes);
        }

        public static IEnumerable<EdaPolygon> Transform(this IEnumerable<EdaPolygon> polygons, EdaTransformation transformation) {

            var transformedPolygons = new List<EdaPolygon>();
            foreach (var polygon in polygons)
                transformedPolygons.Add(polygon.Transform(transformation));
            return transformedPolygons;
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
            
            cp.AddSubject(ToPath64(polygon.Contour));
            
            foreach (var substractPolygon in substractPolygons)
                cp.AddClip(ToPath64(substractPolygon.Contour));
            
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

            cpo.AddPath(ToPath64(polygon.Contour), JoinType.Round, EndType.Round);
            
            var solution = cpo.Execute(delta);

            var points = ToPoints(solution[0]);
            return new EdaPolygon(points);
        }

        private static Path64 ToPath64(IEnumerable<EdaPoint> points) {

            return new Path64(points.Select(i => new Point64(i.X, i.Y)));
        }

        private static IEnumerable<EdaPoint> ToPoints(Path64 path) {
            
            return path.Select(i => new EdaPoint((int)i.X, (int)i.Y));
        }

        private static IEnumerable<EdaPolygon> ToPolygons(PolyPath64 tree) {

            var polygons = new List<EdaPolygon>(tree.Count);
            if (tree.Count > 0) 
                foreach (PolyPath64 item in tree) {

                    List<List<EdaPoint>> holes = null;
                    if (item.Count > 0) {
                        holes = new List<List<EdaPoint>>();
                        foreach (PolyPath64 subItem in item)
                            holes.Add(new List<EdaPoint>(subItem.Polygon.Select(i => new EdaPoint((int)i.X, (int)i.Y))));
                    }

                    var contour = new List<EdaPoint>(item.Polygon.Select(i => new EdaPoint((int)i.X, (int)i.Y)));
                    polygons.Add(new EdaPolygon(contour, holes));
                }

            return polygons;
        }
    }
}
