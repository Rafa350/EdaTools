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

        private readonly struct ModuleNodeInfo {

            private readonly string _name;
            private readonly Point _position;
            private readonly Angle _rotation;
            private readonly BoardSide _side;

            public ModuleNodeInfo(string name, Point position, Angle rotation, BoardSide side) {

                _name = name;
                _position = position;
                _rotation = rotation;
                _side = side;
            }

            public string Name => _name;
            public Point Position => _position;
            public Angle Rotation => _rotation;
            public BoardSide Side => _side;
        }

        private readonly struct TextNodeInfo {

            private readonly Point _position;
            private readonly Angle _rotation;
            private readonly string _layer;
            private readonly string _type;
            private readonly string _text;

            public TextNodeInfo(Point position, Angle rotation, string layer, string type, string text) {

                _position = position;
                _rotation = rotation;
                _layer = layer;
                _type = type;
                _text = text;
            }

            public Point Position => _position;
            public Angle Rotation => _rotation;
            public string Layer => _layer;
            public string Type => _type;
            public string Text => _text;
        }

        private readonly struct LineNodeInfo {

            private readonly Point _start;
            private readonly Point _end;
            private readonly int _thickness;
            private readonly string _layer;

            public LineNodeInfo(Point start, Point end, int thickness, string layer) {

                _start = start;
                _end = end;
                _thickness = thickness;
                _layer = layer;
            }

            public Point Start => _start;
            public Point End => _end;
            public int Thickness => _thickness;
            public string Layer => _layer;
        }

        private readonly struct CircleNodeInfo {

            private readonly Point _center;
            private readonly int _radius;
            private readonly int _thickness;
            private readonly string _layer;

            public CircleNodeInfo(Point center, int radius, int thickness, string layer) {

                _center = center;
                _radius = radius;
                _thickness = thickness;
                _layer = layer;
            }

            public Point Center => _center;
            public int Radius => _radius;
            public int Thickness => _thickness;
            public string Layer => _layer;
        }

        private readonly struct PadNodeInfo {

            private readonly string _name;
            private readonly string _type;
            private readonly Point _position;
            private readonly Angle _rotation;
            private readonly Size _size;
            private readonly Ratio _roundness;
            private readonly int _drill;

            public PadNodeInfo(string name, string type, Point position, Size size, Angle rotation, int drill, Ratio roundness) {

                _name = name;
                _type = type;
                _position = position;
                _size = size;
                _rotation = rotation;
                _drill = drill;
                _roundness = roundness;
            }

            public string Name => _name;
            public string Type => _type;
            public Point Position => _position;
            public Size Size => _size;
            public Angle Rotation => _rotation;
            public int Drill => _drill;
            public Ratio Roundness => _roundness;
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
            
            throw new NotImplementedException();
        }

        public override Net ReadNet(Stream stream) {
            
            throw new NotImplementedException();
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
        /// Procesa el node 'layers' i genera la llista de capes.
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

            ModuleNodeInfo moduleInfo = ParseModuleNode(tree, node);

            List<Element> elements = new List<Element>();
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

                    case "fp_circle":
                        elements.Add(ParseComponentCircle(tree, childNode));
                        break;

                    case "fp_line": 
                        elements.Add( ParseComponentLine(tree, childNode));
                        break;

                    case "pad": 
                        elements.Add(ParseComponentPad(tree, childNode));
                        break;

                    default:
                        break;
                }
            }

            foreach (var element in elements)
                if (element is IRotation r)
                    r.Rotation -= moduleInfo.Rotation;

            return new Component(moduleInfo.Name, elements);
        }

        private Element ParseComponentLine(STree tree, SBranch node) {

            LineNodeInfo lineInfo = ParseLineNode(tree, node);

            return new LineElement(
                new LayerSet(lineInfo.Layer), 
                lineInfo.Start, 
                lineInfo.End, 
                lineInfo.Thickness, 
                LineElement.CapStyle.Round);
        }

        private Element ParseComponentCircle(STree tree, SBranch node) {

            CircleNodeInfo circleIndo = ParseCircleNode(tree, node);

            return new CircleElement(
                new LayerSet(circleIndo.Layer), 
                circleIndo.Center, 
                circleIndo.Radius, 
                circleIndo.Thickness, 
                true);
        }

        /// <summary>
        /// Procesa un node 'fp_text' per crear un element text.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'element creat.</returns>
        /// 
        private Element ParseComponentText(STree tree, SBranch node) {

            TextNodeInfo textInfo = ParseTextNode(tree, node);

            return new TextElement(
                new LayerSet(textInfo.Layer), 
                textInfo.Position, 
                textInfo.Rotation, 
                1000000, 
                100000, 
                HorizontalTextAlign.Center, 
                VerticalTextAlign.Middle, 
                textInfo.Text);
        }

        /// <summary>
        /// Procesa un node 'pad' per crear un element pad.
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node a procesar.</param>
        /// <returns>L'element pad creat.</returns>
        /// 
        private Element ParseComponentPad(STree tree, SBranch node) {

            PadNodeInfo padInfo = ParsePadNode(tree, node);

            switch (padInfo.Type) {
                
                case "smd": {
                    LayerSet layerSet = ParseLayerSetDesignator(tree, tree.SelectBranch(node, "layers"));
                    return new SmdPadElement(
                        padInfo.Name, 
                        layerSet, 
                        padInfo.Position, 
                        padInfo.Size, 
                        padInfo.Rotation, 
                        padInfo.Roundness);
                }

                case "thru_hole": {
                    LayerSet layerSet = ParseLayerSetDesignator(tree, tree.SelectBranch(node, "layers"));
                    layerSet += "Drills";
                    layerSet += "Pads";

                    ThPadElement.ThPadShape shape;
                    switch (tree.ValueAsString(node[3])) {
                        case "oval":
                            shape = padInfo.Size.Width == padInfo.Size.Height ? ThPadElement.ThPadShape.Circle : ThPadElement.ThPadShape.Oval;
                            break;

                        case "rect":
                            shape = ThPadElement.ThPadShape.Square;
                            break;

                        default:
                            shape = ThPadElement.ThPadShape.Circle;
                            break;
                    }

                    return new ThPadElement(
                        padInfo.Name, 
                        layerSet, 
                        padInfo.Position, 
                        padInfo.Rotation, 
                        padInfo.Size.Width, 
                        shape, 
                        padInfo.Drill);
                }

                case "np_thru_hole":
                    return new HoleElement(
                        new LayerSet("Holes"), 
                        padInfo.Position, 
                        padInfo.Drill);

                default:
                    throw new InvalidDataException("Tipo de pad no reconicido.");
            }
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

        /// <summary>
        /// Procesa un modul.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>El part generat.</returns>
        /// 
        private Part ParsePart(STree tree, SBranch node) {

            ModuleNodeInfo moduleInfo = ParseModuleNode(tree, node);

            List<PartAttribute> attributes = new List<PartAttribute>();
            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                switch (tree.GetBranchName(childNode)) {
                    case "fp_text":
                        attributes.Add(ParsePartAttribute(tree, childNode));
                        break;
                }
            }

            string name = String.Format("{0}:{1}", moduleInfo.Name, partCount++);
            bool flip = moduleInfo.Side == BoardSide.Bottom;
            Part part = new Part(GetComponent(moduleInfo.Name), name, moduleInfo.Position, moduleInfo.Rotation, flip);
            part.AddAttributes(attributes);
            return part;
        }

        private PartAttribute ParsePartAttribute(STree tree, SBranch node) {

            string name = tree.ValueAsString(node[1]);
            string value = tree.ValueAsString(node[2]);

            bool visible = false;
            if (name == "value") {
                name = "VALUE";
                visible = true;
            }
            else if (name == "reference") {
                name = "NAME";
                visible = true;
            }

            return new PartAttribute(name, value, visible);
        }

        /// <summary>
        /// Procesa un node 'module'
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node</param>
        /// <returns>Els parametres del node.</returns>
        /// 
        private ModuleNodeInfo ParseModuleNode(STree tree, SBranch node) {

            string name = tree.ValueAsString(node[1]);

            SBranch layerNode = tree.SelectBranch(node, "layer");
            string layer = tree.ValueAsString(layerNode[1]);
            BoardSide side = layer == "Bottom" ? BoardSide.Bottom : BoardSide.Top;

            SBranch atNode = tree.SelectBranch(node, "at");
            double x = tree.ValueAsDouble(atNode[1]);
            double y = tree.ValueAsDouble(atNode[2]);
            double r = atNode.Count == 4 ? tree.ValueAsDouble(atNode[3]) : 0;
            Point position = new Point((int)(x * 1000000.0), (int)(y * 1000000.0));
            Angle rotation = Angle.FromDegrees(r);

            return new ModuleNodeInfo(
                name,
                position,
                rotation,
                side);
        }

        /// <summary>
        /// Procesa un node 'pad'
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>El parametres del node.</returns>
        /// 
        private PadNodeInfo ParsePadNode(STree tree, SBranch node) {

            string name = tree.ValueAsString(node[1]);
            string type = tree.ValueAsString(node[2]);
            string shape = tree.ValueAsString(node[3]);

            SBranch atNode = tree.SelectBranch(node, "at");
            double x = tree.ValueAsDouble(atNode[1]);
            double y = tree.ValueAsDouble(atNode[2]);
            double r = atNode.Count == 4 ? tree.ValueAsDouble(atNode[3]) : 0;
            Point position = new Point((int)(x * 1000000.0), (int)(y * 1000000.0));
            Angle rotation = Angle.FromDegrees(r);

            SBranch sizeNode = tree.SelectBranch(node, "size");
            double sx = tree.ValueAsDouble(sizeNode[1]);
            double sy = tree.ValueAsDouble(sizeNode[2]);
            Size size = new Size((int)(sx * 1000000.0), (int)(sy * 1000000));

            SBranch drillNode = tree.SelectBranch(node, "drill");
            int drill = drillNode == null ? 0 : (int)(tree.ValueAsDouble(drillNode[1]) * 1000000.0);

            SBranch roundnestNode = tree.SelectBranch(node, "roundrect_rratio");
            Ratio roundnest = roundnestNode == null ? 
                Ratio.Zero : 
                Ratio.FromValue((int)(tree.ValueAsDouble(roundnestNode[1]) * 2000.0));

            return new PadNodeInfo(name, type, position, size, rotation, drill, roundnest);
        }

        /// <summary>
        /// Procesa un node 'fp_text'
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>Els parametres del node.</returns>
        /// 
        private TextNodeInfo ParseTextNode(STree tree, SBranch node) {

            string layer;
            string text;

            string type = tree.ValueAsString(node[1]);

            SBranch atNode = tree.SelectBranch(node, "at");
            double x = tree.ValueAsDouble(atNode[1]);
            double y = tree.ValueAsDouble(atNode[2]);
            double r = atNode.Count == 4 ? tree.ValueAsDouble(atNode[3]) : 0;
            Point position = new Point((int)(x * 1000000.0), (int)(y * 1000000.0));
            Angle rotation = Angle.FromDegrees(r);

            SBranch layerNode = tree.SelectBranch(node, "layer");
            string kcLayer = tree.ValueAsString(layerNode[1]);

            if (type == "reference") {
                layer = Layer.GetName(GetLayerSide(kcLayer), "Names");
                text = ">NAME";
            }
            else if (type == "value") {
                layer = Layer.GetName(GetLayerSide(kcLayer), "Values");
                text = ">VALUE";
            }
            else {
                layer = Layer.GetName(GetLayerSide(kcLayer), GetLayerTag(kcLayer));
                text = tree.ValueAsString(node[2]).Replace('%', '>');
            }

            return new TextNodeInfo(position, rotation, layer, type, text);
        }

        /// <summary>
        /// Procesa un node 'ft_line'
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node.</param>
        /// <returns>Els parametres del node.</returns>
        /// 
        private LineNodeInfo ParseLineNode(STree tree, SBranch node) {

            SBranch startNode = tree.SelectBranch(node, "start");
            double sx = tree.ValueAsDouble(startNode[1]);
            double sy = tree.ValueAsDouble(startNode[2]);
            Point start = new Point((int)(sx * 1000000.0), (int)(sy * 1000000.0));

            SBranch endNode = tree.SelectBranch(node, "end");
            double ex = tree.ValueAsDouble(endNode[1]);
            double ey = tree.ValueAsDouble(endNode[2]);
            Point end = new Point((int)(ex * 1000000.0), (int)(ey * 1000000.0));

            SBranch layerNode = tree.SelectBranch(node, "layer");
            string kcLayer = tree.ValueAsString(layerNode[1]);
            string layer = Layer.GetName(GetLayerSide(kcLayer), GetLayerTag(kcLayer));

            SBranch widthNode = tree.SelectBranch(node, "width");
            int thickness = (int)(tree.ValueAsDouble(widthNode[1]) * 1000000.0);

            return new LineNodeInfo(start, end, thickness, layer);
        }

        /// <summary>
        /// Procesa un node 'ft_circle'
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node.</param>
        /// <returns>Els parametres del node.</returns>
        /// 
        private CircleNodeInfo ParseCircleNode(STree tree, SBranch node) {

            SBranch centerNode = tree.SelectBranch(node, "center"); ;
            double cx = tree.ValueAsDouble(centerNode[1]);
            double cy = tree.ValueAsDouble(centerNode[2]);
            Point center = new Point((int)(cx * 1000000.0), (int)(cy * 1000000.0));

            SBranch endNode = tree.SelectBranch(node, "end");
            double ex = tree.ValueAsDouble(endNode[1]);
            double ey = tree.ValueAsDouble(endNode[2]);
            int radius = (int)(Math.Sqrt(Math.Pow(ex - cx, 2) + Math.Pow(ey - cy, 2)) * 1000000.0);

            SBranch layerNode = tree.SelectBranch(node, "layer");
            string kcLayer = tree.ValueAsString(layerNode[1]);
            string layer = Layer.GetName(GetLayerSide(kcLayer), GetLayerTag(kcLayer));

            SBranch widthNode = tree.SelectBranch(node, "width");
            int thickness = (int)(tree.ValueAsDouble(widthNode[1]) * 1000000.0);

            return new CircleNodeInfo(center, radius, thickness, layer);
        }

        private LayerSet ParseLayerSetDesignator(STree tree, SBranch node) {

            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < node.Count; i++) {
                if (i > 1)
                    sb.Append(',');

                string kcName = tree.ValueAsString(node[i]);
                if (kcName.Contains("*.Cu")) 
                    sb.Append("Top.Copper, Bottom.Copper");
                else if (kcName.Contains("*.Mask")) 
                    sb.Append("Top.Stop, Bottom.Stop");                
                else {
                    string tag = GetLayerTag(kcName);
                    BoardSide side = GetLayerSide(kcName);
                    string name = Layer.GetName(side, tag);
                    sb.Append(name);
                }
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
