namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;

    /// <summary>
    /// Identifica el tipus d'element
    /// </summary>
    /// 
    public enum ElementType {
        Line,
        Arc,
        Rectangle,
        Circle,
        Text,
        SmdPad,
        ThPad,
        SlotPad,
        Region,
        Via,
        Hole
    }

    /// <summary>
    /// Clase base per tots els elements de la placa.
    /// </summary>
    /// 
    public abstract class Element : IVisitable {

        private LayerSet layerSet;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// 
        public Element(LayerSet layerSet) {

            this.layerSet = layerSet;
        }

        /// <summary>
        /// Obte una copia en profunditat de l'objecte.
        /// </summary>
        /// <returns>La copia de l'objecte.</returns>
        /// 
        public abstract Element Clone();

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public abstract void AcceptVisitor(IVisitor visitor);

        /// <summary>
        /// Obte el poligon del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon</returns>
        /// 
        public abstract Polygon GetPolygon(BoardSide side);

        /// <summary>
        /// Obte el poligon espaiat del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <returns>El poligon.</returns>
        /// 
        public abstract Polygon GetOutlinePolygon(BoardSide side, int spacing);

        /// <summary>
        /// Obte el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public abstract Rect GetBoundingBox(BoardSide side);

        /// <summary>
        /// Obte o asigna el conjunt de capes.
        /// </summary>
        /// 
        public LayerSet LayerSet {
            get {
                return layerSet;
            }
            set {
                layerSet = value;
            }
        }

        /// <summary>
        /// Obte el tipus d'element
        /// </summary>
        /// 
        public abstract ElementType ElementType { get; }
    }
}
