﻿namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;

    /// <summary>
    /// Clase que representa una placa.
    /// </summary>
    public sealed class Board: IVisitable {

        // Capes
        private readonly HashSet<Layer> layers = new HashSet<Layer>();
        private readonly Dictionary<Layer, Layer> layerPairs = new Dictionary<Layer, Layer>();
        private readonly Dictionary<Layer, HashSet<Element>> elementsOfLayer = new Dictionary<Layer, HashSet<Element>>();
        private readonly Dictionary<Element, HashSet<Layer>> layersOfElement = new Dictionary<Element, HashSet<Layer>>();

        // Senyals
        private readonly HashSet<Signal> signals = new HashSet<Signal>();
        private readonly Dictionary<Signal, HashSet<Tuple<IConectable, Part>>> itemsOfSignal = new Dictionary<Signal, HashSet<Tuple<IConectable, Part>>>();
        private readonly Dictionary<Tuple<IConectable, Part>, Signal> signalOfItem = new Dictionary<Tuple<IConectable, Part>, Signal>();

        // Blocs
        private readonly HashSet<Block> blocks = new HashSet<Block>();

        // Elements
        private readonly List<Element> elements = new List<Element>();

        // Components
        readonly private HashSet<Part> parts = new HashSet<Part>();

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

        #region Metodes per la gestio de parts

        /// <summary>
        /// Afegeix una component.
        /// </summary>
        /// <param name="part">El component a afeigir.</param>
        /// 
        public void AddPart(Part part) {

            if (part == null)
                throw new ArgumentNullException("part");

            if (!parts.Add(part))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}' ya esta asignado a esta la placa.", part.Name));
        }

        /// <summary>
        /// Elimina una peça de la placa.
        /// </summary>
        /// <param name="part">La peça a eliminar.</param>
        /// 
        public void RemovePart(Part part) {

            if (part == null)
                throw new ArgumentNullException("part");
        }


        public Part GetPart(string name, bool throwOnError = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            foreach (Part part in parts)
                if (part.Name == name)
                    return part;

            if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', no se encontro en esta placa.", name));

            return null;
        }

        #endregion


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

        #region Metodes per la gestio de blocs

        /// <summary>
        /// Afeigeix un bloc.
        /// </summary>
        /// <param name="block">El bloc a afeigir.</param>
        /// 
        public void AddBlock(Block block) {

            if (block == null)
                throw new ArgumentNullException("block");

            if (!blocks.Add(block))
                throw new InvalidOperationException(
                    String.Format("El bloque '{0}', ya esta asignado a esta placa.", block.Name));
        }

        #endregion

        #region Metodes per la gestio de capes

        /// <summary>
        /// Afegeix una capa a la placa.
        /// </summary>
        /// <param name="layer">La capa a afeigir.</param>
        /// 
        public void AddLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (!layers.Add(layer))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', ya esta asignada a esta placa.", layer.Name));
        }

        /// <summary>
        /// Obte una capa pel seu Id
        /// </summary>
        /// <param name="layerId">El identificador de la capa.</param>
        /// <param name="throwOnError">True si genera una excepcio en cas d'error.</param>
        /// <returns>La capa.</returns>
        /// 
        public Layer GetLayer(LayerId layerId, bool throwOnError = true) {

            foreach (Layer layer in layers)
                if (layer.Id == layerId)
                    return layer;

            if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro la capa con el ID '{0}'.", layerId.ToString()));

            return null;
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

            // Afegeix l'element al conjunt d'elements de la capa
            //
            HashSet<Element> elementSet;
            if (!elementsOfLayer.TryGetValue(layer, out elementSet)) {
                elementSet = new HashSet<Element>();
                elementsOfLayer.Add(layer, elementSet);
            }
            if (!elementSet.Add(element))
                if (elementSet.Contains(element))
                    throw new InvalidOperationException(
                        String.Format("La capa '{0}', ya contiene este elemento.", layer.Name));

            // Afegeix la capa al conjunt de capes del element
            //
            HashSet<Layer> layerSet;
            if (!layersOfElement.TryGetValue(element, out layerSet)) {
                layerSet = new HashSet<Layer>();
                layersOfElement.Add(element, layerSet);
            }
            if (!layerSet.Add(layer))
                throw new InvalidOperationException(
                    String.Format("El elemento ya esta contenido en la capa '{0}'.", layer.Name));
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

            HashSet<Element> elementSet;
            if (elementsOfLayer.TryGetValue(layer, out elementSet))
                return elementSet;
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

            HashSet<Layer> layerSet;
            if (layersOfElement.TryGetValue(element, out layerSet))
                return layerSet;
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

            HashSet<Layer> layerCollection;
            if (layersOfElement.TryGetValue(element, out layerCollection))
                return layerCollection.Contains(layer);

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

            HashSet<Layer> layerCollection;
            if (layersOfElement.TryGetValue(element, out layerCollection)) 
                return layerCollection.Overlaps(layers);

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

            if (!signals.Add(signal))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', ya esta asignada a esta placa.", signal.Name));
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
        /// Conecta un element conectable amb una senyal.
        /// </summary>
        /// <param name="signal">La senyal.</param>
        /// <param name="element">L'element a conectar.</param>
        /// <param name="part">El component del element.</param>
        /// 
        public void Connect(Signal signal, IConectable element, Part part = null) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (element == null)
                throw new ArgumentNullException("element");

            if (!signals.Contains(signal))
                throw new InvalidOperationException(
                    String.Format("La senyal '{0}', no esta asignada a esta placa.", signal.Name));

            Tuple<IConectable, Part> item = new Tuple<IConectable, Part>(element, part);

            if (signalOfItem.ContainsKey(item))
                throw new InvalidOperationException("El objeto ya esta conectado.");

            HashSet<Tuple<IConectable, Part>> elementSet;
            if (!itemsOfSignal.TryGetValue(signal, out elementSet)) {
                elementSet = new HashSet<Tuple<IConectable, Part>>();
                itemsOfSignal.Add(signal, elementSet);
            }
            elementSet.Add(item);
            signalOfItem.Add(item, signal);
        }

        /// <summary>
        /// Desconecta un element de la senyal.
        /// </summary>
        /// <param name="element">El element a desconectar.</param>
        /// <param name="part">El component del element.</param>
        /// 
        public void Disconnect(IConectable element, Part part = null) {

            if (element == null)
                throw new ArgumentNullException("element");

            Tuple<IConectable, Part> item = new Tuple<IConectable, Part>(element, part);

            if (!signalOfItem.ContainsKey(item))
                throw new InvalidOperationException("El objeto no esta conectado a ninguna señal.");
        }

        /// <summary>
        /// Obte la senyal conectada a un objecte.
        /// </summary>
        /// <param name="element">El objecte.</param>
        /// <param name="throwOnError">True si cal generar una excepcio en cas d'error.</param>
        /// <returns>La senyal. Null si no esta conectat.</returns>
        /// 
        public Signal GetSignal(IConectable element, Part part = null, bool throwOnError = true) {

            if (element == null)
                throw new ArgumentNullException("element");

            Tuple<IConectable, Part> item = new Tuple<IConectable, Part>(element, part);

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

            foreach (Signal signal in signals)
                if (signal.Name == name)
                    return signal;

            if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro la señal '{0}'.", name));

            return null;
        }

        /// <summary>
        /// Obte els items conectats a una senyal.
        /// </summary>
        /// <param name="signal">La senyal.</param>
        /// <returns>Els items conectats. Null si no hi ha cap.</returns>
        /// 
        public IEnumerable<Tuple<IConectable, Part>> GetConnectedItems(Signal signal) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (!signals.Contains(signal))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', no esta asignada a esta placa.", signal.Name));

            HashSet<Tuple<IConectable, Part>> itemSet;
            if (itemsOfSignal.TryGetValue(signal, out itemSet))
                return itemSet;
            else
                return null;
        }

        #endregion

        #region Metodes per operacions amb poligons

        /// <summary>
        /// Calcula el poligon d'una regio.
        /// </summary>
        /// <param name="region">L'element de tipus regio.</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public Polygon GetRegionPolygon(RegionElement region, double spacing, Matrix transformation) {

            if (region == null)
                throw new ArgumentNullException("region");

            Polygon regionPolygon = region.GetPolygon();
            regionPolygon.Transform(transformation);

            // Si estem en capes de senyal, cal generar els porus i termals
            //
            if (IsOnLayer(region, GetLayer(LayerId.Top)) ||
                IsOnLayer(region, GetLayer(LayerId.Bottom))) {

                IEnumerable<Layer> regionLayers = GetLayers(region);
                Signal regionSignal = GetSignal(region, null, false);

                spacing += region.Thickness / 2;
                List<Polygon> holePolygons = new List<Polygon>();

                Layer restrictLayer = GetLayer(LayerId.TopRestrict);

                // Procesa els elements de la placa en la mateixa capa que la regio o el la capa restrict
                //
                foreach (Element element in elements) {
                    if (element != region) {

                        // El element es en la mateixa capa que la regio
                        //
                        if (IsOnAnyLayer(element, regionLayers)) {
                            IConectable item = element as IConectable;
                            if ((item == null) || (GetSignal(item, null, false) != regionSignal)) {
                                Polygon elementPolygon = element.GetPourPolygon(spacing);
                                holePolygons.Add(elementPolygon);
                            }
                        }

                        // El element esta el la capa restrict
                        //
                        else if (IsOnLayer(element, restrictLayer)) {
                            Polygon elementPolygon = element.GetPolygon();
                            holePolygons.Add(elementPolygon);
                        }
                    }
                }

                // Procesa els elements dels components
                //
                foreach (Part part in parts) {
                    foreach (Element element in part.Elements) {

                        if ((element != region) &&
                            (IsOnAnyLayer(element, regionLayers) || IsOnLayer(element, restrictLayer))) {

                            PadElement padElement = element as PadElement;

                            // Si l'element no esta conectat a la mateixa senyal que la regio, genera un forat
                            //
                            if ((padElement == null) || (GetSignal(padElement, part, false) != regionSignal)) {
                                Polygon elementPolygon = element.GetPourPolygon(spacing);
                                elementPolygon.Transform(part.Transformation);
                                holePolygons.Add(elementPolygon);
                            }

                            // En es un pad i esta conectat per tant, genera un thermal
                            //
                            else if (padElement != null) {
                                Polygon thermalPolygon = padElement.GetThermalPolygon(spacing, 0.2);
                                thermalPolygon.Transform(part.Transformation);
                                foreach (Polygon polygon in thermalPolygon.Childs)
                                    holePolygons.Add(polygon);
                            }
                        }
                    }
                }

                return PolygonProcessor.ClipExtended(regionPolygon, holePolygons, PolygonProcessor.ClipOperation.Diference);
            }

            // Si no es capa de senyal no cal fer res mes
            //
            else
                return regionPolygon;
        }

        #endregion

        /// <summary>
        /// Obte un enumerador pels blocs.
        /// </summary>
        /// 
        public IEnumerable<Block> Blocks {
            get {
                return blocks;
            }
        }

        /// <summary>
        /// Obte un enumerador pels components.
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
