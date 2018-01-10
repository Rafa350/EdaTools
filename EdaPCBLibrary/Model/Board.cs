﻿namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model.Collections;

    /// <summary>
    /// Clase que representa una placa.
    /// </summary>
    public sealed class Board: IVisitable {

        private readonly LayerStackup layerStackup = new LayerStackup();
        private readonly List<Element> elements = new List<Element>();
        private readonly Dictionary<Signal, List<Element>> mapSignalToElements = new Dictionary<Signal, List<Element>>();
        private readonly Dictionary<Element, Signal> mapElementToSignal = new Dictionary<Element, Signal>();
        private static readonly Dictionary<Component, Board> componentOwners = new Dictionary<Component, Board>();
        private readonly List<Component> components = new List<Component>();
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

        /// <summary>
        /// Afeigeix un component.
        /// </summary>
        /// <param name="element">El component a afeigir.</param>
        /// 
        public void AddComponent(Component component) {

            if (component == null)
                throw new ArgumentNullException("component");

            if (components.Contains(component))
                throw new InvalidOperationException("El componente ya pertenece a la placa.");

            if (componentOwners.ContainsKey(component))
                throw new InvalidOperationException("El componente ya pertenece a otra placa.");

            components.Add(component);
            componentOwners.Add(component, this);
        }

        /// <summary>
        /// Afegeix una conexio entre un element i un senyal.
        /// </summary>
        /// <param name="signal">La senyal.</param>
        /// <param name="element">El element a conectar.</param>
        /// 
        public void AddConnection(Signal signal, Element element) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (element == null)
                throw new ArgumentNullException("element");

            if (mapElementToSignal.ContainsKey(element))
                throw new InvalidOperationException("El elemento ya esta conectado.");

            List<Element> elementList;
            if (mapSignalToElements.TryGetValue(signal, out elementList)) {
                elementList = new List<Element>();
                mapSignalToElements.Add(signal, elementList);
            }
            elementList.Add(element);
            mapElementToSignal.Add(element, signal);
        }

        public void RemoveConnection(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (!mapElementToSignal.ContainsKey(element))
                throw new InvalidOperationException("El elemento no esta conectado a ninguna señal.");
        }

        public Signal GetSignal(string name) {

            if (signals != null)
                foreach (Signal signal in signals)
                    if (signal.Name == name)
                        return signal;
            return null;
        }

        public Layer GetLayer(LayerId id) {

            return layerStackup.GetLayer(id);
        }

        public LayerStackup LayerStackup {
            get {
                return layerStackup;
            }
        }

        public IEnumerable<Layer> Layers {
            get {
                return layerStackup.Layers;
            }
        }

        public bool HasComponents {
            get {
                return components.Count > 0;
            }
        }

        /// <summary>
        /// Obte la llista de components.
        /// </summary>
        /// 
        public IEnumerable<Component> Components {
            get {
                return components;
            }
        }

        public static Board BoardOf(Component component) {

            if (component == null)
                throw new ArgumentNullException("component");

            Board board;
            if (componentOwners.TryGetValue(component, out board))
                return board;
            else
                return null;
        }


        public bool HasParts {
            get {
                return parts.Count > 0;
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
