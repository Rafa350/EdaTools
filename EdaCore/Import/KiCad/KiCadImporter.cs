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

                    int number = tree.ValueAsInteger(layerNode[0]);
                    string name = tree.ValueAsString(layerNode[1]);
                    string type = tree.ValueAsString(layerNode[2]);

                    BoardSide side = GetLayerSide(name);

                    LayerFunction function = LayerFunction.Unknown;
                    if (type == "signal")
                        function = LayerFunction.Signal;
                    else if ((type == "user") && (name == "Edge.Cuts"))
                        function = LayerFunction.Outline;

                    Layer layer = new Layer(side, name, function);
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

            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                switch (tree.GetBranchName(childNode)) {
                    case "layer":
                        break;

                    case "tedit":
                        break;

                    case "tstamp":
                        break;

                    case "fp_text":
                        elements.Add(ParseComponentText(tree, childNode));
                        break;

                    case "fp_line":
                        break;

                    case "pad": 
                        elements.Add(ParseComponentPad(tree, childNode));
                        break;

                    default:
                        break;
                }
            }

            //string layerName = ProcessLayerDesignator(tree, tree.SelectBranch(node, "layer"));
            //Location location = ProcessLocationDesignator(tree, tree.SelectBranch(node, "at"));

            return new Component(componentName, elements);
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

            List<PartAttribute> attributes = new List<PartAttribute>();
            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                switch (tree.GetBranchName(childNode)) {
                    case "fp_text":
                        attributes.Add(ParsePartAttribute(tree, childNode));
                        break;
                }
            }

            string name = String.Format("{0}:{1}", componentName, partCount++);
            Part part = new Part(GetComponent(componentName), name, location.Position, location.Rotation);
            part.AddAttributes(attributes);
            return part;
        }

        private PartAttribute ParsePartAttribute(STree tree, SBranch node) {

            string name = tree.ValueAsString(node[1]);
            string value = tree.ValueAsString(node[2]);

            return new PartAttribute(name, value);
        }

        private string ParseLayerDesignator(STree tree, SBranch node) {

            return tree.ValueAsString(node[1]);
        }

        private Location ParseLocationDesignator(STree tree, SBranch node) {

            double x = tree.ValueAsDouble(node[1]);
            double y = tree.ValueAsDouble(node[2]);
            Point position = new Point((int)(x * 1000000.0), (int)(y * 1000000));

            Angle rotation = node.Count == 4 ? Angle.FromDegrees(tree.ValueAsDouble(node[3])) : Angle.Zero;

            return new Location(position, rotation);
        }

        private Size ParseSizeDesignator(STree tree, SBranch node) {

            double width = tree.ValueAsDouble(node[1]);
            double height = tree.ValueAsDouble(node[2]);
            return new Size((int)(width * 1000000.0), (int)(height * 1000000));
        }

        private LayerSet ParseLayerSetDesignator(STree tree, SBranch node) {

            StringBuilder layersList = new StringBuilder();
            for (int i = 1; i < node.Count; i++) {
                if (i > 1)
                    layersList.Append(',');
                string tag = tree.ValueAsString(node[i]);
                BoardSide side =
                    tag.Contains("Top") || tag.Contains("F.") ? BoardSide.Top :
                    tag.Contains("Bottom") ||  tag.Contains("B.") ? BoardSide.Bottom :
                    BoardSide.None;
                string name = Layer.GetName(side, tag);
                layersList.Append(name);
            }

            return LayerSet.Parse(layersList.ToString());
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

        private static BoardSide GetLayerSide(string name) {

            return
                name.Contains("Top") || name.Contains("F.") ? BoardSide.Top :
                name.Contains("Bottom") || name.Contains("B.") ? BoardSide.Bottom :
                BoardSide.None;
        }

        private static string GetLayerName(string name) {
        }

        private static LayerFunction GetLayerFunction(string name) {
        }
    }
}
