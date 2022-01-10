using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Base.Geometry.Polygons {

    public static class EdaPolygonExtensions {

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

        public static EdaPolygon Transform(this EdaPolygon polygon, Transformation transformation) {

            List<EdaPolygon> childs = null;
            if (polygon.HasChilds) {
                childs = new List<EdaPolygon>(polygon.NumChilds);
                foreach (var child in polygon.Childs)
                    childs.Add(child.Transform(transformation));
            }

            List<EdaPoint> points = null;
            if (polygon.HasPoints) {
                points = new List<EdaPoint>(polygon.NumPoints);
                foreach (var point in polygon.Points)
                    points.Add(transformation.Transform(point));
            }

            return new EdaPolygon(points, childs);
        }
    }
}
