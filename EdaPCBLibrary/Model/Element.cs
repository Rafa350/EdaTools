namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System.Windows;

    /// <summary>
    /// Clase base per tots els elements de la placa.
    /// </summary>
    public abstract class Element : IVisitable {

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public abstract void AcceptVisitor(IVisitor visitor);

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
        public abstract Rect GetBoundingBox();
    }
}
