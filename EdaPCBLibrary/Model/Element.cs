namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Model.Collections;
    using System;

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
    public abstract class Element : IVisitable, ICollectionChild<Board>, ICollectionChild<Block> {

        private LayerSet layerSet;
        private Board board;
        private Block block;

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
        /// Comprova si l'element pertany a una capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public bool IsOnLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            return layerSet.Contains(layer.Id);
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public abstract void AcceptVisitor(IVisitor visitor);

        /// <summary>
        /// Asigna l'objecte pare.
        /// </summary>
        /// <param name="block">L'objecte pare.</param>
        /// 
        public void AssignParent(Block block) {

            this.block = block;
        }

        /// <summary>
        /// Asigna l'objecte pare.
        /// </summary>
        /// <param name="board">L'objecte pare.</param>
        /// 
        public void AssignParent(Board board) {

            this.board = board;
        }

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
        /// Obte el bloc al que pertany
        /// </summary>
        /// 
        public Block Block {
            get {
                return block;
            }
        }

        /// <summary>
        /// Obte l'etiqueta a la que pertany
        /// </summary>
        /// 
        public Board Board {
            get {
                return board;
            }
        }

        /// <summary>
        /// Obte el tipus d'element
        /// </summary>
        /// 
        public abstract ElementType ElementType { get; }
    }
}
