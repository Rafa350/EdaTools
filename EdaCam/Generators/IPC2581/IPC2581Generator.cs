using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public sealed class IPC2581Generator: Generator {

        private const double _scale = 1000000.0;

        private List<LayerInfo> _layerInfoList;
        private XmlWriter _writer;

        public IPC2581Generator(Target target) :
            base(target) {

            _layerInfoList = new List<LayerInfo>() {
                new LayerInfo { Name = "TOP", Side = LayerSide.Top, Function = LayerFunction.Conductor },
                new LayerInfo { Name = "BOTTOM", Side = LayerSide.Bottom, Function = LayerFunction.Conductor },
                new LayerInfo { Name = "DRILL_TOP_BOTTOM", Side = LayerSide.All, Function = LayerFunction.Drill },
                new LayerInfo { Name = "OUTLINE", Side = LayerSide.None, Function = LayerFunction.Outline },
                new LayerInfo { Name = "TOP_MASK", Side = LayerSide.Top, Function = LayerFunction.SolderMask },
                new LayerInfo { Name = "BOTTOM_MASK", Side = LayerSide.Bottom, Function = LayerFunction.SolderMask },
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

                                double diameter = viaElement.OuterSize / 1000000.0;

                                _writer.WriteStartElement("EntryStandard");
                                _writer.WriteAttributeString("id", id);

                                _writer.WriteStartElement("Circle");
                                _writer.WriteAttributeString("diameter", XmlConvert.ToString(diameter));
                                _writer.WriteEndElement();

                                _writer.WriteEndElement();
                            }
                        }

                        else if (element is EdaSmtPadElement smdPadElement) {

                            string id = $"smdpad_{smdPadElement.Size.Width}_{smdPadElement.Size.Height}_{smdPadElement.CornerSize}";

                            if (!list.Contains(id)) {
                                list.Add(id);

                                double width = smdPadElement.Size.Width / 1000000.0;
                                double height = smdPadElement.Size.Height / 1000000.0;
                                double radius = smdPadElement.CornerSize / 1000000.0;

                                _writer.WriteStartElement("EntryStandard");
                                _writer.WriteAttributeString("id", id);

                                _writer.WriteStartElement(radius == 0 ? "RectCenter" : "RectRound");
                                _writer.WriteAttributeString("width", XmlConvert.ToString(width));
                                _writer.WriteAttributeString("height", XmlConvert.ToString(height));
                                if (radius > 0) {
                                    _writer.WriteAttributeString("radius", XmlConvert.ToString(radius));
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
                _writer.WriteAttributeString("quantity", XmlConvert.ToString(item.Value.Count));

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
            _writer.WriteAttributeString("tolPlus", "0.02");
            _writer.WriteAttributeString("tolMinus", "0.02");
            _writer.WriteAttributeString("global", "TRUE");

            _writer.WriteStartElement("Location");
            _writer.WriteAttributeString("x", "0.00");
            _writer.WriteAttributeString("y", "0.00");
            _writer.WriteEndElement();

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

            WritePoint("Datum", board.Position);

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

            foreach (var layerInfo in _layerInfoList) {

                string functionName = "CONDUCTOR";
                switch (layerInfo.Function) {
                    case LayerFunction.Drill:
                        functionName = "DRILL";
                        break;

                    case LayerFunction.Outline:
                        functionName = "BOARD_OUTLINE";
                        break;

                    case LayerFunction.SolderMask:
                        functionName = "SOLDERMASK";
                        break;

                    case LayerFunction.SolderPaste:
                        functionName = "SOLDERPASTE";
                        break;
                }

                string sideName = "NONE";
                switch (layerInfo.Side) {
                    case LayerSide.Top:
                        sideName = "TOP";
                        break;

                    case LayerSide.Bottom:
                        sideName = "BOTTOM";
                        break;

                    case LayerSide.All:
                        sideName = "ALL";
                        break;
                }

                _writer.WriteStartElement("Layer");
                _writer.WriteAttributeString("name", layerInfo.Name);
                _writer.WriteAttributeString("layerFunction", functionName);
                _writer.WriteAttributeString("side", sideName);

                if (layerInfo.Function == LayerFunction.Drill) {
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
            _writer.WriteAttributeString("overallThickness", "1.44");
            _writer.WriteAttributeString("whereMeasured", "METAL");
            _writer.WriteAttributeString("tolPlus", "0");
            _writer.WriteAttributeString("tolMinus", "0");
            _writer.WriteStartElement("StackupGroup");
            _writer.WriteAttributeString("name", "stackup_group_0");
            _writer.WriteAttributeString("thickness", "1.45");
            _writer.WriteAttributeString("tolPlus", "0");
            _writer.WriteAttributeString("tolMinus", "0");

            int sequence = 1;
            foreach (var layerInfo in _layerInfoList) {
                if (layerInfo.Function == LayerFunction.Conductor) {
                    _writer.WriteStartElement("StackupLayer");
                    _writer.WriteAttributeString("layerOfGroupRef", layerInfo.Name);
                    _writer.WriteAttributeString("thickness", "0.35");
                    _writer.WriteAttributeString("tolPlus", "0");
                    _writer.WriteAttributeString("tolMinus", "0");
                    _writer.WriteAttributeString("sequence", XmlConvert.ToString(sequence++));
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
            foreach (var layerInfo in _layerInfoList.Where(x => x.Function == LayerFunction.Conductor)) {

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", layerInfo.Name);

                EdaLayerId layerId = default;
                switch (layerInfo.Side) {
                    case LayerSide.Top:
                        layerId = EdaLayerId.TopCopper;
                        break;

                    case LayerSide.Bottom:
                        layerId = EdaLayerId.BottomCopper;
                        break;
                }

                var elements = board.GetElements(board.GetLayer(layerId), false);
                foreach (var element in elements) {

                    if (element is EdaSmtPadElement smdPadElement) {
                        _writer.WriteStartElement("Set");
                        _writer.WriteStartElement("Pad");
                        WritePoint("Location", smdPadElement.Position);
                        _writer.WriteStartElement("StandardPrimitiveRef");
                        _writer.WriteAttributeString("id", $"smdpad_{smdPadElement.Size.Width}_{smdPadElement.Size.Height}_{smdPadElement.CornerSize}");
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                    }

                    else if (element is EdaViaElement viaElement) {
                        _writer.WriteStartElement("Set");
                        _writer.WriteAttributeString("padUsage", "VIA");
                        _writer.WriteStartElement("Pad");
                        WritePoint("Location", viaElement.Position);
                        _writer.WriteStartElement("StandardPrimitiveRef");
                        _writer.WriteAttributeString("id", $"via_{viaElement.OuterSize}");
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                    }
                }

                _writer.WriteEndElement();
            }

            // Procesa les capes de forats
            //
            int holeNumber = 0;
            foreach (var layerInfo in _layerInfoList.Where(x => x.Function == LayerFunction.Drill)) {

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", layerInfo.Name);

                var elements = board.GetElements(board.GetLayer(EdaLayerId.Vias), true);
                foreach (var element in elements) {
                    if (element is EdaViaElement viaElement) {
                        _writer.WriteStartElement("Set");
                        _writer.WriteStartElement("Hole");
                        _writer.WriteAttributeString("name", String.Format("H{0}", holeNumber++));
                        _writer.WriteAttributeString("platingStatus", "VIA");
                        _writer.WriteAttributeString("x", XmlConvert.ToString(viaElement.Position.X / _scale));
                        _writer.WriteAttributeString("y", XmlConvert.ToString(viaElement.Position.Y / _scale));
                        _writer.WriteAttributeString("diameter", XmlConvert.ToString(viaElement.DrillDiameter / _scale));
                        _writer.WriteAttributeString("plusTol", "0");
                        _writer.WriteAttributeString("minusTol", "0");
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                    }
                }

                _writer.WriteEndElement();
            }

            // Procesa les capes de mascara de soldadura
            //
            foreach (var layerInfo in _layerInfoList.Where(x => x.Function == LayerFunction.SolderMask)) {

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", layerInfo.Name);

                EdaLayerId layerId = default;
                switch (layerInfo.Side) {
                    case LayerSide.Top:
                        layerId = EdaLayerId.TopStop;
                        break;

                    case LayerSide.Bottom:
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
                WritePoint("Location", part.Position);
                _writer.WriteEndElement();
            }
        }

        private void WritePoint(string name, EdaPoint point) {

            _writer.WriteStartElement(name);
            _writer.WriteAttributeString("x", XmlConvert.ToString(point.X / _scale));
            _writer.WriteAttributeString("y", XmlConvert.ToString(point.Y / _scale));
            _writer.WriteEndElement();
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
                    WritePoint("PolyBegin", point);
                }
                else
                    WritePoint("PolyStepSegment", point);
            }
            WritePoint("PolyStepSegment", firstPoint);
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
