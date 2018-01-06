namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System.Windows.Media;

    public abstract class Element : IVisitable {

        private Polygon polygon;

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public abstract void AcceptVisitor(IVisitor visitor);

        /// <summary>
        /// Comprova si l'element pertany a la capa.
        /// </summary>
        /// <param name="layer">La capa a comprovar.</param>
        /// <returns>True si pertany a la capa. False en cas contraru.</returns>
        /// 
        public abstract bool IsOnLayer(Layer layer);

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="inflate">Increment de tamany</param>
        /// <returns>El poligon</returns>
        /// 
        public abstract Polygon GetPolygon(double inflate = 0);

        /// <summary>
        /// Invalida el caches interns de l'element.
        /// </summary>
        /// 
        protected virtual void Invalidate() {

            polygon = null;
        }

        /// <summary>
        /// Obte el poligon del element.
        /// </summary>
        /// 
        public Polygon Polygon {
            get {
                if (polygon == null)
                    polygon = GetPolygon();
                return polygon.Clone();
            }
        }
    }
}
