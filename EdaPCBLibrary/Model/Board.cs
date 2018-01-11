namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa una placa.
    /// </summary>
    public sealed class Board: IVisitable {

        // Gestio de capes
        private readonly List<Layer> layers = new List<Layer>();
        private readonly Dictionary<Layer, List<Element>> mapLayerToElements = new Dictionary<Layer, List<Element>>();
        private readonly Dictionary<Element, List<Layer>> mapElementToLayers = new Dictionary<Element, List<Layer>>();

        // Gestio de senyals
        private readonly List<Signal> signals = new List<Signal>();
        private readonly Dictionary<Signal, List<IConectable>> mapSignalToItems = new Dictionary<Signal, List<IConectable>>();
        private readonly Dictionary<IConectable, Signal> mapItemToSignal = new Dictionary<IConectable, Signal>();

        // Gestio dels blocs
        private static readonly Dictionary<Component, Board> componentOwners = new Dictionary<Component, Board>();
        private readonly List<Component> components = new List<Component>();

        private readonly List<Element> elements = new List<Element>();
        readonly private List<Part> parts = new List<Part>();

        /// <summary>
        /// Constructor per defecte.
        /// </summary>
        /// 
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

            parts.Add(part);
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
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', ya esta asignado a esta placa.", component.Name));

            if (componentOwners.ContainsKey(component))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', ya esta asignado a otra placa.", component.Name));

            components.Add(component);
            componentOwners.Add(component, this);
        }

        #region Metodes per la gestio de capes

        /// <summary>
        /// Afegeix una capa a la placa.
        /// </summary>
        /// <param name="layer">La capa a afeigir.</param>
        /// 
        public void AddLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (layers.Contains(layer))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', ya esta asignada a esta placa.", layer.Name));

            layers.Add(layer);
        }

        /// <summary>
        /// Obte una capa pel seu Id
        /// </summary>
        /// <param name="id">El id de la capa.</param>
        /// <returns>La capa.</returns>
        /// 
        public Layer GetLayer(LayerId id) {

            return layers.Find(a => a.Id == id);
        }

        /// <summary>
        /// Asigna un element a una capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <param name="element">El element.</param>
        /// 
        public void Place(Layer layer, Element element) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (element == null)
                throw new ArgumentNullException("element");

            if (!layers.Contains(layer))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', no esta asignada a esta placa.", layer.Name));

            List<Element> elementList;
            if (!mapLayerToElements.TryGetValue(layer, out elementList)) {
                elementList = new List<Element>();
                mapLayerToElements.Add(layer, elementList);
            }

            if (elementList.Contains(element))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', ya contiene este elemento.", layer.Name));

            elementList.Add(element);
        }

        public void Unplace(Layer layer, Element element) {

        }

        public void Unplace(Element element) {

        }

        public IEnumerable<Element> GetElements(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (!layers.Contains(layer))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', no esta asignada a esta placa.", layer.Name));

            if (mapLayerToElements.ContainsKey(layer))
                return mapLayerToElements[layer];
            else
                return new List<Element>();
        }

        #endregion

        #region Metodes de gestio de les senyals

        /// <summary>
        /// Afeigeix una senyal a la placa.
        /// </summary>
        /// <param name="signal">La senyal a afeigir.</param>
        /// 
        public void AddSignal(Signal signal) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (signals.Contains(signal))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', ya esta asignada a esta placa.", signal.Name));

            signals.Add(signal);
        }

        /// <summary>
        /// Retira una senyal de la placa.
        /// </summary>
        /// <param name="signal">La senyal a retirar.</param>
        /// 
        public void RemoveSignal(Signal signal) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (!signals.Contains(signal))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', no esta asignada a esta placa.", signal.Name));

            if (mapSignalToItems.ContainsKey(signal))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', esta en uso y no puede ser retirada de la placa.", signal.Name));

            signals.Remove(signal);
        }

        /// <summary>
        /// Afegeix una conexio entre un objece i un senyal.
        /// </summary>
        /// <param name="signal">La senyal.</param>
        /// <param name="item">El objecte a conectar.</param>
        /// 
        public void AddConnection(Signal signal, IConectable item) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (item == null)
                throw new ArgumentNullException("item");

            if (!signals.Contains(signal))
                throw new InvalidOperationException(
                    String.Format("La senyal '{0}', no esta asignada a esta placa.", signal.Name));

            if (mapItemToSignal.ContainsKey(item))
                throw new InvalidOperationException("El objeto ya esta conectado.");

            List<IConectable> elementList;
            if (!mapSignalToItems.TryGetValue(signal, out elementList)) {
                elementList = new List<IConectable>();
                mapSignalToItems.Add(signal, elementList);
            }
            elementList.Add(item);
            mapItemToSignal.Add(item, signal);
        }

        /// <summary>
        /// Elimina la conexio a un objecte.
        /// </summary>
        /// <param name="item">El element a desconectar.</param>
        /// 
        public void RemoveConnection(IConectable item) {

            if (item == null)
                throw new ArgumentNullException("item");

            if (!mapItemToSignal.ContainsKey(item))
                throw new InvalidOperationException("El objeto no esta conectado a ninguna señal.");
        }

        /// <summary>
        /// Obte la senyal conectada a un objecte.
        /// </summary>
        /// <param name="item">El objecte.</param>
        /// <returns>La senyal. Null si no esta conectat.</returns>
        /// 
        public Signal GetSignal(IConectable item) {

            Signal signal;

            if (mapItemToSignal.TryGetValue(item, out signal))
                return signal;
            else
                return null;
        }

        /// <summary>
        /// Obte els items conectats a una senyal.
        /// </summary>
        /// <param name="signal">La senyal.</param>
        /// <returns>Els items conectats.</returns>
        /// 
        public IEnumerable<IConectable> GetConnectedItems(Signal signal) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (!signals.Contains(signal))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', no esta asignada a esta placa.", signal.Name));

            List<IConectable> items;
            if (mapSignalToItems.TryGetValue(signal, out items))
                return items;
            else
                return null;
        }

        #endregion

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
        /// Obte un enumerados per les capes
        /// </summary>
        /// 
        public IEnumerable<Layer> Layers {
            get {
                return layers;
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
