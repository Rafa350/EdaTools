using System;
using System.Collections.Generic;
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

    public sealed class KiCadImporter: IEdaImporter {

        private const double _scale = 1000000.0;

        private EdaPoint _origin;
        private int _partCount = 0;
        private int _version = 5;
        private readonly Dictionary<int, EdaSignal> _signals = new Dictionary<int, EdaSignal>();

        /// <inheritdoc/>
        /// 
        public EdaBoard ReadBoard(string fileName) {

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
        public EdaLibrary ReadLibrary(string fileName) {

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
        private void LoadComponent(string componentName, EdaBoard board) {

            var fileName = GetModuleFileName(componentName);
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {

                var reader = new StreamReader(stream);
                var source = reader.ReadToEnd();
                reader.Close();

                var parser = new SParser();
                var tree = parser.Parse(source);

                var libraryName = componentName.Split(':')[0];
                var library = new EdaLibrary(libraryName);
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
        private EdaBoard ProcessBoard(STree tree) {

            var board = new EdaBoard();

            // Afegeix les capes basiques que no existeixen en la placa a importar
            //
            board.AddLayer(new EdaLayer(EdaLayerId.Pads, BoardSide.None, LayerFunction.Unknown));
            board.AddLayer(new EdaLayer(EdaLayerId.Vias, BoardSide.None, LayerFunction.Unknown));
            board.AddLayer(new EdaLayer(EdaLayerId.Drills, BoardSide.None, LayerFunction.Unknown));
            board.AddLayer(new EdaLayer(EdaLayerId.Platted, BoardSide.None, LayerFunction.Mechanical));
            board.AddLayer(new EdaLayer(EdaLayerId.Unplatted, BoardSide.None, LayerFunction.Mechanical));
            board.AddLayer(new EdaLayer(EdaLayerId.TopNames, BoardSide.Top, LayerFunction.Unknown));
            board.AddLayer(new EdaLayer(EdaLayerId.BottomNames, BoardSide.Bottom, LayerFunction.Unknown));
            board.AddLayer(new EdaLayer(EdaLayerId.TopValues, BoardSide.Top, LayerFunction.Unknown));
            board.AddLayer(new EdaLayer(EdaLayerId.BottomValues, BoardSide.Bottom, LayerFunction.Unknown));
            board.AddLayer(new EdaLayer(EdaLayerId.TopRestrict, BoardSide.Top, LayerFunction.Unknown));
            board.AddLayer(new EdaLayer(EdaLayerId.InnerRestrict, BoardSide.Inner, LayerFunction.Unknown));
            board.AddLayer(new EdaLayer(EdaLayerId.BottomRestrict, BoardSide.Bottom, LayerFunction.Unknown));
            board.AddLayer(new EdaLayer(EdaLayerId.TopKeepout, BoardSide.Top, LayerFunction.Unknown));
            board.AddLayer(new EdaLayer(EdaLayerId.BottomKeepout, BoardSide.Bottom, LayerFunction.Unknown));

            // Obte el numero de versio
            //
            var versionNode = tree.SelectBranch(tree.Root, "version");
            var versionCode = tree.ValueAsString(versionNode[1]);
            if (versionCode.StartsWith("2021"))
                _version = 6;

            // Obte el parametres de configuracio
            //
            var setupNode = tree.SelectBranch(tree.Root, "setup");
            if (setupNode != null) {
                var auxAxisOriginNode = tree.SelectBranch(setupNode, "aux_axis_origin");
                if (auxAxisOriginNode != null) {
                    _origin = ParsePoint(tree, auxAxisOriginNode);
                    board.Position = _origin;
                }
            }

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
            foreach (var moduleNode in tree.SelectBranches(tree.Root, _version == 5 ? "module" : "footprint")) {
                string componentName = tree.ValueAsString(moduleNode[1]);
                if (board.GetComponent(componentName, false) == null)
                    LoadComponent(componentName, board);
            }

            // Procesa el contingut
            //
            foreach (var childNode in (tree.Root as SBranch).Nodes.OfType<SBranch>()) {
                switch (tree.GetBranchName(childNode)) {
                    case "footprint":
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

                    case "gr_arc":
                        ProcessArc(tree, childNode, board);
                        break;

                    case "zone":
                        ProcessZone(tree, childNode, board);
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
        private EdaLibrary ProcessLibrary(STree tree, string libraryName) {

            var library = new EdaLibrary(libraryName);

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
        private static void ProcessLayer(STree tree, SBranch node, EdaBoard board) {

            string name = tree.ValueAsString(node[1]);
            string type = tree.ValueAsString(node[2]);

            BoardSide side = GetLayerSide(name);
            EdaLayerId id = GetLayerId(name);
            LayerFunction function = type == "signal" ? LayerFunction.Signal : GetLayerFunction(name);

            if (board.GetLayer(id, false) == null) {
                var layer = new EdaLayer(id, side, function);
                board.AddLayer(layer);
            }
        }

        /// <summary>
        /// Procesa una senyal.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void ProcessSignal(STree tree, SBranch node, EdaBoard board) {

            int id = tree.ValueAsInteger(node[1]);
            string name = tree.ValueAsString(node[2]);

            if (String.IsNullOrEmpty(name))
                name = "unnamed_signal";

            var signal = new EdaSignal {
                Name = name,
                Clearance = 150000
            };
            board.AddSignal(signal);

            _signals.Add(id, signal);
        }

        /// <summary>
        /// Procesa una via
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node</param>
        /// <returns>L'element creat.</returns>
        /// 
        private void ProcessVia(STree tree, SBranch node, EdaBoard board) {

            var position = ParsePoint(tree, tree.SelectBranch(node, "at")) - _origin;
            var size = ParseMeasure(tree, tree.SelectBranch(node, "size"));
            var drill = ParseMeasure(tree, tree.SelectBranch(node, "drill"));
            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layers"));

            var via = new EdaViaElement {
                LayerSet = layerSet,
                Position = position,
                OuterSize = size,
                InnerSize = size,
                DrillDiameter = drill
            };

            var netNode = tree.SelectBranch(node, "net");
            if (netNode != null) {
                int netId = tree.ValueAsInteger(netNode[1]);
                board.Connect(_signals[netId], via);
            }

            board.AddElement(via);
        }

        /// <summary>
        /// Procesa un modul en una llibreria.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="library">La llibreria.</param>
        /// 
        private void ProcessModule(STree tree, SBranch node, EdaLibrary library) {

            string name = tree.ValueAsString(node[1]);

            var component = new EdaComponent();
            component.Name = String.Format("{0}:{1}", library.Name, name);

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
        /// Procesa un modul en una placa.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node a procesar.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void ProcessPart(STree tree, SBranch node, EdaBoard board) {

            string name = tree.ValueAsString(node[1]);

            var (position, rotation) = ParsePointAndAngle(tree, tree.SelectBranch(node, "at"));
            position -= _origin;

            var layerNode = tree.SelectBranch(node, "layer");
            var layer = tree.ValueAsString(layerNode[1]);
            var side = layer == "Bottom" ? PartSide.Bottom : PartSide.Top;

            var component = board.GetComponent(name);
            var partName = String.Format("{0}:{1}", name, _partCount++);

            // Procesa els atributs
            //
            List<EdaPartAttribute> attributes = null;
            foreach (var childNode in node.Nodes.OfType<SBranch>()) {

                EdaPartAttribute attribute = null;
                switch (tree.GetBranchName(childNode)) {
                    case "fp_text": {
                            string attrValue = tree.ValueAsString(childNode[2]);
                            bool attrVisible = childNode.Count == 6; // Si no te el node 'hide' es visible
                            var (attrPosition, attrRotation) = ParsePointAndAngle(tree, tree.SelectBranch(childNode, "at"));
                            attrPosition -= _origin;
                            attrRotation -= rotation;

                            switch (tree.ValueAsString(childNode[1])) {

                                case "value":
                                    attribute = new EdaPartAttribute("VALUE", attrValue, attrVisible);
                                    break;

                                case "reference":
                                    attribute = new EdaPartAttribute("NAME", "{%name}", attrVisible);
                                    partName = attrValue;
                                    break;

                                case "user":
                                    break;
                            }

                            if (attribute != null) {
                                attribute.Position = attrPosition;
                                attribute.Rotation = attrRotation;
                            }
                        }
                        break;

                    case "property": {
                            string attrName = tree.ValueAsString(childNode[1]);
                            string attrValue = tree.ValueAsString(childNode[2]);
                            attribute = new EdaPartAttribute(attrName, attrValue);
                        }
                        break;
                }

                if (attribute != null) {
                    if (attributes == null)
                        attributes = new List<EdaPartAttribute>();
                    attributes.Add(attribute);
                }
            }

            var part = new EdaPart {
                Component = component,
                Name = partName,
                Position = position,
                Rotation = rotation,
                Side = side
            };
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
        /// Procesa una node 'fp_line'
        /// </summary>
        /// <param name="tree">STree.</param>
        /// <param name="node">El node</param>
        /// <param name="board">La placa.</param>
        /// 
        private void ProcessLine(STree tree, SBranch node, EdaBoard board) {

            var start = ParsePoint(tree, tree.SelectBranch(node, "start")) - _origin;
            var end = ParsePoint(tree, tree.SelectBranch(node, "end")) - _origin;
            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layer"));
            var thickness = ParseMeasure(tree, tree.SelectBranch(node, "width"));

            var element = new EdaLineElement {
                LayerSet = layerSet,
                StartPosition = start,
                EndPosition = end,
                Thickness = thickness,
                LineCap = EdaLineElement.CapStyle.Round
            };

            board.AddElement(element);

            var netNode = tree.SelectBranch(node, "net");
            if (netNode != null) {
                var netId = tree.ValueAsInteger(netNode[1]);
                board.Connect(_signals[netId], element);
            }
        }

        /// <summary>
        /// Procesa una node 'fp_line'.
        /// </summary>
        /// <param name="tree">STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// 
        private void ProcessLine(STree tree, SBranch node, EdaComponent component) {

            var start = ParsePoint(tree, tree.SelectBranch(node, "start"));
            var end = ParsePoint(tree, tree.SelectBranch(node, "end"));
            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layer"));
            var thickness = ParseMeasure(tree, tree.SelectBranch(node, "width"));

            var element = new EdaLineElement {
                LayerSet = layerSet,
                StartPosition = start,
                EndPosition = end,
                Thickness = thickness,
                LineCap = EdaLineElement.CapStyle.Round
            };

            component.AddElement(element);
        }

        /// <summary>
        /// Procesa un node 'gr_arc'.
        /// </summary>
        /// <param name="tree">STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void ProcessArc(STree tree, SBranch node, EdaBoard board) {

            var start = ParsePoint(tree, tree.SelectBranch(node, "start")) - _origin;
            var mid = ParsePoint(tree, tree.SelectBranch(node, "mid")) - _origin;
            var end = ParsePoint(tree, tree.SelectBranch(node, "end")) - _origin;
            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layer"));
            var thickness = ParseMeasure(tree, tree.SelectBranch(node, "width"));

            EdaPoint center = ArcUtils.Center(start, end, mid);
            var startAngle = ArcUtils.StartAngle(start, center).AsDegrees;
            var endAngle = ArcUtils.EndAngle(end, center).AsDegrees;
            var angle = startAngle - endAngle;
            while (angle < 0)
                angle += 360;
            while (angle >= 360)
                angle -= 360;

            var element = new EdaArcElement {
                LayerSet = layerSet,
                StartPosition = angle > 0 ? end : start,
                EndPosition = angle > 0 ? start : end,
                Thickness = thickness,
                Angle = EdaAngle.FromDegrees(Math.Abs(angle)),
                LineCap = EdaLineElement.CapStyle.Flat
            };

            board.AddElement(element);

            var netNode = tree.SelectBranch(node, "net");
            if (netNode != null) {
                var netId = tree.ValueAsInteger(netNode[1]);
                board.Connect(_signals[netId], element);
            }
        }

        /// <summary>
        /// Procesa un node 'fp_arc'.
        /// </summary>
        /// <param name="tree">STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// 
        private void ProcessArc(STree tree, SBranch node, EdaComponent component) {

            var center = ParsePoint(tree, tree.SelectBranch(node, "start"));
            var start = ParsePoint(tree, tree.SelectBranch(node, "end"));
            var angle = ParseAngle(tree, tree.SelectBranch(node, "angle"));
            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layer"));
            var thickness = ParseMeasure(tree, tree.SelectBranch(node, "width"));

            var element = new EdaArcElement {
                LayerSet = layerSet,
                StartPosition = start,
                EndPosition = ArcUtils.EndPosition(center, start, angle),
                Thickness = thickness,
                Angle = angle,
                LineCap = EdaLineElement.CapStyle.Flat
            };

            component.AddElement(element);
        }

        /// <summary>
        /// Procesa un node 'fp_circle'.
        /// </summary>
        /// <param name="tree">STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// 
        private void ProcessCircle(STree tree, SBranch node, EdaComponent component) {

            var center = ParsePoint(tree, tree.SelectBranch(node, "center"));
            var end = ParsePoint(tree, tree.SelectBranch(node, "end"));
            int radius = (int)(Math.Sqrt(Math.Pow(end.X - center.X, 2) + Math.Pow(end.Y - center.Y, 2)));

            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layer"));
            int thickness = ParseMeasure(tree, tree.SelectBranch(node, "width"));

            // Obte el indicador de relleno
            //
            var fillNode = tree.SelectBranch(node, "fill");
            bool filled = (fillNode != null) && (tree.ValueAsString(fillNode[1]) == "solid");

            var element = new EdaCircleElement {
                LayerSet = layerSet,
                Position = center,
                Radius = radius,
                Thickness = thickness,
                Filled = filled
            };
            component.AddElement(element);
        }

        /// <summary>
        /// Procesa un node 'fp_text'.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// 
        private void ProcessText(STree tree, SBranch node, EdaComponent component) {

            string type = tree.ValueAsString(node[1]);
            var (position, rotation) = ParsePointAndAngle(tree, tree.SelectBranch(node, "at"));

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

            EdaLayerId layerId;
            string text;

            if (type == "reference") {
                layerId = layerName.Contains("F.") ? EdaLayerId.TopNames : EdaLayerId.BottomNames;
                text = "{NAME}";
            }
            else if (type == "value") {
                layerId = layerName.Contains("F.") ? EdaLayerId.TopValues : EdaLayerId.BottomValues;
                text = "{VALUE}";
            }
            else {
                layerId = GetLayerId(layerName);
                text = tree.ValueAsString(node[2]);
                if (text.StartsWith('%'))
                    text = String.Format("{{{0}}}", text);
            }
            var layerSet = new EdaLayerSet(layerId);

            var element = new EdaTextElement {
                LayerSet = layerSet,
                Position = position,
                Rotation = rotation,
                Height = height,
                Thickness = thickness,
                HorizontalAlign = horizontalAlign,
                VerticalAlign = verticalAlign,
                Value = text
            };
            component.AddElement(element);
        }

        /// <summary>
        /// Procesa un node 'pad'.
        /// </summary>
        /// <param name="tree">El STree</param>
        /// <param name="node">El node a procesar.</param>
        /// <param name="component">El component.</param>
        /// 
        private void ProcessPadOrHole(STree tree, SBranch node, EdaComponent component) {

            var name = tree.ValueAsString(node[1]);
            var padType = tree.ValueAsString(node[2]);
            var shapeType = tree.ValueAsString(node[3]);

            var layerSet = ParseLayerSet(tree, tree.SelectBranch(node, "layers"));
            var (position, rotation) = ParsePointAndAngle(tree, tree.SelectBranch(node, "at"));
            var size = ParseSize(tree, tree.SelectBranch(node, "size"));
            var drillNode = tree.SelectBranch(node, "drill");
            var drill = drillNode == null ? 0 : (int)(tree.ValueAsDouble(drillNode[1]) * _scale);

            var roundnessNode = tree.SelectBranch(node, "roundrect_rratio");
            var roundness = roundnessNode == null ?
                EdaRatio.Zero :
                EdaRatio.FromPercent(tree.ValueAsDouble(roundnessNode[1]) * 2.0);

            switch (padType) {
                case "smd": {
                        var element = new EdaSmdPadElement {
                            Name = name,
                            LayerSet = layerSet,
                            Position = position,
                            Size = size,
                            Rotation = rotation,
                            CornerRatio = roundness
                        };
                        component.AddElement(element);
                    }
                    break;

                case "thru_hole": {
                        switch (shapeType) {
                            case "rect":
                                roundness = EdaRatio.Zero;
                                break;

                            case "circle":
                            case "oval":
                                roundness = EdaRatio.P100;
                                break;

                            case "roundrect":
                                break;

                            default:
                                throw new InvalidOperationException("Tipo de forma desconocida.");
                        }

                        layerSet.Add(EdaLayerId.Drills);
                        var element = new EdaThPadElement {
                            Name = name,
                            LayerSet = layerSet,
                            Position = position,
                            TopSize = size,
                            InnerSize = size,
                            BottomSize = size,
                            Rotation = rotation,
                            CornerRatio = roundness,
                            DrillDiameter = drill
                        };
                        component.AddElement(element);
                    }
                    break;

                case "np_thru_hole": {
                        var element = new EdaCircularHoleElement {
                            Position = position,
                            Diameter = drill,
                            Platted = false
                        };
                        component.AddElement(element);
                        break;
                    }

                default:
                    throw new InvalidDataException("Tipo de pad no reconocido.");
            }
        }

        /// <summary>
        /// Procesa un node 'zone'
        /// </summary>
        /// <param name="tree">STree.</param>
        /// <param name="node">El node.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void ProcessZone(STree tree, SBranch node, EdaBoard board) {

            var layerNode = tree.SelectBranch(node, "layer");
            if (layerNode == null)
                layerNode = tree.SelectBranch(node, "layers");
            var layerSet = ParseLayerSet(tree, layerNode);
            var priorityNode = tree.SelectBranch(node, "priority");
            var priority = priorityNode == null ? 0 : tree.ValueAsInteger(priorityNode[1]);
            var connectedPadNode = tree.SelectBranch(node, "connect_pads");
            var clearanceNode = tree.SelectBranch(connectedPadNode, "clearance");
            int clearance = ParseMeasure(tree, clearanceNode);

            var fillSettingsNode = tree.SelectBranch(node, "fill");
            var thermalClearanceNode = tree.SelectBranch(fillSettingsNode, "thermal_gap");
            var thermalClearance = ParseMeasure(tree, thermalClearanceNode);
            var thermalThicknessNode = tree.SelectBranch(fillSettingsNode, "thermal_bridge_width");
            var thermalThickness = ParseMeasure(tree, thermalThicknessNode);
            var radiusNode = tree.SelectBranch(fillSettingsNode, "radius");
            var radius = ParseMeasure(tree, radiusNode);

            var segments = new List<EdaArcPoint>();
            var polygonNode = tree.SelectBranch(node, "polygon");
            if (polygonNode != null) {
                var ptsNode = tree.SelectBranch(polygonNode, "pts");
                if (ptsNode != null) {
                    foreach (var xyNode in ptsNode.Nodes.OfType<SBranch>()) {
                        var point = ParsePoint(tree, xyNode) - _origin;
                        segments.Add(new EdaArcPoint(point));
                    }
                }
            }

            var element = new EdaRegionElement {
                LayerSet = layerSet,
                Clearance = clearance,
                Thickness = radius * 2,
                ThermalClearance = thermalClearance,
                ThermalThickness = thermalThickness,
                Filled = true,
                Priority = priority,
                Segments = segments
            };
            board.AddElement(element);

            var netNode = tree.SelectBranch(node, "net");
            if (netNode != null) {
                int netId = tree.ValueAsInteger(netNode[1]);
                board.Connect(_signals[netId], element);
            }
        }


        /// <summary>
        /// Obte un punt.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>El punt</returns>
        /// 
        private static EdaPoint ParsePoint(STree tree, SBranch node) {

            int x = (int)(tree.ValueAsDouble(node[1]) * _scale);
            int y = (int)(tree.ValueAsDouble(node[2]) * _scale * -1);
            return new EdaPoint(x, y);
        }

        /// <summary>
        /// Obte un punt i un angle (Posicio i rotacio)
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>El punt i l'angle.</returns>
        /// 
        private static (EdaPoint, EdaAngle) ParsePointAndAngle(STree tree, SBranch node) {

            int x = (int)(tree.ValueAsDouble(node[1]) * _scale);
            int y = (int)(tree.ValueAsDouble(node[2]) * _scale * -1);
            var position = new EdaPoint(x, y);

            double r = node.Count == 4 ? tree.ValueAsDouble(node[3]) : 0;
            var rotation = EdaAngle.FromDegrees(r);

            return (position, rotation);
        }

        /// <summary>
        /// Obte un tamany.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>El tamany.</returns>
        /// 
        private static EdaSize ParseSize(STree tree, SBranch node) {

            int sx = (int)(tree.ValueAsDouble(node[1]) * _scale);
            int sy = (int)(tree.ValueAsDouble(node[2]) * _scale);
            return new EdaSize(sx, sy);
        }

        /// <summary>
        /// Obte un angle.
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>L'angle.</returns>
        /// 
        private static EdaAngle ParseAngle(STree tree, SBranch node) {

            double a = tree.ValueAsDouble(node[1]);
            return EdaAngle.FromDegrees(a);
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
        /// Obte el conjunt de capes
        /// </summary>
        /// <param name="tree">El STree.</param>
        /// <param name="node">El node.</param>
        /// <returns>El conjunt obtingut.</returns>
        /// 
        private static EdaLayerSet ParseLayerSet(STree tree, SBranch node) {

            var layers = new EdaLayerSet();

            for (int i = 1; i < node.Count; i++) {
                string kcName = tree.ValueAsString(node[i]);
                if (kcName.StartsWith("*.")) {
                    layers.Add(GetLayerId(kcName.Replace("*.", "F.")));
                    layers.Add(GetLayerId(kcName.Replace("*.", "B.")));
                }
                else
                    layers.Add(GetLayerId(kcName));
            }

            return layers;
        }

        /// <summary>
        /// Obte la cara de la placa a partir d'un nom de capa.
        /// </summary>
        /// <param name="kcName">Nom de la capa.</param>
        /// <returns>La cara.</returns>
        /// 
        private static BoardSide GetLayerSide(string kcName) {

            if (kcName.Contains("Top") || kcName.Contains("F."))
                return BoardSide.Top;
            else if (kcName.Contains("Bottom") || kcName.Contains("B."))
                return BoardSide.Bottom;
            else if (kcName.Contains("In"))
                return BoardSide.Inner;
            else
                return BoardSide.None;
        }

        /// <summary>
        /// Obte el identificador de la capa a partir del seu nom.
        /// </summary>
        /// <param name="kcName">Nom de la capa.</param>
        /// <returns>El identificador.</returns>
        /// 
        private static EdaLayerId GetLayerId(string kcName) {

            switch (kcName) {
                case "Top":
                case "F.Cu":
                    return EdaLayerId.TopCopper;

                case "Bottom":
                case "B.Cu":
                    return EdaLayerId.BottomCopper;

                case "In1.Cu":
                    return EdaLayerId.InnerCopper1;

                case "In2.Cu":
                    return EdaLayerId.InnerCopper2;

                case "F.Paste":
                    return EdaLayerId.TopCream;

                case "B.Paste":
                    return EdaLayerId.BottomCream;

                case "F.Adhes":
                    return EdaLayerId.TopGlue;

                case "B.Adhes":
                    return EdaLayerId.BottomGlue;

                case "F.Mask":
                    return EdaLayerId.TopStop;

                case "B.Mask":
                    return EdaLayerId.BottomStop;

                case "F.SilkS":
                    return EdaLayerId.TopPlace;

                case "B.SilkS":
                    return EdaLayerId.BottomPlace;

                case "F.CrtYd":
                    return EdaLayerId.TopKeepout;

                case "B.CrtYd":
                    return EdaLayerId.BottomKeepout;

                case "Edge.Cuts":
                    return EdaLayerId.Profile;

                default:
                    return EdaLayerId.Get(kcName);
            }
        }

        /// <summary>
        /// Obte la funcio de la capa a partir del seu nom.
        /// </summary>
        /// <param name="kcName">El nom.</param>
        /// <returns>La funcio.</returns>
        /// 
        private static LayerFunction GetLayerFunction(string kcName) {

            if (kcName.EndsWith(".Cu"))
                return LayerFunction.Signal;

            else if (kcName == "Edge.Cuts")
                return LayerFunction.Outline;

            else
                return LayerFunction.Unknown;
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
                @"C:\Program Files\KiCad\6.0\share\kicad\footprints",
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
