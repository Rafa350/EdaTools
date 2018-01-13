namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa una placa.
    /// </summary>
    public sealed class Board: IVisitable {

        // llista de capes
        private readonly List<Layer> layers = new List<Layer>();
        private readonly Dictionary<Layer, Layer> layerPairs = new Dictionary<Layer, Layer>();
        private readonly Dictionary<Layer, List<Element>> elementsOfLayer = new Dictionary<Layer, List<Element>>();
        private readonly Dictionary<Element, List<Layer>> layersOfElement = new Dictionary<Element, List<Layer>>();

        // Llista de senyals
        private readonly List<Signal> signals = new List<Signal>();
        private readonly Dictionary<Signal, List<IConectable>> itemsOfSignal = new Dictionary<Signal, List<IConectable>>();
        private readonly Dictionary<IConectable, Signal> signalOfItem = new Dictionary<IConectable, Signal>();

        // Llista del blocs
        private readonly List<Block> blocks = new List<Block>();

        // Llista d'elements
        private readonly List<Element> elements = new List<Element>();

        // Llista de parts
        readonly private List<Part> parts = new List<Part>();

        /// <summary>
        /// Constructor del objecte amb els parametres per defecte.
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
        /// Afeigeix un bloc.
        /// </summary>
        /// <param name="block">El component a afeigir.</param>
        /// 
        public void AddBlock(Block block) {

            if (block == null)
                throw new ArgumentNullException("block");

            if (blocks.Contains(block))
                throw new InvalidOperationException(
                    String.Format("El bloque '{0}', ya esta asignado a esta placa.", block.Name));

            blocks.Add(block);
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
        /// <param name="layerId">El identificador de la capa.</param>
        /// <param name="throwOnError">True si genera una excepcio en cas d'error.</param>
        /// <returns>La capa.</returns>
        /// 
        public Layer GetLayer(LayerId layerId, bool throwOnError = true) {

            Layer layer = layers.Find(l => l.Id == layerId);
            if ((layer == null) && throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro la capa con el ID '{0}'.", layerId.ToString()));

            return layer;
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

            // Afegeix l'element a la llista d'elements de la capa
            //
            List<Element> elementList;
            if (!elementsOfLayer.TryGetValue(layer, out elementList)) {
                elementList = new List<Element>();
                elementsOfLayer.Add(layer, elementList);
            }
            if (elementList.Contains(element))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', ya contiene este elemento.", layer.Name));
            elementList.Add(element);

            // Afegeix la capa a la llista de capes del element
            //
            List<Layer> layerList;
            if (!layersOfElement.TryGetValue(element, out layerList)) {
                layerList = new List<Layer>();
                layersOfElement.Add(element, layerList);
            }
            if (layerList.Contains(layer))
                throw new InvalidOperationException(
                    String.Format("El elemento ya esta contenido en la capa '{0}'.", layer.Name));
            layerList.Add(layer);
        }

        public void Unplace(Element element) {

        }

        /// <summary>
        /// Obte la coleccio d'elements d'una capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <returns>La coleccio d'elements o null si no n'hi ha</returns>
        /// 
        public IEnumerable<Element> GetElements(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (!layers.Contains(layer))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', no esta asignada a esta placa.", layer.Name));

            if (elementsOfLayer.ContainsKey(layer))
                return elementsOfLayer[layer];
            else
                return null;
        }

        /// <summary>
        /// Obte la coleccio de capes d'un element.
        /// </summary>
        /// <param name="element">El element.</param>
        /// <returns>La coleccio de capes o null si n'hi ha.</returns>
        /// 
        public IEnumerable<Layer> GetLayers(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (layersOfElement.ContainsKey(element))
                return layersOfElement[element];
            else
                return null;
        }

        /// <summary>
        /// Comproba si un element pertany a una capa.
        /// </summary>
        /// <param name="element">El element.</param>
        /// <param name="layer">La capa.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public bool IsOnLayer(Element element, Layer layer) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (elementsOfLayer.ContainsKey(layer))
                return elementsOfLayer[layer].Contains(element);
            else
                return false;
        }

        /// <summary>
        /// Comprova si un element pertany a qualsevol capa de les especificades.
        /// </summary>
        /// <param name="element">El element.</param>
        /// <param name="layers">La coleccio de capes a verificar.</param>
        /// <returns>True si pertany a qualsevol de les capes, false si no pertany a cap.</returns>
        /// 
        public bool IsOnAnyLayer(Element element, IEnumerable<Layer> layers) {

            foreach (Layer layer in layers)
                if (IsOnLayer(element, layer))
                    return true;
            return false;
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

            if (itemsOfSignal.ContainsKey(signal))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', esta en uso y no puede ser retirada de la placa.", signal.Name));

            signals.Remove(signal);
        }

        /// <summary>
        /// Conecta un objece amb un senyal.
        /// </summary>
        /// <param name="signal">La senyal.</param>
        /// <param name="item">El objecte a conectar.</param>
        /// 
        public void Connect(Signal signal, IConectable item) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (item == null)
                throw new ArgumentNullException("item");

            if (!signals.Contains(signal))
                throw new InvalidOperationException(
                    String.Format("La senyal '{0}', no esta asignada a esta placa.", signal.Name));

            if (signalOfItem.ContainsKey(item))
                throw new InvalidOperationException("El objeto ya esta conectado.");

            List<IConectable> elementList;
            if (!itemsOfSignal.TryGetValue(signal, out elementList)) {
                elementList = new List<IConectable>();
                itemsOfSignal.Add(signal, elementList);
            }
            elementList.Add(item);
            signalOfItem.Add(item, signal);
        }

        /// <summary>
        /// Desconecta un objecte de la senyal.
        /// </summary>
        /// <param name="item">El element a desconectar.</param>
        /// 
        public void Disconnect(IConectable item) {

            if (item == null)
                throw new ArgumentNullException("item");

            if (!signalOfItem.ContainsKey(item))
                throw new InvalidOperationException("El objeto no esta conectado a ninguna señal.");
        }

        /// <summary>
        /// Obte la senyal conectada a un objecte.
        /// </summary>
        /// <param name="item">El objecte.</param>
        /// <param name="throwOnError">True si cal generar una excepcio en cas d'error.</param>
        /// <returns>La senyal. Null si no esta conectat.</returns>
        /// 
        public Signal GetSignal(IConectable item, bool throwOnError = true) {

            if (item == null)
                throw new ArgumentNullException("item");

            Signal signal;

            if (signalOfItem.TryGetValue(item, out signal))
                return signal;
            else {
                if (throwOnError)
                    throw new InvalidOperationException("El item no esta conectado.");
                return null;
            }
        }

        /// <summary>
        /// Obte una senyal pel seu nom.
        /// </summary>
        /// <param name="name">Nom de la senyal.</param>
        /// <param name="throwOnError">True si cal generar una exepcio en cas d'error.</param>
        /// <returns>La senyal. Null si no existeix.</returns>
        /// 
        public Signal GetSignal(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Signal signal = signals.Find(s => s.Name == name);
            if ((signal == null) && throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro la señal '{0}'.", name));

            return signal;
        }

        /// <summary>
        /// Obte els items conectats a una senyal.
        /// </summary>
        /// <param name="signal">La senyal.</param>
        /// <returns>Els items conectats. Null si no hi ha cap.</returns>
        /// 
        public IEnumerable<IConectable> GetConnectedItems(Signal signal) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (!signals.Contains(signal))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', no esta asignada a esta placa.", signal.Name));

            List<IConectable> items;
            if (itemsOfSignal.TryGetValue(signal, out items))
                return items;
            else
                return null;
        }

        #endregion

        /// <summary>
        /// Obte la llista de components.
        /// </summary>
        /// 
        public IEnumerable<Block> Blocks {
            get {
                return blocks;
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
        /// Obte un enumerador per les capes
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
