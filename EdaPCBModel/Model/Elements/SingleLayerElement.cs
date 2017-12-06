namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;

    public abstract class SingleLayerElement: ElementBase {

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
        /// <param name="layer">La capa a la que pertany.</param>
        /// 
        public SingleLayerElement(Layer layer) {

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
        public override bool InLayer(Layer layer) {

            return this.layer == layer;
        }

        /// <summary>
        /// Obte o asigna la capa a la que pertany l'element.
        /// </summary>
        /// 
        public Layer Layer {
            get {
                return layer;
            }
            set {
                layer = value;
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
