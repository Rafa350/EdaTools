namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System.Windows;

    /// <summary>
    /// Clase base per tots els elements de la placa.
    /// </summary>
    public abstract class Element : IVisitable {

        private Polygon polygon = null;
        private Rect boundingBox = Rect.Empty;
        private bool locked = false;

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public abstract void AcceptVisitor(IVisitor visitor);

        /// <summary>
        /// Bloqueja l'element i impedeix que es pugui editar.
        /// </summary>
        /// 
        public void Lock() {

            locked = true;
        }

        /// <summary>
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="inflate">Increment de tamany</param>
        /// <returns>El poligon</returns>
        /// 
        public abstract Polygon GetPolygon(double inflate = 0);

        /// <summary>
        /// Calula el bounding box del element.
        /// </summary>
        /// <returns>El bounding box.</returns>
        /// 
        protected abstract Rect GetBoundingBox();

        /// <summary>
        /// Invalida el caches interns de l'element.
        /// </summary>
        /// 
        protected void Invalidate() {

            polygon = null;
            boundingBox = Rect.Empty;
        }

        /// <summary>
        /// Obte el bounding box del element.
        /// </summary>
        /// 
        public Rect BoundingBox {
            get {
                if (boundingBox.IsEmpty)
                    boundingBox = GetBoundingBox();
                return boundingBox;
            }
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

        /// <summary>
        /// Indica si esta bloquejat.
        /// </summary>
        /// 
        public bool IsLocked {
            get {
                return locked;
            }
        }
    }
}
