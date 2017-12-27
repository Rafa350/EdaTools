namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa una regio amb forats. Tant la regio com els forats
    /// estan representats per poligons. El primer nivell son poligons, despres,
    /// cada nivell va alternant forat/poligon.
    /// </summary>
    /// 
    public sealed class PolygonNode {

        private readonly Polygon polygon;
        private readonly List<PolygonNode> childs;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// <param name="childs">Els nodes fills.</param>
        /// 
        public PolygonNode(Polygon polygon, IEnumerable<PolygonNode> childs = null) {

            this.polygon = polygon;
            if (childs != null)
                this.childs = new List<PolygonNode>(childs);
        }

        /// <summary>
        /// Obte el poligon.
        /// </summary>
        /// 
        public Polygon Polygon {
            get {
                return polygon;
            }
        }

        /// <summary>
        /// Indica si el node te fills.
        /// </summary>
        /// 
        public bool HasChilds {
            get {
                return childs != null;
            }
        }

        /// <summary>
        /// Obte un enumerador dels fills.
        /// </summary>
        /// 
        public IEnumerable<PolygonNode> Childs {
            get {
                return childs;
            }
        }
    }
}
