﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
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
        private Dictionary<int, Signal> _signals = new Dictionary<int, Signal>();

        /// <inheritdoc/>
        /// 
        public Board ReadBoard(string fileName) {

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
             
                var reader = new StreamReader(stream);
                var source = reader.ReadToEnd();

                var parser = new SParser();
                var tree = parser.Parse(source);

                return ProcessBoard(tree);
            }
        }

        /// <inheritdoc/>
        /// 
        public Library ReadLibrary(string fileName) {

            var sb = new StringBuilder();
            sb.Append('(');
            var fNames = Directory.EnumerateFiles(fileName, "*.kicad_mod");
            foreach (var fName in fNames) {

                using (var stream = new FileStream(fName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                
                    var reader = new StreamReader(stream);
                    sb.Append(reader.ReadToEnd());
                }
            }
            sb.Append(')');

            var parser = new SParser();
            var tree = parser.Parse(sb.ToString());

            return ProcessLibrary(tree, Path.GetFileName(fileName));
        }

        /// <inheritdoc/>
        /// 
        public Net ReadNet(string fileName) {
            
            throw new NotImplementedException();
        }

        /// <summary>
        /// Carrega un component desde una llibreria.
        /// </summary>
        /// <param name="componentName">Nom del component.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void LoadComponent(string componentName, Board board) {

            var fileName = GetModuleFileName(componentName);
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {

                var reader = new StreamReader(stream);
                var source = reader.ReadToEnd();
                reader.Close();

                var parser = new SParser();
                var tree = parser.Parse(source);

                var libraryName = componentName.Split(':')[0];
                var library = new Library(libraryName);
                ProcessModule(tree, tree.Root as SBranch, library);

                board.AddComponents(library.Components);
            }
        }

        /// <summary>
        /// Procesa una placa.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <returns>La placa.</returns>
        /// 
        private Board ProcessBoard(STree tree) {

            var board = new Board();

            // Afegeix les capes basiques que no existeixen en la placa a importar
            //
            board.AddLayer(new Layer(BoardSide.None, "Pads", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.None, "Vias", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.None, "Drils", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.None, "Holes", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.Top, "Names", LayerFunction.Unknown));
            board.AddLayer(new Layer(BoardSide.Bottom, "Values", LayerFunction.Unknown));

            // Procesa les capes
            //
            var layersNode = tree.SelectBranch(tree.Root, "layers");
            foreach (var layerNode in layersNode.Nodes.OfType<SBranch>())
                ProcessLayer(tree, layerNode, board);

            // Procesa les senyals
            //
            foreach (var netNode in tree.SelectBranches(tree.Root, "net")) 
                ProcessSignal(tree, netNode, board);

            // Procesa els components referenciats en llibreries externes
            //
            foreach (var moduleNode in tree.SelectBranches(tree.Root, "module")) {
                string componentName = tree.ValueAsString(moduleNode[1]);
                if (board.GetComponent(componentName, false) == null) 
                    LoadComponent(componentName, board);
            }

            // Procesa el contingut
            //
            foreach (var childNode in (tree.Root as SBranch).Nodes.OfType<SBranch>()) {
                switch (tree.GetBranchName(childNode)) {
                    case "module":
                        ProcessPart(tree, childNode, board);
                        break;

                    case "via":
                        ProcessVia(tree, childNode, board);
                        break;

                    case "gr_line":
                    case "segment":
                        ProcessLine(tree, childNode, board);
                        break;

                    case "zone":
                        throw new NotImplementedException();
                        break;
                }
            }

            return board;
        }

        /// <summary>
        /// Procesa una llibreria
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="libraryName">El nom de la llibreria.</param>
        /// <returns>La llibreria.</returns>
        /// 
        private Library ProcessLibrary(STree tree, string libraryName) {

            var library = new Library(libraryName);

            foreach (var childNode in (tree.Root as SBranch).Nodes.OfType<SBranch>()) 
                ProcessModule(tree, childNode, library);

            return library;
        }

        /// <summary>
        /// Procesa una capa.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void ProcessLayer(STree tree, SBranch node, Board board) {

            string name = tree.ValueAsString(node[1]);
            string type = tree.ValueAsString(node[2]);

            BoardSide side = GetLayerSide(name);
            LayerFunction function = type == "signal" ? LayerFunction.Signal : GetLayerFunction(name);
            string tag = GetLayerTag(name);

            var layer = new Layer(side, tag, function);
            board.AddLayer(layer);
        }

        /// <summary>
        /// Procesa una senal.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void ProcessSignal(STree tree, SBranch node, Board board) {

            int id = tree.ValueAsInteger(node[1]);
            string name = tree.ValueAsString(node[2]);

            if (String.IsNullOrEmpty(name))
                name = "unnamed_signal";

            var signal = new Signal(name);
            board.AddSignal(signal);

            _signals.Add(id, signal);
        }

        /// <summary>
        /// Obte un element via
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node</param>
        /// <returns>L'element creat.</returns>
        /// 
        private void ProcessVia(STree tree, SBranch node, Board board) {

            var position = ParsePoint(tree, tree.SelectBranch(node, "at"));
            var size = ParseMeasure(tree, tree.SelectBranch(node, "size"));
            var drill = ParseMeasure(tree, tree.SelectBranch(node, "drill"));
            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layers"));
            
            layerSet += "Vias";
            layerSet += "Drills";

            var via = new ViaElement(layerSet, position, size, drill, ViaElement.ViaShape.Circle);

            var netNode = tree.SelectBranch(node, "net");
            if (netNode != null) {
                int netId = tree.ValueAsInteger(netNode[1]);
                board.Connect(_signals[netId], via);
            }

            board.AddElement(via);
        }

        /// <summary>
        /// Obte un component.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node</param>
        /// <param name="library">La llibreria</param>
        /// 
        private void ProcessModule(STree tree, SBranch node, Library library) {

            string name = tree.ValueAsString(node[1]);

            var component = new Component(String.Format("{0}:{1}", library.Name, name));

            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                switch (tree.GetBranchName(childNode)) {
                    case "fp_text":
                        ProcessText(tree, childNode, component);
                        break;

                    case "fp_circle":
                        ProcessCircle(tree, childNode, component);
                        break;

                    case "fp_rectangle":
                        throw new NotImplementedException();

                    case "fp_line":
                        ProcessLine(tree, childNode, component);
                        break;

                    case "fp_arc":
                        ProcessArc(tree, childNode, component);
                        break;

                    case "pad":
                        ProcessPadOrHole(tree, childNode, component);
                        break;
                }
            }

            library.AddComponent(component);
        }

        /// <summary>
        /// Obte un part.
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node a procesar.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void ProcessPart(STree tree, SBranch node, Board board) {

            string name = tree.ValueAsString(node[1]);

            var (position, rotation) = ParseLocation(tree, tree.SelectBranch(node, "at"));

            var layerNode = tree.SelectBranch(node, "layer");
            string layer = tree.ValueAsString(layerNode[1]);
            BoardSide side = layer == "Bottom" ? BoardSide.Bottom : BoardSide.Top;

            var component = board.GetComponent(name);
            var partName = String.Format("{0}:{1}", name, _partCount++);

            // Procesa els atributs
            //
            List<PartAttribute> attributes = null;
            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                if (tree.GetBranchName(childNode) == "fp_text") {

                    string attrValue = tree.ValueAsString(childNode[2]);

                    bool attrVisible = childNode.Count == 6; // Si no te el node 'hide' es visible

                    var (attrPosition, attrRotation) = ParseLocation(tree, tree.SelectBranch(childNode, "at"));
                    attrRotation -= rotation;

                    PartAttribute attribute = null;
                    switch (tree.ValueAsString(childNode[1])) {

                        case "value":
                            attribute = new PartAttribute("VALUE", partName, attrVisible);
                            break;

                        case "reference":
                            attribute = new PartAttribute("NAME", attrValue, attrVisible);
                            partName = attrValue;
                            break;

                        case "user":
                            break;
                    }

                    if (attribute != null) {
                        attribute.Position = attrPosition;
                        attribute.Rotation = attrRotation;
                        if (attributes == null)
                            attributes = new List<PartAttribute>();
                        attributes.Add(attribute);
                    }
                }
            }

            var part = new Part(component, partName, position, rotation, side == BoardSide.Bottom);
            if (attributes != null)
                part.AddAttributes(attributes);

            // Procesa les senyals
            //
            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                if (tree.GetBranchName(childNode) == "pad") {
                    var netNode = tree.SelectBranch(childNode, "net");
                    if (netNode != null) {

                        string padName = tree.ValueAsString(childNode[1]);
                        int netId = tree.ValueAsInteger(netNode[1]);
                        
                        var pad = part.GetPad(padName);
                        var signal = _signals[netId];
                        try {
                            board.Connect(signal, pad, part);
                        }
                        catch {

                        }
                    }
                }
            }
             
            board.AddPart(part);
        }

        /// <summary>
        /// Procesa una linia
        /// </summary>
        /// <param name="tree">STree.</param>
        /// <param name="node">El node</param>
        /// <param name="board">La placa.</param>
        /// 
        private void ProcessLine(STree tree, SBranch node, Board board) {

            var start = ParsePoint(tree, tree.SelectBranch(node, "start"));
            var end = ParsePoint(tree, tree.SelectBranch(node, "end"));
            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layer"));
            int thickness = ParseMeasure(tree, tree.SelectBranch(node, "width"));

            var element = new LineElement(layerSet, start, end, thickness, LineElement.CapStyle.Round);
            board.AddElement(element);

            var netNode = tree.SelectBranch(node, "net");
            if (netNode != null) {
                int netId = tree.ValueAsInteger(netNode[1]);
                board.Connect(_signals[netId], element);
            }
        }

        /// <summary>
        /// Obte un element line.
        /// </summary>
        /// <param name="tree">STree.</param>
        /// <param name="node">El node</param>
        /// <param name="component">El component.</param>
        /// 
        private void ProcessLine(STree tree, SBranch node, Component component) {

            var start = ParsePoint(tree, tree.SelectBranch(node, "start"));
            var end = ParsePoint(tree, tree.SelectBranch(node, "end"));
            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layer"));
            int thickness = ParseMeasure(tree, tree.SelectBranch(node, "width"));

            var element = new LineElement(layerSet, start, end, thickness, LineElement.CapStyle.Round);

            component.AddElement(element);
        }

        /// <summary>
        /// Obte un element arc.
        /// </summary>
        /// <param name="tree">STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// 
        private void ProcessArc(STree tree, SBranch node, Component component) {

            var center = ParsePoint(tree, tree.SelectBranch(node, "start"));
            var start = ParsePoint(tree, tree.SelectBranch(node, "end"));
            var angle = ParseAngle(tree, tree.SelectBranch(node, "angle"));
            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layer"));
            int thickness = ParseMeasure(tree, tree.SelectBranch(node, "width"));

            var element = new ArcElement(layerSet, start, ArcUtils.EndPosition(center, start, angle),
                thickness, angle, LineElement.CapStyle.Flat);

            component.AddElement(element);
        }

        /// <summary>
        /// Obte un element circle.
        /// </summary>
        /// <param name="tree">STree</param>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// 
        private void ProcessCircle(STree tree, SBranch node, Component component) {

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

            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layer"));
            int thickness = ParseMeasure(tree, tree.SelectBranch(node, "width"));

            // Obte el indicador de relleno
            //
            var fillNode = tree.SelectBranch(node, "fill");
            bool fill = (fillNode != null) && (tree.ValueAsString(fillNode[1]) == "solid");

            var element = new CircleElement(layerSet, center, radius, thickness, fill);

            component.AddElement(element);
        }

        /// <summary>
        /// Obte un element text.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// 
        private void ProcessText(STree tree, SBranch node, Component component) {

            string type = tree.ValueAsString(node[1]);
            var (position, rotation) = ParseLocation(tree, tree.SelectBranch(node, "at"));

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
                    height = ParseMeasure(tree, tree.SelectBranch(fontNode, "size"));
                    thickness = ParseMeasure(tree, tree.SelectBranch(fontNode, "thickness"));
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
            string layerName = tree.ValueAsString(layerNode[1]);

            string layer;
            string text;

            if (type == "reference") {
                layer = Layer.GetName(GetLayerSide(layerName), "Names");
                text = "{NAME}";
            }
            else if (type == "value") {
                layer = Layer.GetName(GetLayerSide(layerName), "Values");
                text = "{VALUE}";
            }
            else {
                layer = Layer.GetName(GetLayerSide(layerName), GetLayerTag(layerName));
                text = tree.ValueAsString(node[2]);
                if (text.StartsWith('%'))
                    text = String.Format("{{{0}}}", text);
            }

            var element = new TextElement(new LayerSet(layer), position, rotation, height, 
                thickness, horizontalAlign, verticalAlign, text);

            component.AddElement(element);
        }
       
        /// <summary>
        /// Obte un element pad o hole.
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node a procesar.</param>
        /// <param name="component">El component</param>
        /// 
        private void ProcessPadOrHole(STree tree, SBranch node, Component component) {

            string name = tree.ValueAsString(node[1]);
            string padType = tree.ValueAsString(node[2]);
            string shapeType = tree.ValueAsString(node[3]);

            var (position, rotation) = ParseLocation(tree, tree.SelectBranch(node, "at"));
            var size = ParseSize(tree, tree.SelectBranch(node, "size"));
            var drillNode = tree.SelectBranch(node, "drill");
            int drill = drillNode == null ? 0 : (int)(tree.ValueAsDouble(drillNode[1]) * _scale);

            var roundnessNode = tree.SelectBranch(node, "roundrect_rratio");
            Ratio roundness = roundnessNode == null ?
                Ratio.Zero :
                Ratio.FromValue((int)(tree.ValueAsDouble(roundnessNode[1]) * 2000.0));

            Element element;
            switch (padType) {               
                case "smd": {
                    LayerSet layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layers"));
                    element = new SmdPadElement(name, layerSet, position, size, rotation, roundness);
                }
                break;

                case "thru_hole": {
                    LayerSet layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layers"));
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
                    throw new InvalidDataException("Tipo de pad no reconocido.");
            }

            component.AddElement(element);
        }

        /// <summary>
        /// Obte un punt.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>El punt</returns>
        /// 
        private Point ParsePoint(STree tree, SBranch node) {

            var p = new Vector2D(tree.ValueAsDouble(node[1]), tree.ValueAsDouble(node[2])) * _m;
            return new Point((int)p.X, (int)p.Y);
        }

        /// <summary>
        /// Obte un tamany.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>El tamany.</returns>
        /// 
        private static Size ParseSize(STree tree, SBranch node) {

            double sx = tree.ValueAsDouble(node[1]);
            double sy = tree.ValueAsDouble(node[2]);
            return new Size((int)(sx * _scale), (int)(sy * _scale));
        }

        /// <summary>
        /// Obte un angle.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>L'angle.</returns>
        /// 
        private static Angle ParseAngle(STree tree, SBranch node) {

            double a = tree.ValueAsDouble(node[1]);
            return Angle.FromDegrees(a);
        }

        /// <summary>
        /// Obte una mesura.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>El valor.</returns>
        /// 
        private static int ParseMeasure(STree tree, SBranch node) {

            return (int)(tree.ValueAsDouble(node[1]) * _scale);
        }

        /// <summary>
        /// Obte una localitzacio (Posicio i rotacio)
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>La posicio i la rotacio..</returns>
        /// 
        private (Point, Angle) ParseLocation(STree tree, SBranch node) {

            var p = new Vector2D(tree.ValueAsDouble(node[1]), tree.ValueAsDouble(node[2])) * _m;
            double r = node.Count == 4 ? tree.ValueAsDouble(node[3]) : 0;
            var position = new Point((int)p.X, (int)p.Y);
            var rotation = Angle.FromDegrees(r);

            return (position, rotation);
        }

        /// <summary>
        /// Obte un conjunt de capes.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>El resultat.</returns>
        /// 
        private static LayerSet ParseLayerSet(STree tree, SBranch node) {

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

        /// <summary>
        /// Obte el nom de fitxer del modul.
        /// </summary>
        /// <param name="name">El nom del modul.</param>
        /// <returns>El nom de fitxer.</returns>
        /// 
        private static string GetModuleFileName(string name) {

            string[] s = name.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string libFolder = String.Format("{0}.pretty", s[0]);
            string modFileName = String.Format("{0}.kicad_mod", s[1]);
            string[] libBaseFolders = {
                ".",
                @"C:\Program Files\KiCad\share\kicad\modules",
                @"C:\Users\Rafael\Documents\Projectes\KiCad\Libraries\User",
                @"C:\Users\Rafael\Documents\Projectes\KiCad\Aplex",
            };

            foreach (var libBaseFolder in libBaseFolders) {
                string fileName = Path.Combine(libBaseFolder, libFolder, modFileName);
                if (File.Exists(fileName))
                    return fileName;
            }

            return name;
        }
    }
}
