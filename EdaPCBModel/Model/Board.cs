namespace MikroPic.EdaTools.v1.Model {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa una placa.
    /// </summary>
    public sealed class Board: IVisitable {

        private Dictionary<LayerId, Layer> layers;
        private List<Component> components;
        private List<Part> parts;
        private List<Signal> signals;

        /// <summary>
        /// Constructor per defecte.
        /// </summary>
        public Board() {
        }

        /// <summary>
        /// Procesa un visitador.
        /// </summary>
        /// <param name="visitor">Visitador.</param>
        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Afegeix una capa.
        /// </summary>
        /// <param name="layer">La capa a afeigir.</param>
        public void AddLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (layers == null)
                layers = new Dictionary<LayerId, Layer>();
            layers.Add(layer.Id, layer);
        }

        /// <summary>
        /// Afegeix un component.
        /// </summary>
        /// <param name="component">El component a afeigir.</param>
        public void AddComponent(Component component) {

            if (component == null)
                throw new ArgumentNullException("component");

            if (components == null)
                components = new List<Component>();
            components.Add(component);
        }

        /// <summary>
        /// Afegeix una peça.
        /// </summary>
        /// <param name="part">La peça a afeigir.</param>
        public void AddPart(Part part) {

            if (part == null)
                throw new ArgumentNullException("part");

            if (parts == null)
                parts = new List<Part>();
            parts.Add(part);
        }

        /// <summary>
        /// Afeigeix una senyal.
        /// </summary>
        /// <param name="signal">La senyal a afeigir.</param>
        public void AddSignal(Signal signal) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (signals == null)
                signals = new List<Signal>();
            signals.Add(signal);
        }

        /// <summary>
        /// Obte una capa a partir del seu identificador.
        /// </summary>
        /// <param name="id">El identificador de la capa.</param>
        /// <returns>La capa. Null si no la troba.</returns>
        public Layer GetLayer(LayerId id) {

            return layers == null ? null : layers[id];
        }

        /// <summary>
        /// Obte un enuymerador per les capes.
        /// </summary>
        public IEnumerable<Layer> Layers {
            get {
                return layers.Values;
            }
        }

        /// <summary>
        /// Obte un enumerador pel components.
        /// </summary>
        public IEnumerable<Component> Components {
            get {
                return components;
            }
        }

        /// <summary>
        /// Obte un enumerador per les peces.
        /// </summary>
        public IEnumerable<Part> Parts {
            get {
                return parts;
            }
        }

        /// <summary>
        /// Obte un enunerador per les senyals.
        /// </summary>
        public IEnumerable<Signal> Signals {
            get {
                return signals;
            }
        }
    }
}
