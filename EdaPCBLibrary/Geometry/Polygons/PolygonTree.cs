namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System.Collections.Generic;

    public sealed class PolygonTree {

        private readonly Polygon polygon;
        private readonly List<PolygonTree> childs;

        public PolygonTree(Polygon polygon, IEnumerable<PolygonTree> childs = null) {

            this.polygon = polygon;
            if (childs != null)
                this.childs = new List<PolygonTree>(childs);
        }

        public Polygon Polygon {
            get {
                return polygon;
            }
        }

        public bool HasChilds {
            get {
                return childs != null;
            }
        }

        public IEnumerable<PolygonTree> Childs {
            get {
                return childs;
            }
        }
    }
}
