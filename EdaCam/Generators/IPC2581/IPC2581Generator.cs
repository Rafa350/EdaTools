﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public sealed class IPC2581Generator: Generator {

        private const double _scale = 1000000.0;

        private List<IPCLayer> _ipcLayers;
        private XmlWriter _writer;

        public IPC2581Generator(Target target) :
            base(target) {

            _ipcLayers = new List<IPCLayer>() {
                new IPCLayer("TOP", IPCLayerSide.Top, IPCLayerFunction.Conductor),
                new IPCLayer("BOTTOM", IPCLayerSide.Bottom, IPCLayerFunction.Conductor),
                new IPCLayer("DRILL_TOP_BOTTOM", IPCLayerSide.All, IPCLayerFunction.Drill),
                new IPCLayer("OUTLINE", IPCLayerSide.None, IPCLayerFunction.Outline),
                new IPCLayer("TOP_MASK", IPCLayerSide.Top, IPCLayerFunction.SolderMask),
                new IPCLayer("BOTTOM_MASK", IPCLayerSide.Bottom, IPCLayerFunction.SolderMask)
            };
        }

        public override void Generate(EdaBoard board, string outputFolder, GeneratorOptions options = null) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            if (String.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException(nameof(outputFolder));

            string fileName = Path.Combine(outputFolder, Target.FileName);

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";

            using (_writer = XmlWriter.Create(
                new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None),
                settings)) {

                _writer.WriteStartDocument();
                _writer.WriteStartElement("IPC-2581");
                _writer.WriteAttributeString("revision", "C");

                WriteContentSection(board);
                WriteSection_Bom(board);
                WriteSection_Ecad(board);

                _writer.WriteEndElement();
                _writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Escriu la seccio 'Content'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteContentSection(EdaBoard board) {

            _writer.WriteStartElement("Content");

            _writer.WriteStartElement("DictionaryStandard");
            _writer.WriteAttributeString("units", "MILLIMETER");

            var elements = new List<EdaElement>();
            foreach (var component in board.Components) {
                foreach (var element in component.Elements)
                    elements.Add(element);
            }
            foreach (var element in board.Elements)
                elements.Add(element);

            var list = new HashSet<string>();
            foreach (var layerName in new string[] { "Top.Copper", "Bottom.Copper" }) {
                foreach (var element in elements)
                    if (element.IsOnLayer(EdaLayerId.Get(layerName))) {
                        if (element is EdaViaElement viaElement) {

                            string id = $"via_{viaElement.OuterSize}";

                            if (!list.Contains(id)) {
                                list.Add(id);

                                double diameter = viaElement.OuterSize / _scale;

                                _writer.WriteStartElement("EntryStandard");
                                _writer.WriteAttributeString("id", id);

                                _writer.WriteStartElement("Circle");
                                _writer.WriteAttributeDouble("diameter", diameter);
                                _writer.WriteEndElement();

                                _writer.WriteEndElement();
                            }
                        }

                        else if (element is EdaSmtPadElement smdPadElement) {

                            string id = $"smdpad_{smdPadElement.Size.Width}_{smdPadElement.Size.Height}_{smdPadElement.CornerSize}";

                            if (!list.Contains(id)) {
                                list.Add(id);

                                double width = smdPadElement.Size.Width / _scale;
                                double height = smdPadElement.Size.Height / _scale;
                                double radius = smdPadElement.CornerSize / _scale;

                                _writer.WriteStartElement("EntryStandard");
                                _writer.WriteAttributeString("id", id);

                                _writer.WriteStartElement(radius == 0 ? "RectCenter" : "RectRound");
                                _writer.WriteAttributeDouble("width", width);
                                _writer.WriteAttributeDouble("height", height);
                                if (radius > 0) {
                                    _writer.WriteAttributeDouble("radius", radius);
                                    _writer.WriteAttributeString("upperLeft", "TRUE");
                                    _writer.WriteAttributeString("upperRight", "TRUE");
                                    _writer.WriteAttributeString("lowerLeft", "TRUE");
                                    _writer.WriteAttributeString("lowerRight", "TRUE");
                                }
                                _writer.WriteEndElement();

                                _writer.WriteEndElement();
                            }
                        }
                    }
            }

            _writer.WriteEndElement();
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'Bom'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Bom(EdaBoard board) {

            _writer.WriteStartElement("Bom");
            _writer.WriteAttributeString("name", "bom_0");

            WriteSection_BomHeader(board);
            WriteSection_BomItem(board);

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'BomHeader'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_BomHeader(EdaBoard board) {

            _writer.WriteStartElement("BomHeader");
            _writer.WriteAttributeString("assembly", "assembly_0");
            _writer.WriteAttributeString("revision", "1.0");
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'BomItem'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_BomItem(EdaBoard board) {

            var bom = new Dictionary<string, List<EdaPart>>();
            foreach (var part in board.Parts) {
                var attribute = part.GetAttribute("MPN");
                if (attribute != null) {
                    if (!bom.TryGetValue(attribute.Value, out var value)) {
                        value = new List<EdaPart>();
                        bom.Add(attribute.Value, value);
                    }
                    value.Add(part);
                }
            }

            foreach (var item in bom) {
                _writer.WriteStartElement("BomItem");
                _writer.WriteAttributeString("OEMDesignNumberRef", item.Key);
                _writer.WriteAttributeInteger("quantity", item.Value.Count);

                foreach (var part in item.Value) {
                    _writer.WriteStartElement("RefDes");
                    _writer.WriteAttributeString("name", part.Name);
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'ECad'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Ecad(EdaBoard board) {

            _writer.WriteStartElement("Ecad");
            _writer.WriteAttributeString("name", "ecad_0");
            WriteSection_CadHeader(board);
            WriteSection_CadData(board);
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'CadHeader'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_CadHeader(EdaBoard board) {

            _writer.WriteStartElement("CadHeader");
            _writer.WriteAttributeString("units", "MILLIMETER");

            _writer.WriteStartElement("Spec");
            _writer.WriteAttributeString("name", "name_example");
            _writer.WriteAttributeString("value", "value_example");
            _writer.WriteAttributeDouble("tolPlus", 0);
            _writer.WriteAttributeDouble("tolMinus", 0);
            _writer.WriteAttributeString("global", "TRUE");
            WriteSection_Point("Location", new EdaPoint(0, 0));
            _writer.WriteEndElement();

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'CadData'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_CadData(EdaBoard board) {

            _writer.WriteStartElement("CadData");

            WriteSection_Layer(board);
            WriteSection_Stackup(board);

            _writer.WriteStartElement("Step");
            _writer.WriteAttributeString("name", "board");

            WriteSection_Point("Datum", board.Position);

            WriteProfile(board);
            WriteSection_Package(board);
            WriteSection_Component(board);
            WriteSection_LogicalNet(board);
            WriteSection_LayerFeature(board);

            _writer.WriteEndElement();

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'Package'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Package(EdaBoard board) {

            foreach (var component in board.Components) {
                _writer.WriteStartElement("Package");
                _writer.WriteAttributeString("name", component.Name);
                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'LogicalNet'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_LogicalNet(EdaBoard board) {

            foreach (var signal in board.Signals) {
                _writer.WriteStartElement("LogicalNet");
                _writer.WriteAttributeString("name", signal.Name);

                var items = board.GetConnectedItems(signal);
                if (items != null) {
                    foreach (var item in items) {
                        if (item.Item1 is EdaPadElement pad) {
                            var part = item.Item2;
                            _writer.WriteStartElement("PinRef");
                            _writer.WriteAttributeString("componentRef", part.Name);
                            _writer.WriteAttributeString("pin", pad.Name);
                            _writer.WriteEndElement();
                        }
                    }
                }

                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'Layer'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Layer(EdaBoard board) {

            foreach (var _ipcLayer in _ipcLayers) {

                string functionName = "CONDUCTOR";
                switch (_ipcLayer.Function) {
                    case IPCLayerFunction.Drill:
                        functionName = "DRILL";
                        break;

                    case IPCLayerFunction.Outline:
                        functionName = "BOARD_OUTLINE";
                        break;

                    case IPCLayerFunction.SolderMask:
                        functionName = "SOLDERMASK";
                        break;

                    case IPCLayerFunction.SolderPaste:
                        functionName = "SOLDERPASTE";
                        break;
                }

                string sideName = "NONE";
                switch (_ipcLayer.Side) {
                    case IPCLayerSide.Top:
                        sideName = "TOP";
                        break;

                    case IPCLayerSide.Bottom:
                        sideName = "BOTTOM";
                        break;

                    case IPCLayerSide.All:
                        sideName = "ALL";
                        break;
                }

                _writer.WriteStartElement("Layer");
                _writer.WriteAttributeString("name", _ipcLayer.Name);
                _writer.WriteAttributeString("layerFunction", functionName);
                _writer.WriteAttributeString("side", sideName);

                if (_ipcLayer.Function == IPCLayerFunction.Drill) {
                    _writer.WriteStartElement("Span");
                    _writer.WriteAttributeString("fromLayer", "TOP");
                    _writer.WriteAttributeString("toLayer", "BOTTOM");
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'Stackup'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Stackup(EdaBoard board) {

            _writer.WriteStartElement("Stackup");
            _writer.WriteAttributeString("name", "stackup_0");
            _writer.WriteAttributeDouble("overallThickness", 1.44);
            _writer.WriteAttributeString("whereMeasured", "METAL");
            _writer.WriteAttributeDouble("tolPlus", 0);
            _writer.WriteAttributeDouble("tolMinus", 0);
            _writer.WriteStartElement("StackupGroup");
            _writer.WriteAttributeString("name", "stackup_group_0");
            _writer.WriteAttributeDouble("thickness", 1.45);
            _writer.WriteAttributeDouble("tolPlus", 0);
            _writer.WriteAttributeDouble("tolMinus", 0);

            int sequence = 1;
            foreach (var ipcLayer in _ipcLayers) {
                if (ipcLayer.Function == IPCLayerFunction.Conductor) {
                    _writer.WriteStartElement("StackupLayer");
                    _writer.WriteAttributeString("layerOfGroupRef", ipcLayer.Name);
                    _writer.WriteAttributeDouble("thickness", 0.35);
                    _writer.WriteAttributeDouble("tolPlus", 0);
                    _writer.WriteAttributeDouble("tolMinus", 0);
                    _writer.WriteAttributeInteger("sequence", sequence++);
                    _writer.WriteEndElement();
                }
            }

            _writer.WriteEndElement();
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la la seccio 'LayerFeature'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_LayerFeature(EdaBoard board) {

            // Procesa les capes conductores.
            //
            foreach (var ipcLayer in _ipcLayers.Where(o => o.Function == IPCLayerFunction.Conductor)) {

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", ipcLayer.Name);

                EdaLayerId layerId = default;
                switch (ipcLayer.Side) {
                    case IPCLayerSide.Top:
                        layerId = EdaLayerId.TopCopper;
                        break;

                    case IPCLayerSide.Bottom:
                        layerId = EdaLayerId.BottomCopper;
                        break;
                }

                var elements = board.GetElements(board.GetLayer(layerId), false);
                foreach (var element in elements) {

                    if (element is EdaSmtPadElement smtPadElement) {
                        _writer.WriteStartElement("Set");
                        _writer.WriteStartElement("Pad");
                        WriteSection_Point("Location", smtPadElement.Position);
                        _writer.WriteStartElement("StandardPrimitiveRef");
                        _writer.WriteAttributeString("id", $"smdpad_{smtPadElement.Size.Width}_{smtPadElement.Size.Height}_{smtPadElement.CornerSize}");
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                    }

                    else if (element is EdaViaElement viaElement) {
                        _writer.WriteStartElement("Set");
                        _writer.WriteAttributeString("padUsage", "VIA");
                        _writer.WriteStartElement("Pad");
                        WriteSection_Point("Location", viaElement.Position);
                        _writer.WriteStartElement("StandardPrimitiveRef");
                        _writer.WriteAttributeString("id", $"via_{viaElement.OuterSize}");
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                    }
                }

                _writer.WriteEndElement();
            }

            // Procesa les capes de forats conductors (Vias i pads THT).
            //
            int holeNumber = 0;
            foreach (var ipcLayer in _ipcLayers.Where(i => i.Function == IPCLayerFunction.Drill)) {

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", ipcLayer.Name);

                var elements = board.GetElements(board.GetLayer(EdaLayerId.Vias), true);
                foreach (var element in elements) {
                    if (element is EdaViaElement viaElement) {
                        _writer.WriteStartElement("Set");
                        _writer.WriteStartElement("Hole");
                        _writer.WriteAttributeString("name", String.Format("H{0}", holeNumber++));
                        _writer.WriteAttributeString("platingStatus", "VIA");
                        _writer.WriteAttributeDouble("x", viaElement.Position.X / _scale);
                        _writer.WriteAttributeDouble("y", viaElement.Position.Y / _scale);
                        _writer.WriteAttributeDouble("diameter", viaElement.DrillDiameter / _scale);
                        _writer.WriteAttributeDouble("plusTol", 0);
                        _writer.WriteAttributeDouble("minusTol", 0);
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                    }
                }

                _writer.WriteEndElement();
            }

            // Procesa les capes de mascara de soldadura.
            //
            foreach (var ipcLayer in _ipcLayers.Where(x => x.Function == IPCLayerFunction.SolderMask)) {

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", ipcLayer.Name);

                EdaLayerId layerId = default;
                switch (ipcLayer.Side) {
                    case IPCLayerSide.Top:
                        layerId = EdaLayerId.TopStop;
                        break;

                    case IPCLayerSide.Bottom:
                        layerId = EdaLayerId.BottomStop;
                        break;
                }

                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la la seccio 'Component'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Component(EdaBoard board) {

            foreach (var part in board.Parts) {

                _writer.WriteStartElement("Component");
                _writer.WriteAttributeString("refDes", part.Name);
                _writer.WriteAttributeString("layerRef", "TOP");
                _writer.WriteAttributeString("packageRef", part.Component.Name);
                WriteSection_Point("Location", part.Position);
                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la descripocio d'un poligon.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// 
        private void WritePolygon(EdaPolygon polygon) {

            bool first = true;
            EdaPoint firstPoint = default;
            foreach (var point in polygon.Outline) {
                if (first) {
                    first = false;
                    firstPoint = point;
                    WriteSection_Point("PolyBegin", point);
                }
                else
                    WriteSection_Point("PolyStepSegment", point);
            }
            WriteSection_Point("PolyStepSegment", firstPoint);
        }

        private void WriteSection_Point(string name, EdaPoint point) {

            _writer.WriteStartElement(name);
            _writer.WriteAttributeDouble("x", point.X / _scale);
            _writer.WriteAttributeDouble("y", point.Y / _scale);
            _writer.WriteEndElement();

        }

        /// <summary>
        /// Escriu la descripcio del perfil de la placa.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteProfile(EdaBoard board) {

            _writer.WriteStartElement("Profile");
            _writer.WriteStartElement("Polygon");

            var polygon = board.GetOutlinePolygon();
            if (polygon != null)
                WritePolygon(polygon);

            _writer.WriteEndElement();
            _writer.WriteEndElement();
        }
    }
}
