﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Geometry.Utils;
using MikroPic.EdaTools.v1.Core.Import.KiCad.Infrastructure;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Net;

namespace MikroPic.EdaTools.v1.Core.Import.KiCad {

    public sealed class KiCadImporter: IEdaImporter {

        private enum StrokeType {
            Solid
        }

        private struct Stroke {
            public int Width;
            public StrokeType Type;
        }

        private struct Location {
            public EdaPoint Position;
            public EdaAngle Rotation;
            public bool Locked;
        }

        private const double _scale = 1000000.0;

        private int _version = 5;
        private int _partCount;
        private EdaPoint _origin;
        private STree _tree;

        private readonly Dictionary<int, EdaSignal> _signalLocator = new Dictionary<int, EdaSignal>();

        public EdaBoard ReadBoard(string fileName) {

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {

                var reader = new StreamReader(stream);
                var source = reader.ReadToEnd();

                var parser = new SParser();
                _tree = parser.Parse(source);

                return BuildBoard(_tree.Root as SBranch);
            }
        }

        public EdaLibrary ReadLibrary(string fileName) {

            return null;
        }

        public Net ReadNet(string fileName) {

            return null;
        }

        /// <summary>
        /// Construeix la placa.
        /// </summary>
        /// <param name="tree">L'arbre.</param>
        /// <param name="node">El node.</param>
        /// <returns>La placa.</returns>
        /// 
        private EdaBoard BuildBoard(SBranch node) {

            var board = new EdaBoard();

            // Obte el numero de versio
            //
            var versionNode = _tree.SelectBranch(node, "version");
            string versionCode = _tree.ValueAsString(versionNode[1]);
            if (versionCode.StartsWith("2021"))
                _version = 6;
            else if (versionCode.StartsWith("2022"))
                _version = 7;

            // Obte el parametres de configuracio
            //
            var setupNode = _tree.SelectBranch(node, "setup");
            if (setupNode != null) {
                var auxAxisOriginNode = _tree.SelectBranch(setupNode, "aux_axis_origin");
                if (auxAxisOriginNode != null) {
                    _origin = ParsePoint(auxAxisOriginNode);
                    board.Position = _origin;
                }
            }

            BuildLayers(node, board);
            BuildNets(node, board);
            BuildParts(node, board);
            BuildElements(node, board);

            return board;
        }

        /// <summary>
        /// Construeix les capes.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void BuildLayers(SBranch node, EdaBoard board) {

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

            var layersNode = _tree.SelectBranch(node, "layers");
            foreach (var layerNode in layersNode.Nodes.OfType<SBranch>()) {
                var layer = CreateLayer(layerNode);
                board.AddLayer(layer);
            }
        }

        /// <summary>
        /// Construeix els senyals.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="board">La placas.</param>
        /// 
        private void BuildNets(SBranch node, EdaBoard board) {

            foreach (var netNode in _tree.SelectBranches(node, "net")) {
                var signal = CreateSignal(netNode);
                board.AddSignal(signal);
            }
        }

        /// <summary>
        /// Construeix els parts.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void BuildParts(SBranch node, EdaBoard board) {

