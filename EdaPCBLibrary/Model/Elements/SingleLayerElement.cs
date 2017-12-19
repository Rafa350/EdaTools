namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public abstract class SingleLayerElement: Element {

        private Layer layer;

        /// <summary>
        /// Constructor por defecte de l'objecte.
        /// </summary>
        /// 
        public SingleLayerElement() {

        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="position">Posicio.</param>
        /// <param name="layer">La capa a la que pertany.</param>
        /// 
        public SingleLayerElement(Point position, Layer layer):
            base(position) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            this.layer = layer;
        }

        /// <summary>
        /// Comprova si pertany a una capa.
        /// </summary>
        /// <param name="layer">La capa a comprovar.</param>
        /// <returns>True si pertany a la capa, false en cas contrari.</returns>
        /// 
        public override bool IsOnLayer(Layer layer) {

            return this.layer == layer;
        }

        /// <summary>
        /// Afegeix l'element a la capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// 
        public void AddToLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (this.layer != null)
                throw new InvalidOperationException("Este elemento solo puede pertenecer a una capa.");

            this.layer = layer;
        }

        /// <summary>
        /// Treu l'element de la capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// 
        public void RemoveFromLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (this.layer != layer)
                throw new InvalidOperationException("El elemento no pertenece a la capa.");

            this.layer = null;
        }

        /// <summary>
        /// Obte la capa a la que pertany l'element.
        /// </summary>
        /// 
        public Layer Layer {
            get {
                return layer;
            }
        }

        /// <summary>
        /// Obte la capa mirall, si en te.
        /// </summary>
        /// 
        public Layer MirrorLayer {
            get {
                if ((layer != null) && (layer.Mirror != null))
                    return layer.Mirror;
                else
                    return null;
            }
        }
    }
}
