using System;
using System.Collections.Generic;
using System.Drawing;
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

        public IPC2581Generator(Target target) :
            base(target) {
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

            using (var writer = XmlWriter.Create(
                new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None),
                settings)) {

                writer.WriteStartDocument();
                writer.WriteStartElement("IPC-2581");
                writer.WriteAttributeString("revision", "C");

                WriteContentSection(writer, board);
                WriteSection_Bom(writer, board);
                WriteSection_Ecad(writer, board);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Escriu la seccio 'Content'
        /// </summary>
        /// <param name="writer">El escriptor XML.</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteContentSection(XmlWriter writer, EdaBoard board) {

            writer.WriteStartElement("Content");

            writer.WriteStartElement("DictionaryStandard");
            writer.WriteAttributeString("units", "MILLIMETER");

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

                                writer.WriteStartElement("EntryStandard");
                                writer.WriteAttributeString("id", id);

                                writer.WriteStartElement("Circle");
                                writer.WriteAttributeString("diameter", XmlConvert.ToString(diameter));
                                writer.WriteEndElement();

                                writer.WriteEndElement();
                            }
                        }

                        else if (element is EdaSmdPadElement smdPadElement) {

                            string id = $"smdpad_{smdPadElement.Size.Width}_{smdPadElement.Size.Height}_{smdPadElement.CornerSize}";

                            if (!list.Contains(id)) {
                                list.Add(id);

                                double width = smdPadElement.Size.Width / 1000000.0;
                                double height = smdPadElement.Size.Height / 1000000.0;
                                double radius = smdPadElement.CornerSize / 1000000.0;

                                writer.WriteStartElement("EntryStandard");
                                writer.WriteAttributeString("id", id);

                                writer.WriteStartElement(radius == 0 ? "RectCenter" : "RectRound");
                                writer.WriteAttributeString("width", XmlConvert.ToString(width));
                                writer.WriteAttributeString("height", XmlConvert.ToString(height));
                                if (radius > 0) {
                                    writer.WriteAttributeString("radius", XmlConvert.ToString(radius));
                                    writer.WriteAttributeString("upperLeft", "TRUE");
                                    writer.WriteAttributeString("upperRight", "TRUE");
                                    writer.WriteAttributeString("lowerLeft", "TRUE");
                                    writer.WriteAttributeString("lowerRight", "TRUE");
                                }
                                writer.WriteEndElement();

                                writer.WriteEndElement();
                            }
                        }
                    }
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'Bom'
        /// </summary>
        /// <param name="writer">El escriptor XML.</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteSection_Bom(XmlWriter writer, EdaBoard board) {

            writer.WriteStartElement("Bom");
            writer.WriteAttributeString("name", "bom_0");
            
            WriteSection_BomHeader(writer, board);
            WriteSection_BomItem(writer, board);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'BomHeader'
        /// </summary>
        /// <param name="writer">L'escriptor XML.</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteSection_BomHeader(XmlWriter writer, EdaBoard board) {

            writer.WriteStartElement("BomHeader");
            writer.WriteAttributeString("assembly", "assembly_0");
            writer.WriteAttributeString("revision", "1.0");
            writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'BomItem'
        /// </summary>
        /// <param name="writer">L'escriptor XML.</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteSection_BomItem(XmlWriter writer, EdaBoard board) {

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
                writer.WriteStartElement("BomItem");
                writer.WriteAttributeString("OEMDesignNumberRef", item.Key);
                writer.WriteAttributeString("quantity", XmlConvert.ToString(item.Value.Count));

                foreach (var part in item.Value) {
                    writer.WriteStartElement("RefDes");
                    writer.WriteAttributeString("name", part.Name);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'ECad'
        /// </summary>
        /// <param name="writer">L'escriptor XML.</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteSection_Ecad(XmlWriter writer, EdaBoard board) {

            writer.WriteStartElement("Ecad");
            writer.WriteAttributeString("name", "ecad_0");
            WriteSection_CadHeader(writer, board);
            WriteSection_CadData(writer, board);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'CadHeader'
        /// </summary>
        /// <param name="writer">L'escriptor XML</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteSection_CadHeader(XmlWriter writer, EdaBoard board) {

            writer.WriteStartElement("CadHeader");
            writer.WriteAttributeString("units", "MILLIMETER");

            writer.WriteStartElement("Spec");
            writer.WriteAttributeString("name", "name_example");
            writer.WriteAttributeString("value", "value_example");
            writer.WriteAttributeString("tolPlus", "0.02");
            writer.WriteAttributeString("tolMinus", "0.02");
            writer.WriteAttributeString("global", "TRUE");

            writer.WriteStartElement("Location");
            writer.WriteAttributeString("x", "0.00");
            writer.WriteAttributeString("y", "0.00");
            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'CadData'
        /// </summary>
        /// <param name="writer">L'scriptor XML</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteSection_CadData(XmlWriter writer, EdaBoard board) {

            writer.WriteStartElement("CadData");

            WriteSection_Layer(writer, board);

            writer.WriteStartElement("Stackup");
            writer.WriteAttributeString("name", "stackup_0");
            writer.WriteAttributeString("tolPlus", "0");
            writer.WriteAttributeString("tolMinus", "0");
            writer.WriteStartElement("StackupGroup");
            writer.WriteAttributeString("name", "stackup_group_0");
            writer.WriteAttributeString("thickness", "1.45");
            writer.WriteAttributeString("tolPlus", "0");
            writer.WriteAttributeString("tolMinus", "0");
            foreach (var layer in board.Layers) {
                writer.WriteStartElement("StackupLayer");
                writer.WriteAttributeString("layerOfGroupRef", layer.Name);
                writer.WriteAttributeString("materialType", "COPPER");
                writer.WriteAttributeString("thickness", "0.35");
                writer.WriteAttributeString("tolPlus", "0");
                writer.WriteAttributeString("tolMinus", "0");
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("Step");
            writer.WriteAttributeString("name", "board");

            WritePoint(writer, "Datum", board.Position);

            WriteProfile(writer, board);
            WriteSection_Package(writer, board);
            WriteSection_Component(writer, board);
            WriteSection_LogicalNet(writer, board);
            WriteSection_LayerFeature(writer, board);

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'Package'
        /// </summary>
        /// <param name="writer">El escriptor XML</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteSection_Package(XmlWriter writer, EdaBoard board) {

            foreach (var component in board.Components) {
                writer.WriteStartElement("Package");
                writer.WriteAttributeString("name", component.Name);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'LogicalNet'
        /// </summary>
        /// <param name="writer">El escriptor XML.</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteSection_LogicalNet(XmlWriter writer, EdaBoard board) {

            foreach (var signal in board.Signals) {
                writer.WriteStartElement("LogicalNet");
                writer.WriteAttributeString("name", signal.Name);

                var items = board.GetConnectedItems(signal);
                if (items != null) {
                    foreach (var item in items) {
                        if (item.Item1 is EdaPadElement pad) {
                            var part = item.Item2;
                            writer.WriteStartElement("PinRef");
                            writer.WriteAttributeString("componentRef", part.Name);
                            writer.WriteAttributeString("pin", pad.Name);
                            writer.WriteEndElement();
                        }
                    }
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'Layer'
        /// </summary>
        /// <param name="writer">El escriptor XML.</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteSection_Layer(XmlWriter writer, EdaBoard board) {

            foreach (var layer in board.Layers) {

                string sideName = "NONE";
                switch (layer.Side) {
                    case BoardSide.Top:
                        sideName = "TOP";
                        break;

                    case BoardSide.Bottom:
                        sideName = "BOTTOM";
                        break;

                    case BoardSide.Inner:
                        sideName = "INTERNAL";
                        break;
                }

                string layerFunction = "OTHER";
                switch (layer.Function) {
                    case LayerFunction.Signal:
                        layerFunction = "SIGNAL";
                        break;

                    case LayerFunction.Outline:
                        layerFunction = "BOARD_OUTLINE";
                        break;
                }

                writer.WriteStartElement("Layer");
                writer.WriteAttributeString("name", layer.Name);
                writer.WriteAttributeString("layerFunction", layerFunction);
                writer.WriteAttributeString("side", sideName);
                writer.WriteAttributeString("polarity", "POSITIVE");
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la la seccio 'LayerFeature'.
        /// </summary>
        /// <param name="writer">El escriptor XML.</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteSection_LayerFeature(XmlWriter writer, EdaBoard board) {

            foreach (var layer in board.Layers) {
                writer.WriteStartElement("LayerFeature");
                writer.WriteAttributeString("layerRef", layer.Name);

                var elements = board.GetElements(layer, true);
                foreach (var element in elements) {
                    if (element is EdaSmdPadElement smdPadElement) {
                        writer.WriteStartElement("Set"); 
                        writer.WriteStartElement("Pad");
                        WritePoint(writer, "Location", smdPadElement.Position);
                        writer.WriteStartElement("StandardPrimitiveRef");
                        string id = $"smdpad_{smdPadElement.Size.Width}_{smdPadElement.Size.Height}_{smdPadElement.CornerSize}";
                        writer.WriteAttributeString("id", id);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }

                    else if (element is EdaViaElement viaElement) {
                        writer.WriteStartElement("Set");
                        writer.WriteAttributeString("padUsage", "VIA");
                        writer.WriteStartElement("Pad");
                        WritePoint(writer, "Location", viaElement.Position);
                        writer.WriteStartElement("StandardPrimitiveRef");
                        string id = $"via_{viaElement.OuterSize}";
                        writer.WriteAttributeString("id", id);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la la seccio 'Component'.
        /// </summary>
        /// <param name="writer">El escriptor XML.</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteSection_Component(XmlWriter writer, EdaBoard board) {

            foreach (var part in board.Parts) {

                writer.WriteStartElement("Component");
                writer.WriteAttributeString("refDes", part.Name);
                writer.WriteAttributeString("layerRef", "Top.Copper");
                writer.WriteAttributeString("packageRef", part.Component.Name);
                WritePoint(writer, "Location", part.Position);
                writer.WriteEndElement();
            }
        }

        private static void WritePoint(XmlWriter writer, string name, EdaPoint point) {

            writer.WriteStartElement(name);
            writer.WriteAttributeString("x", XmlConvert.ToString(point.X / _scale));
            writer.WriteAttributeString("y", XmlConvert.ToString(point.Y / _scale));
            writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la descripocio d'un poligon.
        /// </summary>
        /// <param name="writer">El escriptor XML.</param>
        /// <param name="polygon">El poligon.</param>
        /// 
        private static void WritePolygon(XmlWriter writer, EdaPolygon polygon) {

            bool first = true;
            EdaPoint firstPoint = default;
            foreach (var point in polygon.Outline) {
                if (first) {
                    first = false;
                    firstPoint = point;
                    WritePoint(writer, "PolyBegin", point);
                }
                else
                    WritePoint(writer, "PolyStepSegment", point);
            }
            WritePoint(writer, "PolyStepSegment", firstPoint);
        }

        /// <summary>
        /// Escriu la descripcio del perfil de la placa.
        /// </summary>
        /// <param name="writer">El escriptor XML.</param>
        /// <param name="board">La placa.</param>
        /// 
        private static void WriteProfile(XmlWriter writer, EdaBoard board) {

            writer.WriteStartElement("Profile");
            writer.WriteStartElement("Polygon");

            var polygon = board.GetOutlinePolygon();
            if (polygon != null)
                WritePolygon(writer, polygon);

            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
