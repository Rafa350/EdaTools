namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;
    using System.Collections.Generic;

    public abstract class MultiLayerElement: ElementBase {

        private readonly LayerSet layers = new LayerSet();

        /// <summary>
        /// Constructor per defecte de l'objecte.
        /// </summary>
        /// 
        public MultiLayerElement() {

        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio.</param>
        /// <param name="layers">La llista de capes a la que pertany.</param>
        /// 
        public MultiLayerElement(Point position, IEnumerable<Layer> layers):
            base(position) {

            if (layers == null)
                throw new ArgumentNullException("layers");

            this.layers.Add(layers);
        }

        /// <summary>
        /// Comprova si el objecte pertany a una capa.
        /// </summary>
        /// <param name="layer">La capa a comprovar.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public override bool InLayer(Layer layer) {

            foreach (var l in layers)
                if (l == layer)
                    return true;
            return false;
        }

        /// <summary>
        /// Obte el conjunt da capes del element.
        /// </summary>
        /// 
        public LayerSet Layers {
            get {
                return layers;
            }
        }
    }
}
