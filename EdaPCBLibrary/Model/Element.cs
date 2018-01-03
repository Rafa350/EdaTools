namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;

    public abstract class Element : IVisitable {

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
        /// Obte el poligon del element.
        /// </summary>
        /// 
        //public abstract Polygon Polygon { get; }
        public virtual Polygon Polygon { get { return null; } }
    }
}