            foreach (var footprintNode in _tree.SelectBranches(node, _version > 5 ? "footprint" : "module")) {

                var component = CreateComponent(footprintNode);
                board.AddComponent(component);

                var part = CreatePart(footprintNode, board);
                board.AddPart(part);
            }
        }

        /// <summary>
        /// Construeix els elements.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="board">La placa.</param>
        /// 
        private void BuildElements(SBranch node, EdaBoard board) {

            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                switch (_tree.GetBranchName(childNode)) {
                    case "via":
                        ProcessVia(childNode, board);
                        break;

                    case "segment":
                        ProcessSegment(childNode, board);
                        break;

                    case "zone":
                        ProcessZone(childNode, board);
                        break;

                    case "gr_line":
                        ProcessGrLine(childNode, board);
                        break;

                    case "gr_arc":
                        ProcessGrArc(childNode, board);
                        break;
                }
            }
        }

        /// <summary>
        /// Crea una capa.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>La capa creada.</returns>
        /// 
        private EdaLayer CreateLayer(SBranch node) {

            string name = _tree.ValueAsString(node[1]);
            string type = _tree.ValueAsString(node[2]);

            // Obte el identificador de la capa.
            //
            EdaLayerId id = GetLayerId(name);

            // Obte la cara de la capa.
            //
            BoardSide side = BoardSide.None;
            if (name.StartsWith("F."))
                side = BoardSide.Top;
            else if (name.StartsWith("B."))
                side = BoardSide.Bottom;
            else if (name.StartsWith("In") && name.EndsWith(".Cu"))
                side = BoardSide.Inner;

            // Obte la funcio de la capa.
            //
            LayerFunction function = LayerFunction.Unknown;
            if ((type == "signal") || (type == "mixed") || (type == "power") || (type == "jumper") || name.EndsWith(".Cu"))
                function = LayerFunction.Signal;
            else if (type == "user") {
                if (name.EndsWith(".SilkS") || name.EndsWith(".CrtYr"))
                    function = LayerFunction.Document;
                else if (name.EndsWith(".Paste") || name.EndsWith(".Mask") || name.EndsWith(".Adhes"))
                    function = LayerFunction.Design;
                else if (name == "Edge.Cuts")
                    function = LayerFunction.Outline;
            }

            return new EdaLayer(id, side, function);
        }

        /// <summary>
        /// Crea una senyal
        /// </summary>
        /// <param name="node">El node.</param>
        /// 
        private EdaSignal CreateSignal(SBranch node) {

            var id = _tree.ValueAsInteger(node[1]);
            var name = _tree.ValueAsString(node[2]);

            if (String.IsNullOrEmpty(name))
                name = "unnamed_signal";

            var signal = new EdaSignal {
                Name = name,
                Clearance = 150000
            };
            _signalLocator.Add(id, signal);

            return signal;
        }

        /// <summary>
        /// Crea un component.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>El component creat.</returns>
        /// 
        private EdaComponent CreateComponent(SBranch node) {

            var name = _tree.ValueAsString(node[1]);
            var tsNode = _tree.SelectBranch(node, "tstamp");
            var ts = _tree.ValueAsString(tsNode[1]);
            var componentName = string.Format("{0}:{1}", name, ts);

            var location = ParseLocation(_tree.SelectBranch(node, "at"));

            var solderMaskMarginNode = _tree.SelectBranch(node, "solder_mask_margin");
            int solderMaskMargin = 0;
            if (solderMaskMarginNode != null)
                solderMaskMargin = ParseScalar(solderMaskMarginNode);

            var solderPasteRatioNode = _tree.SelectBranch(node, "solder_paste_ratio");
            EdaRatio solderPasteRatio = EdaRatio.Zero;
            if (solderPasteRatioNode != null)
                solderPasteRatio = ParseRatio(solderPasteRatioNode);

            var component = new EdaComponent();
            component.Name = componentName;

            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                switch (_tree.GetBranchName(childNode)) {
                    case "fp_text":
                        ProcessFpText(childNode, location.Rotation, component);
                        break;

                    case "fp_circle":
                        ProcessFpCircle(childNode, component);
                        break;

                    case "fp_rectangle":
                        throw new NotImplementedException();

                    case "fp_line":
                        ProcessFpLine(childNode, component);
                        break;

                    case "fp_arc":
                        ProcessFpArc(childNode, component);
                        break;

                    case "pad":
                        ProcessPad(childNode, component, location.Rotation, solderMaskMargin, solderPasteRatio);
                        break;
                }
            }

            return component;
        }

        /// <summary>
        /// Crea un part.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="board">La placa.</param>
        /// 
        private EdaPart CreatePart(SBranch node, EdaBoard board) {

            string name = _tree.ValueAsString(node[1]);
            var tsNode = _tree.SelectBranch(node, "tstamp");
            var ts = _tree.ValueAsString(tsNode[1]);
            var componentName = string.Format("{0}:{1}", name, ts);

            var location = ParseLocation(_tree.SelectBranch(node, "at"));
            location.Position -= _origin;

            var layerNode = _tree.SelectBranch(node, "layer");
            var layer = _tree.ValueAsString(layerNode[1]);
            var side = layer.Contains("B.") ? PartSide.Bottom : PartSide.Top;

            var component = board.GetComponent(componentName);
            var partName = String.Format("{0}:{1}", name, _partCount++);

            // Procesa els atributs
            //
            List<EdaPartAttribute> attributes = null;
            foreach (var childNode in node.Nodes.OfType<SBranch>()) {

                EdaPartAttribute attribute = null;
                switch (_tree.GetBranchName(childNode)) {
                    case "fp_text": {
                            string attrValue = _tree.ValueAsString(childNode[2]);
                            var attrLocation = ParseLocation(_tree.SelectBranch(childNode, "at"));

                            bool attrVisible = true;
                            for (int i = 3; i < childNode.Count; i++)
                                if ((childNode[i] is SLeaf) && (_tree.ValueAsString(childNode[i]) == "hide")) {
                                    attrVisible = false;
                                    break;
                                }

                            switch (_tree.ValueAsString(childNode[1])) {
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
                                attribute.Position = attrLocation.Position;
                                attribute.Rotation = attrLocation.Rotation - location.Rotation;
                            }
                        }
                        break;

                    case "property": {
                            string attrName = _tree.ValueAsString(childNode[1]);
                            string attrValue = _tree.ValueAsString(childNode[2]);
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
                Position = location.Position,
                Rotation = location.Rotation,
                Side = side
            };
            if (attributes != null)
                part.AddAttributes(attributes);

            // Procesa les senyals
            //
            foreach (var childNode in node.Nodes.OfType<SBranch>()) {
                if (_tree.GetBranchName(childNode) == "pad") {
                    var netNode = _tree.SelectBranch(childNode, "net");
                    if (netNode != null) {

                        var padName = _tree.ValueAsString(childNode[1]);
                        var netId = _tree.ValueAsInteger(netNode[1]);

                        var pad = part.GetPad(padName);
                        var signal = _signalLocator[netId];
                        try {
                            board.Connect(signal, pad, part);
                        }
                        catch {

                        }
                    }
                }
            }

            return part;
        }

        /// <summary>
        /// Procesa un 'fp_line'.
        /// </summary>
        /// <param name="node">La branca.</param>
        /// <param name="component">El component.</param>
        /// <remarks>Les coordinades son relatives al component.</remarks>
        /// 
        private void ProcessFpLine(SBranch node, EdaComponent component) {

            var start = ParsePoint(_tree.SelectBranch(node, "start"));
            var end = ParsePoint(_tree.SelectBranch(node, "end"));
            var layerSet = ParseLayerSet(_tree.SelectBranch(node, "layer"));
            var stroke = ParseStroke(_tree.SelectBranch(node, "stroke"));

            var element = new EdaLineElement {
                LayerSet = layerSet,
                StartPosition = start,
                EndPosition = end,
                Thickness = stroke.Width,
                LineCap = EdaLineCap.Round
            };

            component.AddElement(element);
        }

        /// <summary>
        /// Procesa un 'fp_arc'.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// <remarks>Les coordinades son relatives al component.</remarks>
        /// 
        private void ProcessFpArc(SBranch node, EdaComponent component) {

            var start = ParsePoint(_tree.SelectBranch(node, "start")) - _origin;
            var mid = ParsePoint(_tree.SelectBranch(node, "mid")) - _origin;
            var end = ParsePoint(_tree.SelectBranch(node, "end")) - _origin;
            var layerSet = ParseLayerSet(_tree.SelectBranch(node, "layer"));
            var stroke = ParseStroke(_tree.SelectBranch(node, "stroke"));

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
                Thickness = stroke.Width,
                Angle = EdaAngle.FromDegrees(Math.Abs(angle)),
                LineCap = EdaLineCap.Flat
            };

            component.AddElement(element);
        }

        /// <summary>
        /// Procesa un 'fp_circle'.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// <remarks>Les coordinades son relatives al component.</remarks>
        /// 
        private void ProcessFpCircle(SBranch node, EdaComponent component) {

            var center = ParsePoint(_tree.SelectBranch(node, "center"));
            var end = ParsePoint(_tree.SelectBranch(node, "end"));
            int radius = (int)(Math.Sqrt(Math.Pow(end.X - center.X, 2) + Math.Pow(end.Y - center.Y, 2)));
            var layerSet = ParseLayerSet(_tree.SelectBranch(node, "layer"));
            var stroke = ParseStroke(_tree.SelectBranch(node, "stroke"));

            var fillBranch = _tree.SelectBranch(node, "fill");
            bool filled = (fillBranch != null) && (_tree.ValueAsString(fillBranch[1]) == "solid");

            int thickness = stroke.Width;
            if (radius <= thickness / 2) {
                radius = thickness / 2;
                thickness = 0;
                filled = true;
            }

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
        /// Procesa un node 'pad'.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="component">El component.</param>
        /// <param name="fpRotation">Rotacio del footprint al que pertany.</param>
        /// <param name="fpMaskClearance">Espaiat de la mascara del footprint.</param>
        /// <param name="fpPasteReductionRatio">Reduccio de pasta del footprint.</param>
        /// <remarks>Les coordinades son relatives al component.</remarks>
        /// 
        private void ProcessPad(SBranch node, EdaComponent component, EdaAngle fpRotation, int fpMaskClearance, EdaRatio fpPasteReductionRatio) {

            var name = _tree.ValueAsString(node[1]);
            var padType = _tree.ValueAsString(node[2]);
            var shapeType = _tree.ValueAsString(node[3]);

            var layerSet = ParseLayerSet(_tree.SelectBranch(node, "layers"));
            var maskEnabled = layerSet.Contains(EdaLayerId.TopStop) || layerSet.Contains(EdaLayerId.BottomStop);
            var pasteEnabled = layerSet.Contains(EdaLayerId.TopCream) || layerSet.Contains(EdaLayerId.BottomCream);
            var location = ParseLocation(_tree.SelectBranch(node, "at"));
            var size = ParseSize(_tree.SelectBranch(node, "size"));

            var drillNode = _tree.SelectBranch(node, "drill");
            var drill = drillNode == null ? 0 : (int)(_tree.ValueAsDouble(drillNode[1]) * _scale);

            var clearanceNode = _tree.SelectBranch(node, "clearance");
            var clearance = clearanceNode == null ? 0 : (int)(_tree.ValueAsDouble(clearanceNode[1]) * _scale);

            var roundnessNode = _tree.SelectBranch(node, "roundrect_rratio");
            var roundness = roundnessNode == null ?
                EdaRatio.Zero :
                EdaRatio.FromPercent(_tree.ValueAsDouble(roundnessNode[1]) * 2.0);

            switch (padType) {
                case "smd": {
                        var element = new EdaSmtPadElement {
                            Name = name,
                            LayerSet = layerSet,
                            Position = location.Position,
                            Size = size,
                            Rotation = location.Rotation - fpRotation,
                            CornerRatio = roundness,
                            Clearance = clearance,
                            MaskEnabled = maskEnabled,
                            MaskClearance = fpMaskClearance,
                            PasteEnabled = pasteEnabled,
                            PasteReductionRatio = fpPasteReductionRatio
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
                        var element = new EdaThtPadElement {
                            Name = name,
                            LayerSet = layerSet,
                            Position = location.Position,
                            TopSize = size,
                            InnerSize = size,
                            BottomSize = size,
                            Rotation = location.Rotation - fpRotation,
                            CornerRatio = roundness,
                            DrillDiameter = drill,
                            Clearance = clearance,
                            MaskEnabled = maskEnabled,
                            MaskClearance = 0
                        };
                        component.AddElement(element);
                    }
                    break;

                case "np_thru_hole": {
                        var element = new EdaCircularHoleElement {
                            Position = location.Position,
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
        /// Procesa un node 'fp_text'.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="fpRotation">Rotacio del footprint.</param>
        /// <param name="component">El component.</param>
        /// <remarks>Les coordinades son relatives al component.</remarks>
        /// 
        private void ProcessFpText(SBranch node, EdaAngle fpRotation, EdaComponent component) {

            string type = _tree.ValueAsString(node[1]);
            var location = ParseLocation(_tree.SelectBranch(node, "at"));

            // Obte els efectes
            //
            HorizontalTextAlign horizontalAlign = HorizontalTextAlign.Center;
            VerticalTextAlign verticalAlign = VerticalTextAlign.Middle;
            int thickness = 125000;
            int height = 1000000;
            var effectsNode = _tree.SelectBranch(node, "effects");
            if (effectsNode != null) {
                var fontNode = _tree.SelectBranch(effectsNode, "font");
                if (fontNode != null) {
                    height = ParseScalar(_tree.SelectBranch(fontNode, "size"));
                    thickness = ParseScalar(_tree.SelectBranch(fontNode, "thickness"));
                }
                var justifyNode = _tree.SelectBranch(effectsNode, "justify");
                if (justifyNode != null) {
                    foreach (var justifyChild in justifyNode.Skip(1)) {
                        var h = _tree.ValueAsString(justifyChild);
                        switch (h) {
                            case "left":
                                horizontalAlign = HorizontalTextAlign.Left;
                                break;
                            case "right":
                                horizontalAlign = HorizontalTextAlign.Right;
                                break;
                            case "top":
                                verticalAlign = VerticalTextAlign.Top;
                                break;
                            case "bottom":
                                verticalAlign = VerticalTextAlign.Bottom;
                                break;
                        }
                    }
                }
            }

            var layerNode = _tree.SelectBranch(node, "layer");
            string layerName = _tree.ValueAsString(layerNode[1]);

            EdaLayerId layerId;
            string text;

            if (type == "reference") {
                layerId = layerName.StartsWith("F.") ? EdaLayerId.TopNames : EdaLayerId.BottomNames;
                text = "{NAME}";
            }
            else if (type == "value") {
                layerId = layerName.StartsWith("F.") ? EdaLayerId.TopValues : EdaLayerId.BottomValues;
                text = "{VALUE}";
            }
            else {
                layerId = GetLayerId(layerName);
                text = _tree.ValueAsString(node[2]);
                if (text.StartsWith('%'))
                    text = String.Format("{{{0}}}", text);
            }
            var layerSet = new EdaLayerSet(layerId);

            var element = new EdaTextElement {
                LayerSet = layerSet,
                Position = location.Position,
                Rotation = location.Rotation - fpRotation,
                Height = height,
                Thickness = thickness,
                HorizontalAlign = horizontalAlign,
                VerticalAlign = verticalAlign,
                Value = text
            };
            component.AddElement(element);
        }

        /// <summary>
        /// Procesa un 'gr_line'
        /// </summary>
        /// <param name="node"></param>
        /// <param name="board"></param>
        /// <remarks>Les coordinades son relatives a la placa.</remarks>
        /// 
        private void ProcessGrLine(SBranch node, EdaBoard board) {

            var start = ParsePosition(_tree.SelectBranch(node, "start")) - _origin;
            var end = ParsePosition(_tree.SelectBranch(node, "end")) - _origin;
            var stroke = ParseStroke(_tree.SelectBranch(node, "stroke"));
            var layerSet = ParseLayerSet(_tree.SelectBranch(node, "layer"));

            var element = new EdaLineElement {
                LayerSet = layerSet,
                StartPosition = start,
                EndPosition = end,
                Thickness = stroke.Width,
                LineCap = EdaLineCap.Flat
            };

            board.AddElement(element);
        }

        /// <summary>
        /// Procesa un 'gr_arc'
        /// </summary>
        /// <param name="node"></param>
        /// <param name="board"></param>
        /// <remarks>Les coordinades son relatives a la placa.</remarks>
        /// 
        private void ProcessGrArc(SBranch node, EdaBoard board) {

            var start = ParsePosition(_tree.SelectBranch(node, "start")) - _origin;
            var mid = ParsePosition(_tree.SelectBranch(node, "mid")) - _origin;
            var end = ParsePosition(_tree.SelectBranch(node, "end")) - _origin;
            var stroke = ParseStroke(_tree.SelectBranch(node, "stroke"));
            var layerSet = ParseLayerSet(_tree.SelectBranch(node, "layer"));

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
                Thickness = stroke.Width,
                Angle = EdaAngle.FromDegrees(Math.Abs(angle)),
                LineCap = EdaLineCap.Flat
            };

            board.AddElement(element);
        }

        /// <summary>
        /// Procesa una via
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="board">La placa.</param>
        /// <remarks>Les coordinades son relatives al la placa.</remarks>
        /// 
        private void ProcessVia(SBranch node, EdaBoard board) {

            var position = ParsePoint(_tree.SelectBranch(node, "at")) - _origin;
            var size = ParseScalar(_tree.SelectBranch(node, "size"));
            var drill = ParseScalar(_tree.SelectBranch(node, "drill"));
            var layerSet = ParseLayerSet(_tree.SelectBranch(node, "layers"));

            var via = new EdaViaElement {
                LayerSet = layerSet,
                Position = position,
                OuterSize = size,
                InnerSize = size,
                DrillDiameter = drill
            };

            var netNode = _tree.SelectBranch(node, "net");
            if (netNode != null) {
                int netId = _tree.ValueAsInteger(netNode[1]);
                board.Connect(_signalLocator[netId], via);
            }

            board.AddElement(via);
        }

        /// <summary>
        /// Procesa un segment.
        /// </summary>
        /// <param name="branch">La branca</param>
        /// <param name="board">La placa.</param>
        /// <remarks>Les coordinades son relatives al la placa.</remarks>
        /// 
        private void ProcessSegment(SBranch branch, EdaBoard board) {

            var start = ParsePoint(_tree.SelectBranch(branch, "start")) - _origin;
            var end = ParsePoint(_tree.SelectBranch(branch, "end")) - _origin;
            var layerSet = ParseLayerSet(_tree.SelectBranch(branch, "layer"));
            var thickness = ParseScalar(_tree.SelectBranch(branch, "width"));

            var element = new EdaLineElement {
                LayerSet = layerSet,
                StartPosition = start,
                EndPosition = end,
                Thickness = thickness,
                LineCap = EdaLineCap.Round
            };

            board.AddElement(element);

            var netNode = _tree.SelectBranch(branch, "net");
            if (netNode != null) {
                var netId = _tree.ValueAsInteger(netNode[1]);
                board.Connect(_signalLocator[netId], element);
            }
        }

        /// <summary>
        /// Procesa un node 'zone'
        /// </summary>
        /// <param name="branch">La branca</param>
        /// <param name="board">La placa.</param>
        /// <remarks>Les coordinades son relatives al la placa.</remarks>
        /// 
        private void ProcessZone(SBranch branch, EdaBoard board) {

            var layerNode = _tree.SelectBranch(branch, "layer");
            if (layerNode == null)
                layerNode = _tree.SelectBranch(branch, "layers");
            var layerSet = ParseLayerSet(layerNode);
            var priorityNode = _tree.SelectBranch(branch, "priority");
            var priority = priorityNode == null ? 0 : _tree.ValueAsInteger(priorityNode[1]);

            var connectedPadNode = _tree.SelectBranch(branch, "connect_pads");
            int clearance = ParseScalar(_tree.SelectBranch(connectedPadNode, "clearance"));

            var fillSettingsNode = _tree.SelectBranch(branch, "fill");
            var thermalClearance = ParseScalar(_tree.SelectBranch(fillSettingsNode, "thermal_gap"));
            var thermalThickness = ParseScalar(_tree.SelectBranch(fillSettingsNode, "thermal_bridge_width"));

            var radiusNode = _tree.SelectBranch(fillSettingsNode, "radius");
            var radius = radiusNode == null ? 0 : ParseScalar(radiusNode);

            var vertices = new List<EdaArcPoint>();
            var polygonNode = _tree.SelectBranch(branch, "polygon");
            if (polygonNode != null) {
                var ptsNode = _tree.SelectBranch(polygonNode, "pts");
                if (ptsNode != null) {
                    foreach (var xyNode in ptsNode.Nodes.OfType<SBranch>()) {
                        var point = ParsePoint(xyNode) - _origin;
                        vertices.Add(new EdaArcPoint(point));
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
                Vertices = vertices
            };
            board.AddElement(element);

            var netNode = _tree.SelectBranch(branch, "net");
            if (netNode != null) {
                int netId = _tree.ValueAsInteger(netNode[1]);
                board.Connect(_signalLocator[netId], element);
            }
        }

        /// <summary>
        /// Obte un punt.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>El punt.</returns>
        /// 
        private EdaPoint ParsePoint(SBranch node) {

            int x = (int)(_tree.ValueAsDouble(node[1]) * _scale);
            int y = (int)(_tree.ValueAsDouble(node[2]) * _scale * -1);
            return new EdaPoint(x, y);
        }

        /// <summary>
        /// Obte els parametres de traçat
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>El resultat.</returns>
        /// 
        private Stroke ParseStroke(SBranch node) {

            var widthNode = _tree.SelectBranch(node, "width");
            var width = _tree.ValueAsDouble(widthNode[1]);

            return new Stroke {
                Width = (int)(width * _scale)
            };
        }

        /// <summary>
        /// Obte els parametres de posicio i angle
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>El resultat.</returns>
        /// 
        private Location ParseLocation(SBranch node) {

            int x = (int)(_tree.ValueAsDouble(node[1]) * _scale);
            int y = (int)(_tree.ValueAsDouble(node[2]) * _scale * -1);           
            double r = 0;
            bool locked = true;

            if (node.Count == 4) {
                if (_tree.ValueAsString(node[3]) == "unlocked")
                    locked = false;
                else
                    r = _tree.ValueAsDouble(node[3]);
            }

            return new Location {
                Position = new EdaPoint(x, y),
                Rotation = EdaAngle.FromDegrees(r),
                Locked = locked
            };
        }

        /// <summary>
        /// Obte els parametres de posicio i angle
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>El resultat.</returns>
        /// 
        private EdaPoint ParsePosition(SBranch node) {

            int x = (int)(_tree.ValueAsDouble(node[1]) * _scale);
            int y = (int)(_tree.ValueAsDouble(node[2]) * _scale * -1);
            return new EdaPoint(x, y);
        }

        /// <summary>
        /// Obte un tamany.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>El tamany.</returns>
        /// 
        private EdaSize ParseSize(SBranch node) {

            int sx = (int)(_tree.ValueAsDouble(node[1]) * _scale);
            int sy = (int)(_tree.ValueAsDouble(node[2]) * _scale);
            return new EdaSize(sx, sy);
        }

        /// <summary>
        /// Obte un angle.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>L'angle.</returns>
        /// 
        private EdaAngle ParseAngle(SBranch node) {

            double a = _tree.ValueAsDouble(node[1]);
            return EdaAngle.FromDegrees(a);
        }

        /// <summary>
        /// Obte un percentatge.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>El percentatge</returns>
        /// 
        private EdaRatio ParseRatio(SBranch node) {

            double r = _tree.ValueAsDouble(node[1]);
            return EdaRatio.FromPercent(Math.Abs(r));
        }

        /// <summary>
        /// Obte un nombre escalar.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>El valor.</returns>
        /// 
        private int ParseScalar(SBranch node) {

            return (int)(_tree.ValueAsDouble(node[1]) * _scale);
        }

        /// <summary>
        /// Obte el conjunt de capes
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>El conjunt obtingut.</returns>
        /// 
        private EdaLayerSet ParseLayerSet(SBranch node) {

            var layers = new EdaLayerSet();

            for (int i = 1; i < node.Count; i++) {
                string kcName = _tree.ValueAsString(node[i]);
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
        /// Obte el identificador de la capa a partir del seu nom canonic.
        /// </summary>
        /// <param name="canonicName">Nom de la capa.</param>
        /// <returns>El identificador.</returns>
        /// 
        private static EdaLayerId GetLayerId(string canonicName) {

            switch (canonicName) {
                case "F.Cu":
                    return EdaLayerId.TopCopper;

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
                    return EdaLayerId.Get(canonicName);
            }
        }
    }
}
