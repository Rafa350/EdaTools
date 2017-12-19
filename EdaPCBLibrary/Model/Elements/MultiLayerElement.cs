namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Model.Collections;
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public abstract class MultiLayerElement: Element {

        private readonly LayerCollection layers = new LayerCollection();

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
        public override bool IsOnLayer(Layer layer) {

            foreach (var l in layers)
                if (l == layer)
                    return true;
            return false;
        }

        /// <summary>
        /// Afegeix l'element a la capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// 
        public void AddToLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (layers.Contains(layer))
                throw new InvalidOperationException("El elemento ya pertenece a la capa.");

            layers.Add(layer);
        }

        /// <summary>
        /// Treu l'element de la capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// 
        public void RemoveFromLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (!layers.Contains(layer))
                throw new InvalidOperationException("El elemento no pertenece a la capa.");

            layers.Remove(layer);
        }

        /// <summary>
        /// Obte el conjunt de capes a la que pertany l'element.
        /// </summary>
        /// 
        public IEnumerable<Layer> Layers {
            get {
                return layers;
            }
        }
    }
}
