namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Collections;

    /// <summary>
    /// Clase que representa una placa.
    /// </summary>
    public sealed class Board: IVisitable {

        private readonly LayerCollection layers = new LayerCollection();
        private readonly ElementCollection elements = new ElementCollection();
        private readonly ComponentCollection components = new ComponentCollection();
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
        /// 
        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Afegeix una peça.
        /// </summary>
        /// <param name="part">La peça a afeigir.</param>
        /// 
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
        /// 
        public void AddSignal(Signal signal) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (signals == null)
                signals = new List<Signal>();
            signals.Add(signal);
        }

        /// <summary>
        /// Afeigeix un element.
        /// </summary>
        /// <param name="element">L'element a afeigir.</param>
        /// 
        public void AddElement(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            elements.Add(element);
        }

        public Signal GetSignal(string name) {

            if (signals != null)
                foreach (Signal signal in signals)
                    if (signal.Name == name)
                        return signal;
            return null;
        }

        /// <summary>
        /// Obte la llista de capes.
        /// </summary>
        /// 
        public LayerCollection Layers {
            get {
                return layers;
            }
        }

        /// <summary>
        /// Obte la llista de components.
        /// </summary>
        /// 
        public ComponentCollection Components {
            get {
                return components;
            }
        }

        /// <summary>
        /// Obte un enumerador per les peces.
        /// </summary>
        /// 
        public IEnumerable<Part> Parts {
            get {
                return parts;
            }
        }

        /// <summary>
        /// Obte un enunerador per les senyals.
        /// </summary>
        /// 
        public IEnumerable<Signal> Signals {
            get {
                return signals;
            }
        }

        /// <summary>
        /// Obte un enunerador pels elements.
        /// </summary>
        /// 
        public IEnumerable<Element> Elements {
            get {
                return elements;
            }
        }
    }
}
