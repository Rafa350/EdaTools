﻿namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Model.BoardElements;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Identifica la cara de la placa.
    /// </summary>
    /// 
    public enum BoardSide {
        Unknown,
        Top,
        Inner,
        Bottom
    }

    /// <summary>
    /// Clase que representa una placa.
    /// </summary>
    /// 
    public sealed class Board: IVisitable {

        private static Dictionary<Block, Board> blockBoard = new Dictionary<Block, Board>();

        // Capes
        private readonly HashSet<Layer> layers = new HashSet<Layer>();
        private readonly Dictionary<Layer, Layer> layerPairs = new Dictionary<Layer, Layer>();

        // Senyals
        private readonly HashSet<Signal> signals = new HashSet<Signal>();
        private readonly Dictionary<Signal, HashSet<Tuple<IConectable, Part>>> itemsOfSignal = new Dictionary<Signal, HashSet<Tuple<IConectable, Part>>>();
        private readonly Dictionary<Tuple<IConectable, Part>, Signal> signalOfItem = new Dictionary<Tuple<IConectable, Part>, Signal>();

        // Blocs
        private readonly HashSet<Block> blocks = new HashSet<Block>();

        // Elements
        private readonly HashSet<BoardElement> elements = new HashSet<BoardElement>();

        // Components
        readonly private HashSet<Part> parts = new HashSet<Part>();

        private Point position;
        private Angle rotation;

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
        /// Afegeix un component.
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
        /// Afegeix una coleccio de components.
        /// </summary>
        /// <param name="parts">Els components a afeigir</param>
        /// 
        public void AddParts(IEnumerable<Part> parts) {

            if (parts == null)
                throw new ArgumentException("parts");

            foreach (var part in parts)
                AddPart(part);
        }

        /// <summary>
        /// Elimina una component de la placa.
        /// </summary>
        /// <param name="part">La peça a eliminar.</param>
        /// 
        public void RemovePart(Part part) {

            if (part == null)
                throw new ArgumentNullException("part");
        }

        /// <summary>
        /// Obte un component pel seu nom.
        /// </summary>
        /// <param name="name">El nom del component a buscar.</param>
        /// <param name="throwOnError">True si cal generar una exceptio si no el troba.</param>
        /// <returns>El component, o null si no el troba.</returns>
        /// 
        public Part GetPart(string name, bool throwOnError = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            foreach (var part in parts)
                if (part.Name == name)
                    return part;

            if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', no se encontro en esta placa.", name));

            return null;
        }

        #endregion

        #region Metodes per la gestion d'elements

        /// <summary>
        /// Afeigeix un element.
        /// </summary>
        /// <param name="element">L'element a afeigir.</param>
        /// 
        public void AddElement(BoardElement element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (!elements.Add(element))
                throw new InvalidOperationException("El elemento ya existe en la placa.");
        }

        /// <summary>
        /// Afegeig una col·leccio d'elements.
        /// </summary>
        /// <param name="elements">Els elements a afeigir.</param>
        /// 
        public void AddElements(IEnumerable<BoardElement> elements) {

            if (elements == null)
                throw new ArgumentNullException("elements");

            foreach (var element in elements)
                AddElement(element);
        }

        #endregion

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

            blockBoard.Add(block, this);
        }

        /// <summary>
        /// Elimina un block.
        /// </summary>
        /// <param name="block">El bloc a eliminar.</param>
        /// 
        public void RemoveBlock(Block block) {

            if (block == null)
                throw new ArgumentNullException("block");

            if (!blocks.Remove(block))
                throw new InvalidOperationException(
                    String.Format("El bloque '{0}', no esta asignado a esta placa.", block.Name));

            blockBoard.Remove(block);
        }

        /// <summary>
        /// Obte un bloc pel seu nom.
        /// </summary>
        /// <param name="name">El nom del bloc.</param>
        /// <param name="throwOnError">True si cal generar una excepcio si no el troba.</param>
        /// <returns>El bloc, o null si no el troba.</returns>
        /// 
        public Block GetBlock(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            foreach (var block in blocks)
                if (block.Name == name)
                    return block;

            if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("El bloque '{0}', no esta asignado a esta placa.", name));

            return null;
        }

        /// <summary>
        /// Obte la placa al que pertany el bloc
        /// </summary>
        /// <param name="block">L'element.</param>
        /// <returns>El bloc al que pertany, null si no pertany a cap.</returns>
        /// 
        internal static Board GetBoard(Block block) {

            if (block == null)
                throw new ArgumentNullException("block");

            if (blockBoard.TryGetValue(block, out Board board))
                return board;
            else
                return null;
        }

        #endregion

        #region Metodes per la gestio de capes

        /// <summary>
        /// Afegeix una capa a la placa. L'ordre en que s'afegeixen corresponen a l'apilament fisic de la placa.
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
        /// Elimina una capa de la placa.
        /// </summary>
        /// <param name="layer">La capa a eliminar.</param>
        /// 
        public void  RemoveLayer(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (!layers.Contains(layer))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', no pertenece a la placa.", layer.Name));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Defineix un parell de capes.
        /// </summary>
        /// <param name="layer1">Primera capa del parell.</param>
        /// <param name="layer2">Segona capa del parell.</param>
        /// 
        public void DefinePair(Layer layer1, Layer layer2) {

            if (layer1 == null)
                throw new ArgumentNullException("layer1");

            if (layer2 == null)
                throw new ArgumentNullException("layer2");

            if (layerPairs.ContainsKey(layer1))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', ya esta apareada.", layer1.Name));

            if (layerPairs.ContainsKey(layer2))
                throw new InvalidOperationException(
                    String.Format("La capa '{0}', ya esta apareada.", layer2.Name));

            layerPairs.Add(layer1, layer2);
            layerPairs.Add(layer2, layer1);
        }


        /// <summary>
        /// Anula la definicio de parell de capes
        /// </summary>
        /// <param name="layer1">Primera capa del parell.</param>
        /// <param name="layer2">Segona capa del parell.</param>
        /// 
        public void UndefinePair(Layer layer1, Layer layer2) {

            if (layer1 == null)
                throw new ArgumentNullException("layer1");

            if (layer2 == null)
                throw new ArgumentNullException("layer2");
        }

        /// <summary>
        /// Obte la capa aparellada d'una capa deperminada.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <returns>La capa aparellada.</returns>
        /// 
        public Layer GetLayerPair(Layer layer, bool throwOnError = false) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            Layer pairLayer;
            if (!layerPairs.TryGetValue(layer, out pairLayer)) {
                if (throwOnError)
                    throw new InvalidOperationException(
                        String.Format("La capa '{0}', no esta apareada con otra capa.", layer.Name));
                return null;
            }
            else
                return pairLayer;
        }

        /// <summary>
        /// Obte una capa pel seu nom
        /// </summary>
        /// <param name="name">El nom de la capa.</param>
        /// <param name="throwOnError">True si cal generar una excepcio si no el troba.</param>
        /// <returns>La capa.</returns>
        /// 
        public Layer GetLayer(string name, bool throwOnError = true) {

            foreach (Layer layer in layers)
                if (layer.Name == name)
                    return layer;

            if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro la capa con el nombre '{0}'.", name));

            return null;
        }

        /// <summary>
        /// Obte la llista de capes de senyal, ordenades de la capa TOP a la BOTTOM
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Layer> GetSignalLayers() {

            List<Layer> layers = new List<Layer>();
            foreach (var layer in this.layers) {
                if (layer.Function == LayerFunction.Signal)
                    layers.Add(layer);
            }
            return layers;
        }

        /// <summary>
        /// Obte la coleccio d'elements d'una capa.
        /// </summary>
        /// <param name="layer">La capa.</param>
        /// <param name="includeBlocks">Indica si cal incluir els elements dels blocs.</param>
        /// <returns>La coleccio d'elements.</returns>
        /// 
        public IEnumerable<BoardElement> GetElements(Layer layer, bool includeBlocks = true) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            List<BoardElement> list = new List<BoardElement>();

            foreach (var element in elements)
                if (element.IsOnLayer(layer))
                    list.Add(element);

            if (includeBlocks)
                foreach (var block in blocks)
                    foreach (var element in block.Elements)
                        if (element.IsOnLayer(layer))
                            list.Add(element);

            return list;
        }

        /// <summary>
        /// Obte la coleccio de capes d'un element.
        /// </summary>
        /// <param name="element">El element.</param>
        /// <returns>La coleccio de capes.</returns>
        /// 
        public IEnumerable<Layer> GetLayers(BoardElement element) {

            if (element == null)
                throw new ArgumentNullException("element");

            List<Layer> list = new List<Layer>();

            foreach (var layerId in element.LayerSet) {
                Layer layer = GetLayer(layerId, false);
                if (layer != null)
                    list.Add(layer);
            }

            return list;
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
        /// <param name="conectableElement">El objecte.</param>
        /// <param name="throwOnError">True si cal generar una excepcio en cas d'error.</param>
        /// <returns>La senyal o null si no esta conectat.</returns>
        /// 
        public Signal GetSignal(BoardElement element, Part part = null, bool throwOnError = true) {

            if (element == null)
                throw new ArgumentNullException("element");

            Signal signal = null;

            IConectable conectableElement = element as IConectable;
            if (conectableElement != null) {
                Tuple<IConectable, Part> item = new Tuple<IConectable, Part>(conectableElement, part);
                signalOfItem.TryGetValue(item, out signal);
            }

            if ((signal == null) && throwOnError)
                throw new InvalidOperationException("El item no esta conectado.");

            return signal;
        }

        /// <summary>
        /// Obte una senyal pel seu nom.
        /// </summary>
        /// <param name="name">Nom de la senyal.</param>
        /// <param name="throwOnError">True si cal generar una exepcio en cas d'error.</param>
        /// <returns>La senyal o null si no existeix.</returns>
        /// 
        public Signal GetSignal(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            foreach (var signal in signals)
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
        /// Obte el poligon del perfil de la placa. Es calcula amb es elements de la capa profile.
        /// </summary>
        /// <returns>El poligon.</returns>
        /// 
        public Polygon GetOutlinePolygon() {

            IEnumerable<BoardElement> elements = GetElements(GetLayer(Layer.ProfileName));
            List<Segment> segments = new List<Segment>();
            foreach (var element in elements) {
                if (element is LineElement) {
                    LineElement line = (LineElement)element;
                    segments.Add(new Segment(line.StartPosition, line.EndPosition));
                }
            }
            return PolygonProcessor.CreateFromSegments(segments);
        }

        /// <summary>
        /// Calcula el poligon d'una regio.
        /// </summary>
        /// <param name="region">L'element de tipus regio.</param>
        /// <param name="layer">La capa a procesar.</param>
        /// <param name="transformation">Transformacio a aplicar al poligon.</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public Polygon GetRegionPolygon(RegionElement region, Layer layer, Transformation transformation) {

            if (region == null)
                throw new ArgumentNullException("region");

            if (layer == null)
                throw new ArgumentNullException("layer");

            // Si el poligon no es troba en la capa d'interes, no cal fer res
            //
            if (region.IsOnLayer(layer)) {

                // Obte el poligon de la regio i el transforma si s'escau
                //
                Polygon regionPolygon = region.GetPolygon(layer.Side);
                regionPolygon = regionPolygon.Transformed(transformation);

                // Si estem en capes de senyal, cal generar els porus i termals
                //
                if ((layer.Name == Layer.TopName) || (layer.Name == Layer.BottomName)) {

                    Signal regionSignal = GetSignal(region, null, false);

                    int thicknessCompensation = 150000 + region.Thickness / 2;
                    List<Polygon> holePolygons = new List<Polygon>();

                    Layer restrictLayer = GetLayer(layer.Side == BoardSide.Top ? Layer.TopRestrictName : Layer.BottomRestrictName);
                    Layer profileLayer = GetLayer(Layer.ProfileName);

                    // Procesa els elements de la placa que es troben en la mateixa capa que 
                    // la regio, o en les capes restrict o profile.
                    //
                    foreach (var element in elements) {
                        if (element != region) {

                            // El element es en la mateixa capa que la regio
                            //
                            if (element.IsOnLayer(layer)) {

                                // Si no esta en la mateixa senyal que la regio, genera un forat.
                                //
                                if (GetSignal(element, null, false) != regionSignal) {
                                    int signalClearance = regionSignal == null ? 0 : regionSignal.Clearance;
                                    int clearance = thicknessCompensation + Math.Max(signalClearance, region.Clearance);
                                    Polygon elementPolygon = element.GetOutlinePolygon(layer.Side, clearance);
                                    holePolygons.Add(elementPolygon);
                                }
                            }

                            // El element esta el la capa restrict
                            //
                            else if (element.IsOnLayer(restrictLayer)) {
                                Polygon elementPolygon = element.GetPolygon(restrictLayer.Side);
                                holePolygons.Add(elementPolygon);
                            }

                            // El element esta el la capa profile
                            //
                            else if (element.IsOnLayer(profileLayer)) {
                                Polygon elementPolygon = element.GetOutlinePolygon(profileLayer.Side, 250000);
                                holePolygons.Add(elementPolygon);
                            }
                        }
                    }

                    // Procesa els elements dels components
                    //
                    foreach (var part in parts) {

                        // Obte la transformacio
                        //
                        Transformation localTransformation = part.GetLocalTransformation();

                        foreach (var element in part.Elements) {

                            if ((element != region) &&
                                (element.IsOnLayer(layer) || element.IsOnLayer(restrictLayer))) {

                                // Si l'element no esta conectat a la mateixa senyal que la regio, genera un forat
                                //
                                if (GetSignal(element, part, false) != regionSignal) {
                                    int clearance = thicknessCompensation + Math.Max(regionSignal.Clearance, region.Clearance);
                                    Polygon outlinePolygon = element.GetOutlinePolygon(layer.Side, clearance);
                                    outlinePolygon = outlinePolygon.Transformed(localTransformation);
                                    holePolygons.Add(outlinePolygon);
                                }

                                // En es un pad i esta conectat per tant, genera un thermal
                                //
                                else if (element is PadElement) {
                                    int signalClearance = regionSignal == null ? 0 : regionSignal.Clearance;
                                    int clearance = thicknessCompensation + Math.Max(signalClearance, region.Clearance);
                                    Polygon thermalPolygon = ((PadElement)element).GetThermalPolygon(layer.Side, clearance, 200000);
                                    thermalPolygon = thermalPolygon.Transformed(localTransformation);
                                    for (int i = 0; i < thermalPolygon.Childs.Length; i++)
                                        holePolygons.Add(thermalPolygon.Childs[i]);
                                }
                            }
                        }
                    }

                    // Genera el poligon amb els forats pertinents
                    //
                    return PolygonProcessor.ClipExtended(regionPolygon, holePolygons, PolygonProcessor.ClipOperation.Diference);
                }

                // Si no es capa de senyal no cal fer res mes, ja que no te forats
                //
                else
                    return regionPolygon;
            }
            else
                return null;
        }

        #endregion

        /// <summary>
        /// Obte o asigna la posicio de la placa.
        /// </summary>
        /// 
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        /// <summary>
        /// Obte el tamany de la placa, definit pel seu contingut.
        /// </summary>
        /// 
        public Size Size {
            get {
                Layer layer = GetLayer(Layer.ProfileName, false);
                if (layer != null) {
                    int minX = Int32.MaxValue;
                    int minY = Int32.MaxValue;
                    int maxX = Int32.MinValue;
                    int maxY = Int32.MinValue;
                    foreach (BoardElement element in GetElements(layer)) {
                        Rect r = element.GetBoundingBox(BoardSide.Top);
                        if (minX > r.MinX)
                            minX = r.MinX;
                        if (minY > r.MinY)
                            minY = r.MinY;
                        if (maxX < r.MaxX)
                            maxX = r.MaxX;
                        if (maxY < r.MaxY)
                            maxY = r.MaxY;
                    }

                    return new Size(maxX - minX, maxY - minY);
                }
                else
                    return new Size(100000000, 100000000);
            }
        }

        /// <summary>
        /// Obte o asigna l'angle de rotacio de la placa.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
            }
        }

        /// <summary>
        /// Indica si conte blocs
        /// </summary>
        /// 
        public bool HasBlocks {
            get {
                return (blocks != null) && (blocks.Count > 0);
            }
        }

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
        /// Indica si conte parts.
        /// </summary>
        /// 
        public bool HasParts {
            get {
                return (parts != null) && (parts.Count > 0);
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
        /// Indica si la placa conte senyals
        /// </summary>
        /// 
        public bool HasSignals {
            get {
                return (signals != null) && (signals.Count > 0);
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
        /// Indica si conte elements.
        /// </summary>
        /// 
        public bool HasElements {
            get {
                return (elements != null) && (elements.Count > 0);
            }
        }

        /// <summary>
        /// Obte un enunerador pels elements.
        /// </summary>
        /// 
        public IEnumerable<BoardElement> Elements {
            get {
                return elements;
            }
        }
    }
}
