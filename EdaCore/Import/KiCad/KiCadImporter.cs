using System;
using System.IO;
using System.Linq;
using System.Text;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Geometry.Utils;
using MikroPic.EdaTools.v1.Core.Import.KiCad.Infrastructure;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Net;

namespace MikroPic.EdaTools.v1.Core.Import.KiCad {

    public sealed class KiCadImporter: IImporter {

        private const double _scale = 1000000.0;
        private readonly Matrix2D _m = Matrix2D.CreateScale(_scale, -_scale);
        private int _partCount = 0;

        /// <inheritdoc/>
        /// 
        public Board ReadBoard(string fileName) {

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
             
                var reader = new StreamReader(stream);
                string source = reader.ReadToEnd();

                var parser = new SParser();
                var tree = parser.Parse(source);

                return ProcessBoard(tree);
            }
        }

        /// <inheritdoc/>
        /// 
        public Library ReadLibrary(string fileName) {
            
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// 
        public Net ReadNet(string fileName) {
            
            throw new NotImplementedException();
        }

        private Board ProcessBoard(STree tree) {

            var board = new Board();
            board.AddLayer(new Layer(BoardSide.None, "Pads", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.None, "Vias", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.None, "Drils", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.None, "Holes", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.Top, "Names", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.Bottom, "Values", LayerFunction.Unknown));

            ParseLayers(tree, board);
            ParseNets(tree, board);
            ParseContent(tree, board);

            return board;
        }

        /// <summary>
        /// Procesa els nodes 'net'
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="board">L aplaca.</param>
        /// 
        private void ParseNets(STree tree, Board board) {

            foreach (var netNode in tree.SelectBranches(tree.Root, "net")) {
                string name = tree.ValueAsString(netNode[2]);
                if (!string.IsNullOrEmpty(name)) {
                    var signal = new Signal(name);
                    board.AddSignal(signal);
                }
            }
        }
        
        /// <summary>
        /// Procesa el node 'layers'.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="board">La placa on afeigir les capes.</param>
        /// 
        private void ParseLayers(STree tree, Board board) {

            var layersNode = tree.SelectBranch(tree.Root, "layers");
            if (layersNode != null) {
                foreach (var layerNode in layersNode.Nodes.OfType<SBranch>()) {

                    string kcName = tree.ValueAsString(layerNode[1]);
                    string type = tree.ValueAsString(layerNode[2]);

                    BoardSide side = GetLayerSide(kcName);
                    LayerFunction function = type == "signal" ? LayerFunction.Signal : GetLayerFunction(kcName);
                    string tag = GetLayerTag(kcName);

                    var layer = new Layer(side, tag, function);
                    board.AddLayer(layer);
                }
            }
        }

        /// <summary>
        /// Procesa els nodes 'module'
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// 
        private void ParseContent(STree tree, Board board) {

            foreach (var childNode in (tree.Root as SBranch).Nodes.OfType<SBranch>()) {
                switch (tree.GetBranchName(childNode)) {
                    case "module":
                        ParseModule(tree, childNode, board);
                        break;

                    case "via":
                        ParseVia(tree, childNode, board);
                        break;

                    case "segment":
                    case "gr_line":
                        ParseSegment(tree, childNode, board);
                        break;
                }
            }
        }

        /// <summary>
        /// Procesa un node 'via'
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node</param>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseVia(STree tree, SBranch node, Board board) {

            // Obte la posicio
            //
            var atNode = tree.SelectBranch(node, "at");
            var p = new Vector2D(tree.ValueAsDouble(atNode[1]), tree.ValueAsDouble(atNode[2])) * _m;
            var position = new Point((int)p.X, (int)p.Y);

            // Obte el tamany
            //
            var sizeNode = tree.SelectBranch(node, "size");
            int size = (int)(tree.ValueAsDouble(sizeNode[1]) * _scale);

            // Obte el diametre del forat.
            //
            var drillNode = tree.SelectBranch(node, "drill");
            int drill = (int)(tree.ValueAsDouble(drillNode[1]) * _scale);

            // Obte el conjunt de capes.
            //
            LayerSet layerSet = ParseLayerSetDesignator(tree, tree.SelectBranch(node, "layers"));
            layerSet += "Vias";
            layerSet += "Drills";

            var via = new ViaElement(layerSet, position, size, drill, ViaElement.ViaShape.Circle);
            board.AddElement(via);
        }

        /// <summary>
        /// Procesa un n ode 'segment'
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseSegment(STree tree, SBranch node, Board board) {

            // Obte la posicio inicial
            //
            var startNode = tree.SelectBranch(node, "start");
            var s = new Vector2D(tree.ValueAsDouble(startNode[1]), tree.ValueAsDouble(startNode[2])) * _m;
            var start = new Point((int)s.X, (int)s.Y);

            // Obte la posicio final
            //
            var endNode = tree.SelectBranch(node, "end");
            var e = new Vector2D(tree.ValueAsDouble(endNode[1]), tree.ValueAsDouble(endNode[2])) * _m;
            var end = new Point((int)e.X, (int)e.Y);

            // Obte el gruix de linia
            //
            var widthNode = tree.SelectBranch(node, "width");
            int thickness = (int)(tree.ValueAsDouble(widthNode[1]) * _scale);

            // Obte el conjunt de capes.
            //
            var layerNode = tree.SelectBranch(node, "layer");
            string kcLayer = tree.ValueAsString(layerNode[1]);
            string layer = Layer.GetName(GetLayerSide(kcLayer), GetLayerTag(kcLayer));

            var element = new LineElement(new LayerSet(layer), start, end, thickness, LineElement.CapStyle.Round);
            board.AddElement(element);
        }

        /// <summary>
        /// Procesa un node 'module'.
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node a procesar.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void ParseModule(STree tree, SBranch node, Board board) {

            // Obte el nom 
            //
            string name = tree.ValueAsString(node[1]);

            // Obte la posicio i la rotacio
            //
            var atNode = tree.SelectBranch(node, "at");
            var p = new Vector2D(tree.ValueAsDouble(atNode[1]), tree.ValueAsDouble(atNode[2])) * _m;
            var position = new Point((int)p.X, (int)p.Y);
            double r = atNode.Count == 4 ? tree.ValueAsDouble(atNode[3]) : 0;
            var rotation = Angle.FromDegrees(r);

            // Obte la cara de la placa.
            //
            var layerNode = tree.SelectBranch(node, "layer");
            string layer = tree.ValueAsString(layerNode[1]);
            BoardSide side = layer == "Bottom" ? BoardSide.Bottom : BoardSide.Top;

            /// Si no existeix com component, el crea
            /// 
            var componentName = String.Format("{0}", name);
            var component = board.GetComponent(componentName, false);
            if (component == null) {

                component = new Component(componentName);
                board.AddComponent(component);

                foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                    switch (tree.GetBranchName(childNode)) {
                        case "fp_text":
                            ParseModuleText(tree, childNode, component);
                            break;

                        case "fp_circle":
                            ParseModuleCircle(tree, childNode, component);
                            break;

                        case "fp_line":
                            ParseModuleLine(tree, childNode, component);
                            break;

                        case "fp_arc":
                            ParseModuleArc(tree, childNode, component);
                            break;

                        case "pad":
                            ParseModulePad(tree, childNode, component);
                            break;
                    }
                }

                // Correccio de coordinades
                //
                if (!rotation.IsZero)
                    foreach (var element in component.Elements)
                        if (element is IRotation e)
                            e.Rotation -= rotation;
            }

            var partName = String.Format("{0}:{1}", name, _partCount++);
            var part = new Part(component, partName, position, rotation, side == BoardSide.Bottom);
            board.AddPart(part);

            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                switch (tree.GetBranchName(childNode)) {
                    case "fp_text":
                        ParseModuleText(tree, childNode, part);
                        break;
                }
            }
        }

        /// <summary>
        /// Procesa un node 'fp_line'
        /// </summary>
        /// <param name="tree">STree.</param>
        /// <param name="node">El node</param>
        /// <param name="component">El component.</param>
        /// 
        private void ParseModuleLine(STree tree, SBranch node, Component component) {

            // Obte el punt inicial
            //
            var startNode = tree.SelectBranch(node, "start");
            var s = new Vector2D(tree.ValueAsDouble(startNode[1]), tree.ValueAsDouble(startNode[2])) * _m;
            var start = new Point((int)s.X, (int)s.Y);

            // Obte el punt final.
            //
            var endNode = tree.SelectBranch(node, "end");
            var e = new Vector2D(tree.ValueAsDouble(endNode[1]), tree.ValueAsDouble(endNode[2])) * _m;
            var end = new Point((int)e.X, (int) e.Y);

            // Obte el conjunt de capes.
            //
            var layerNode = tree.SelectBranch(node, "layer");
            string kcLayer = tree.ValueAsString(layerNode[1]);
            string layer = Layer.GetName(GetLayerSide(kcLayer), GetLayerTag(kcLayer));

            // Obte el gruix de linia
            //
            var widthNode = tree.SelectBranch(node, "width");
            int thickness = (int)(tree.ValueAsDouble(widthNode[1]) * _scale);

            var element = new LineElement(new LayerSet(layer), start, end, thickness, LineElement.CapStyle.Round);
            component.AddElement(element);
        }

        /// <summary>
        /// Procesa un node 'fp_arc'
        /// </summary>
        /// <param name="tree">STree.</param>
        /// <param name="node">El node</param>
        /// <param name="component">El component.</param>
        /// 
        private void ParseModuleArc(STree tree, SBranch node, Component component) {

            // Obte el punt inicial
            //
            var centerNode = tree.SelectBranch(node, "start");
            var c = new Vector2D(tree.ValueAsDouble(centerNode[1]), tree.ValueAsDouble(centerNode[2])) * _m;
            var center = new Point((int)c.X, (int)c.Y);

            // Obte el punt final
            //
            var endNode = tree.SelectBranch(node, "end");
            var s = new Vector2D(tree.ValueAsDouble(endNode[1]), tree.ValueAsDouble(endNode[2])) * _m;
            var start = new Point((int)s.X, (int)s.Y);

            // Obte l'angle
            //
            var angleNode = tree.SelectBranch(node, "angle");
            double a = tree.ValueAsDouble(angleNode[1]);
            Angle angle = Angle.FromDegrees(a);

            // Obte el conjunt de capes
            //
            var layerNode = tree.SelectBranch(node, "layer");
            string kcLayer = tree.ValueAsString(layerNode[1]);
            string layer = Layer.GetName(GetLayerSide(kcLayer), GetLayerTag(kcLayer));

            // Obte l'amplada de linia
            //
            var widthNode = tree.SelectBranch(node, "width");
            int thickness = (int)(tree.ValueAsDouble(widthNode[1]) * _scale);

            var element = new ArcElement(new LayerSet(layer), start, ArcUtils.EndPosition(center, start, angle),
                thickness, angle, LineElement.CapStyle.Flat);
            component.AddElement(element);
        }

        /// <summary>
        /// Procesa un node 'fp_circle'
        /// </summary>
        /// <param name="tree">STree</param>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// 
        private void ParseModuleCircle(STree tree, SBranch node, Component component) {

            // Obte el centre
            //
            var startNode = tree.SelectBranch(node, "center");
            var s = new Vector2D(tree.ValueAsDouble(startNode[1]), tree.ValueAsDouble(startNode[2])) * _m;
            var center = new Point((int)s.X, (int)s.Y);

            // Obte la posicio final del vector del radi. El radi es el modul del vector
            //
            var endNode = tree.SelectBranch(node, "end");
            var e = new Vector2D(tree.ValueAsDouble(endNode[1]), tree.ValueAsDouble(endNode[2])) * _m;           
            int radius = (int)(Math.Sqrt(Math.Pow(e.X - s.X, 2) + Math.Pow(e.Y - s.Y, 2)));

            // Obte el conjunt de capes
            //
            var layerNode = tree.SelectBranch(node, "layer");
            string kcLayer = tree.ValueAsString(layerNode[1]);
            string layer = Layer.GetName(GetLayerSide(kcLayer), GetLayerTag(kcLayer));

            // Obte l'amplada de linia
            //
            var widthNode = tree.SelectBranch(node, "width");
            int thickness = (int)(tree.ValueAsDouble(widthNode[1]) * _scale);

            // Obte el indicador de relleno
            //
            var fillNode = tree.SelectBranch(node, "fill");
            bool fill = (fillNode != null) && (tree.ValueAsString(fillNode[1]) == "solid");

            var element = new CircleElement(new LayerSet(layer), center, radius, thickness, fill);
            component.AddElement(element);
        }

        /// <summary>
        /// Procesa un node 'fp_text'.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// 
        private void ParseModuleText(STree tree, SBranch node, Component component) {

            string layer;
            string text;

            string type = tree.ValueAsString(node[1]);

            // Obte la posicio i la rotacio
            //
            var atNode = tree.SelectBranch(node, "at");
            var p = new Vector2D(tree.ValueAsDouble(atNode[1]), tree.ValueAsDouble(atNode[2])) * _m;
            double r = atNode.Count == 4 ? tree.ValueAsDouble(atNode[3]) : 0;
            var position = new Point((int)p.X, (int)p.Y);
            var rotation = Angle.FromDegrees(r);

            // Obte els efectes
            //
            HorizontalTextAlign horizontalAlign = HorizontalTextAlign.Center;
            VerticalTextAlign verticalAlign = VerticalTextAlign.Middle;
            int thickness = 125000;
            int height = 1000000;
            var effectsNode = tree.SelectBranch(node, "effects");
            if (effectsNode != null) {
                var fontNode = tree.SelectBranch(effectsNode, "font");
                if (fontNode != null) {
                    var sizeNode = tree.SelectBranch(fontNode, "size");
                    height = (int)(tree.ValueAsDouble(sizeNode[2]) * _scale);
                    var thicknessNode = tree.SelectBranch(fontNode, "thickness");
                    thickness = (int)(tree.ValueAsDouble(thicknessNode[1]) * _scale);
                }
                var justifyNode = tree.SelectBranch(effectsNode, "justify");
                if (justifyNode != null) {
                    var h = tree.ValueAsString(justifyNode[1]);
                    switch (h) {
                        case "left":
                            horizontalAlign = HorizontalTextAlign.Left;
                            break;
                        case "right":
                            horizontalAlign = HorizontalTextAlign.Right;
                            break;
                    }
                    var v = tree.ValueAsString(justifyNode[2]);
                    switch (v) {
                        case "top":
                            verticalAlign = VerticalTextAlign.Top;
                            break;
                        case "bottom":
                            verticalAlign = VerticalTextAlign.Bottom;
                            break;
                    }
                }
            }

            var layerNode = tree.SelectBranch(node, "layer");
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

            var element = new TextElement(new LayerSet(layer), position, rotation, height, 
                thickness, horizontalAlign, verticalAlign, text);
            component.AddElement(element);
        }

        /// <summary>
        /// Procesa un node 'fp_text'.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="part">El part.</param>
        /// 
        private void ParseModuleText(STree tree, SBranch node, Part part) {

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
            else if (name == "user")
                return;

            var attribute = new PartAttribute(name, value, visible);
            part.AddAttribute(attribute);
        }

        /// <summary>
        /// Procesa un node 'pad'.
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node a procesar.</param>
        /// <param name="component">El component.</param>
        /// 
        private void ParseModulePad(STree tree, SBranch node, Component component) {

            string name = tree.ValueAsString(node[1]);
            string padType = tree.ValueAsString(node[2]);
            string shapeType = tree.ValueAsString(node[3]);

            // Obte la posicio i la rotacio
            //
            var atNode = tree.SelectBranch(node, "at");
            var p = new Vector2D(tree.ValueAsDouble(atNode[1]), tree.ValueAsDouble(atNode[2])) * _m;
            double r = atNode.Count == 4 ? tree.ValueAsDouble(atNode[3]) : 0;
            Point position = new Point((int)p.X, (int)p.Y);
            Angle rotation = Angle.FromDegrees(r);

            var sizeNode = tree.SelectBranch(node, "size");
            double sx = tree.ValueAsDouble(sizeNode[1]);
            double sy = tree.ValueAsDouble(sizeNode[2]);
            Size size = new Size((int)(sx * _scale), (int)(sy * _scale));

            var drillNode = tree.SelectBranch(node, "drill");
            int drill = drillNode == null ? 0 : (int)(tree.ValueAsDouble(drillNode[1]) * _scale);

            var roundnessNode = tree.SelectBranch(node, "roundrect_rratio");
            Ratio roundness = roundnessNode == null ?
                Ratio.Zero :
                Ratio.FromValue((int)(tree.ValueAsDouble(roundnessNode[1]) * 2000.0));

            Element element;
            switch (padType) {               
                case "smd": {
                    LayerSet layerSet = ParseLayerSetDesignator(tree, tree.SelectBranch(node, "layers"));
                    element = new SmdPadElement(name, layerSet, position, size, rotation, roundness);
                }
                break;

                case "thru_hole": {
                    LayerSet layerSet = ParseLayerSetDesignator(tree, tree.SelectBranch(node, "layers"));
                    layerSet += "Drills";
                    layerSet += "Pads";

                    ThPadElement.ThPadShape shape;
                    switch (shapeType) {
                        case "oval":
                            shape = size.Width == size.Height ? ThPadElement.ThPadShape.Circle : ThPadElement.ThPadShape.Oval;
                            break;

                        case "rect":
                            shape = ThPadElement.ThPadShape.Square;
                            break;

                        default:
                            shape = ThPadElement.ThPadShape.Circle;
                            break;
                    }

                    element = new ThPadElement(name, layerSet, position, rotation, size.Width, shape, drill);
                }
                break;

                case "np_thru_hole":
                    element = new HoleElement(new LayerSet("Holes"), position, drill);
                    break;

                default:
                    throw new InvalidDataException("Tipo de pad no reconicido.");
            }

            if (element != null)
                component.AddElement(element);
        }

        private LayerSet ParseLayerSetDesignator(STree tree, SBranch node) {

            var sb = new StringBuilder();
            for (int i = 1; i < node.Count; i++) {
                if (i > 1)
                    sb.Append(',');

                string kcName = tree.ValueAsString(node[i]);

                if (kcName.Contains("*.Cu") || kcName.Contains("F&B.Cu")) 
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
                case "F.Cu":
                case "B.Cu":
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

                case "F.CrtYd":
                case "B.CrtYd":
                    return "Keepout";

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
                case "F.Cu":
                case "B.Cu":
                    return LayerFunction.Signal;

                default:
                    return LayerFunction.Unknown;
            }
        }
    }
}
