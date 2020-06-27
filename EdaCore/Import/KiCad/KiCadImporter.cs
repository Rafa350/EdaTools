namespace MikroPic.EdaTools.v1.Core.Import.KiCad {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Core.Import.KiCad.Infrastructure;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Core.Model.Net;

    public sealed class KiCadImporter: Importer {

        private readonly struct Location {

            private readonly Point _position;
            private readonly Angle _rotation;

            public Location(Point position, Angle rotation) {

                _position = position;
                _rotation = rotation;
            }

            public Point Position => _position;
            public Angle Rotation => _rotation;
        }

        private readonly Dictionary<string, Component> componentDict = new Dictionary<string, Component>();
        private readonly Dictionary<string, Signal> signalDict = new Dictionary<string, Signal>();

        private int partCount = 0;

        public override Board ReadBoard(Stream stream) {

            TextReader reader = new StreamReader(stream);
            string source = reader.ReadToEnd();

            SParser parser = new SParser();
            STree tree = parser.Parse(source);

            return ProcessBoard(tree);
        }

        public override Library ReadLibrary(Stream stream) {
            throw new System.NotImplementedException();
        }

        public override Net ReadNet(Stream stream) {
            throw new System.NotImplementedException();
        }

        private Board ProcessBoard(STree tree) {

            ProcessSignals(tree);
            ProcessComponents(tree);

            Board board = new Board();
            board.AddLayer(new Layer(BoardSide.None, "Pads", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.None, "Vias", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.None, "Drils", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.None, "Holes", LayerFunction.Unknown));

            board.AddSignals(signalDict.Values);
            board.AddComponents(componentDict.Values);
            board.AddLayers(ParseLayers(tree));
            board.AddParts(ParseParts(tree));

            return board;
        }

        /// <summary>
        /// Crea les senyals i les afegeix al diccionari..
        /// </summary>
        /// <param name="tree">El STree</param>
        /// 
        private void ProcessSignals(STree tree) {

            foreach (var netNode in tree.SelectBranches(tree.Root, "net")) {

                string name = tree.ValueAsString(netNode[2]);
                if (!string.IsNullOrEmpty(name)) {
                    Signal signal = new Signal(name);
                    signalDict.Add(signal.Name, signal);
                }
            }
        }
        
        /// <summary>
        /// Crea els components i els afegeix al diccionari.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// 
        private void ProcessComponents(STree tree) {

            foreach (var moduleNode in tree.SelectBranches(tree.Root, "module")) {
                string name = tree.ValueAsString(moduleNode[1]);
                if (!componentDict.ContainsKey(name)) {
                    Component component = ParseComponent(tree, moduleNode);
                    componentDict.Add(component.Name, component);
                }
            }
        }

        /// <summary>
        /// Procesa el node 'layers' i grea la llista de capes.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <returns>Un enumerador de les capes.</returns>
        /// 
        private IEnumerable<Layer> ParseLayers(STree tree) {

            SBranch layerListNode = tree.SelectBranch(tree.Root, "layers");
            if (layerListNode != null) {
                List<Layer> layers = new List<Layer>();
                foreach (var layerNode in layerListNode.Nodes.OfType<SBranch>()) {

                    string kcName = tree.ValueAsString(layerNode[1]);
                    string type = tree.ValueAsString(layerNode[2]);

                    BoardSide side = GetLayerSide(kcName);
                    LayerFunction function = type == "signal" ? LayerFunction.Signal : GetLayerFunction(kcName);
                    string tag = GetLayerTag(kcName);

                    Layer layer = new Layer(side, tag, function);
                    layers.Add(layer);
                }
                return layers;
            }
            else 
                return null;
        }


        /// <summary>
        /// Procesa un node 'module' per crear un component de la placa.
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node a procesar.</param>
        /// <returns>El component creat.</returns>
        /// 
        private Component ParseComponent(STree tree, SBranch node) {

            List<Element> elements = new List<Element>();

            string componentName = tree.ValueAsString(node[1]);
            Location location = ParseLocationDesignator(tree, tree.SelectBranch(node, "at"));

            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                switch (tree.GetBranchName(childNode)) {
                    case "layer":
                        break;

                    case "tedit":
                        break;

                    case "tstamp":
                        break;

                    case "fp_text": {
                        TextElement element = ParseComponentText(tree, childNode);
                        element.Rotation = element.Rotation - location.Rotation;
                        elements.Add(element);
                        break;
                    }

                    case "fp_line": {
                        LineElement element = ParseComponentLine(tree, childNode);
                        elements.Add(element);
                        break;
                    }

                    case "pad": {
                        PadElement element = ParseComponentPad(tree, childNode);
                        element.Rotation = element.Rotation - location.Rotation;
                        elements.Add(element);
                        break;
                    }

                    default:
                        break;
                }
            }

            return new Component(componentName, elements);
        }

        private LineElement ParseComponentLine(STree tree, SBranch node) {

            Location start = ParseLocationDesignator(tree, tree.SelectBranch(node, "start"));
            Location end = ParseLocationDesignator(tree, tree.SelectBranch(node, "end"));
            string layer = ParseLayerDesignator(tree, tree.SelectBranch(node, "layer"));
            int thickness = ParseThicknessDesignator(tree, tree.SelectBranch(node, "width"));

            return new LineElement(new LayerSet(layer), start.Position, end.Position, thickness, LineElement.CapStyle.Round);
        }

        /// <summary>
        /// Procesa un node 'fp_text' per crear un element text.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'element creat.</returns>
        /// 
        private TextElement ParseComponentText(STree tree, SBranch node) {

            string name = tree.ValueAsString(node[1]);
            string value = String.Format(">{0}", name);
            Location location = ParseLocationDesignator(tree, tree.SelectBranch(node, "at"));

            LayerSet layerSet = ParseLayerSetDesignator(tree, tree.SelectBranch(node, "layer"));
            if (name == "reference") {
                if (layerSet.Contains("Top.F_SilkS")) {
                    layerSet -= "Top.F_SilkS";
                    layerSet += "Top.Names";
                }
                else if (layerSet.Contains("Bottom.B_SilkS")) {
                    layerSet -= "Bottom.B_SilkS";
                    layerSet += "Bottom.Names";
                }
            }
            else if (name == "value") {
                if (layerSet.Contains("Top.F_Fab")) {
                    layerSet -= "Top.F_Fab";
                    layerSet += "Top.Values";
                }
                else if (layerSet.Contains("Bottom.B_Fab")) {
                    layerSet -= "Bottom.B_Fab";
                    layerSet += "Bottom.Values";
                }
            }

            return new TextElement(layerSet, location.Position, location.Rotation, 1000000, 100000, 
                HorizontalTextAlign.Center, VerticalTextAlign.Middle, value);
        }

        /// <summary>
        /// Procesa un node 'pad' per crear un element pad.
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'element pad creat.</returns>
        /// 
        private PadElement ParseComponentPad(STree tree, SBranch node) {

            string name = tree.ValueAsString(node[1]);
            Location location = ParseLocationDesignator(tree, tree.SelectBranch(node, "at"));
            Size size = ParseSizeDesignator(tree, tree.SelectBranch(node, "size"));
            LayerSet layerSet = ParseLayerSetDesignator(tree, tree.SelectBranch(node, "layers"));
            layerSet += "Pads";

            return new SmdPadElement(name, layerSet, location.Position, size, location.Rotation, Ratio.Zero);
        }

        /// <summary>
        /// Procesa els nodes module, per generar la llista de parts.
        /// </summary>
        /// <param name="tree">El Stree</param>
        /// <returns>Un enumerador dels parts creats.</returns>
        /// 
        private IEnumerable<Part> ParseParts(STree tree) {

            List<Part> parts = new List<Part>();

            foreach (var moduleNode in tree.SelectBranches(tree.Root, "module"))
                parts.Add(ParsePart(tree, moduleNode));

            return parts;
        }

        private Part ParsePart(STree tree, SBranch node) {

            string componentName = tree.ValueAsString(node[1]);

            Location location = ParseLocationDesignator(tree, tree.SelectBranch(node, "at"));
            bool flip = ParseSideDesignator(tree, tree.SelectBranch(node, "layer")) == BoardSide.Bottom;

            List<PartAttribute> attributes = new List<PartAttribute>();
            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                switch (tree.GetBranchName(childNode)) {
                    case "fp_text":
                        attributes.Add(ParsePartAttribute(tree, childNode));
                        break;
                }
            }

            string name = String.Format("{0}:{1}", componentName, partCount++);
            Part part = new Part(GetComponent(componentName), name, location.Position, location.Rotation, flip);
            part.AddAttributes(attributes);
            return part;
        }

        private PartAttribute ParsePartAttribute(STree tree, SBranch node) {

            string name = tree.ValueAsString(node[1]);
            string value = tree.ValueAsString(node[2]);

            return new PartAttribute(name, value);
        }

        private Location ParseLocationDesignator(STree tree, SBranch node) {

            double x = tree.ValueAsDouble(node[1]);
            double y = tree.ValueAsDouble(node[2]);
            Point position = new Point((int)(x * 1000000.0), (int)(y * 1000000));

            Angle rotation = node.Count == 4 ? Angle.FromDegrees(tree.ValueAsDouble(node[3])) : Angle.Zero;

            return new Location(position, rotation);
        }

        private BoardSide ParseSideDesignator(STree tree, SBranch node) {

            return tree.ValueAsString(node[1]) == "Top" ? BoardSide.Top : BoardSide.Body;
        }

        private Size ParseSizeDesignator(STree tree, SBranch node) {

            double width = tree.ValueAsDouble(node[1]);
            double height = tree.ValueAsDouble(node[2]);
            return new Size((int)(width * 1000000.0), (int)(height * 1000000));
        }

        private int ParseThicknessDesignator(STree tree, SBranch node) {

            double thickness = tree.ValueAsDouble(node[1]);
            return (int) (thickness * 1000000.0);
        }

        private string ParseLayerDesignator(STree tree, SBranch node) {

            string kcName = tree.ValueAsString(node[1]);

            string tag = GetLayerTag(kcName);
            BoardSide side = GetLayerSide(kcName);
            
            return Layer.GetName(side, tag);
        }

        private LayerSet ParseLayerSetDesignator(STree tree, SBranch node) {

            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < node.Count; i++) {
                if (i > 1)
                    sb.Append(',');

                string kcName = tree.ValueAsString(node[i]);

                string tag = GetLayerTag(kcName);
                BoardSide side = GetLayerSide(kcName);
                
                string name = Layer.GetName(side, tag);
                sb.Append(name);
            }

            return LayerSet.Parse(sb.ToString());
        }

        /// <summary>
        /// Obte el component especificat.
        /// </summary>
        /// <param name="name">El nom del component.</param>
        /// <returns>El component.</returns>
        /// 
        private Component GetComponent(string name) {

            return componentDict[name];
        }

        /// <summary>
        /// Obte la cara de la placa a partir d'un nom de capa.
        /// </summary>
        /// <param name="kcName">Nom de la capa.</param>
        /// <returns>La cara.</returns>
        /// 
        private static BoardSide GetLayerSide(string kcName) {

            return
                kcName.Contains("Top") || kcName.Contains("F.") ? BoardSide.Top :
                kcName.Contains("Bottom") || kcName.Contains("B.") ? BoardSide.Bottom :
                BoardSide.None;
        }

        /// <summary>
        /// Obte el tag de la capa a partir del seu nom.
        /// </summary>
        /// <param name="kcName">Nom de la capa.</param>
        /// <returns>El tag.</returns>
        /// 
        private static string GetLayerTag(string kcName) {

            switch (kcName) {
                case "Top":
                case "Bottom":
                    return "Copper";

                case "F.Paste":
                case "B.Paste":
                    return "Cream";

                case "F.Adhes":
                case "B.Adhes":
                    return "Glue";

                case "F.Mask":
                case "B.Mask":
                    return "Stop";

                case "F.Fab":
                case "B.Fab":
                    return "Document";

                case "F.SilkS":
                case "B.SilkS":
                    return "Place";

                case "Edge.Cuts":
                    return "Profile";

                default:
                    return kcName.Replace('.', '_');
            }
        }

        /// <summary>
        /// Obte la funcio de la capa a partir del seu nom.
        /// </summary>
        /// <param name="kcName">El nom.</param>
        /// <returns>La funcio.</returns>
        /// 
        private static LayerFunction GetLayerFunction(string kcName) {

            switch (kcName) {
                case "Top":
                case "Bottom":
                    return LayerFunction.Signal;

                default:
                    return LayerFunction.Unknown;
            }
        }
    }
}
