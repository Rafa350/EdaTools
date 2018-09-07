﻿namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;

    /// <summary>
    /// Clase base per tots els elements de la placa.
    /// </summary>
    /// 
    public abstract class BoardElement : IVisitable {

        private LayerSet layerSet;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="layerSet">El conjunt de capes.</param>
        /// 
        public BoardElement(LayerSet layerSet) {

            this.layerSet = layerSet;
        }

        /// <summary>
        /// Comprova si l'element pertany a una capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public bool IsOnLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            return layerSet.Contains(layer.Name);
        }

        /// <summary>
        /// Obte el bloc al que perteny l'element
        /// </summary>
        /// <returns>El bloc al que pertany o null si no pertany a cap.</returns>
        //
        Block GetBlock() {

            return Block.GetBlock(this);
        }

        /// <summary>
        /// Obte la placa a la que pertany l'element.
        /// </summary>
        /// <returns>La placa a la que pertany o nul si no pertany a cap.</returns>
        /// 
        Board GetBoard() {

            return null;
        }

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
    }
}
