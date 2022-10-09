using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581
{

    public sealed class IPC2581Generator: Generator {

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
                writer.WriteStartElement("IPC-2581A");

                WriteContentSection(writer, board);
                WriteEcadSection(writer, board);

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
        private void WriteContentSection(XmlWriter writer, EdaBoard board) {

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

                                writer.WriteStartElement("EntityStandard");
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

                                writer.WriteStartElement("EntityStandard");
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
        /// Escriu la seccio 'ECad'
        /// </summary>
        /// <param name="writer">El escriptor XML</param>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteEcadSection(XmlWriter writer, EdaBoard board) {

            writer.WriteStartElement("Ecad");

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

            writer.WriteStartElement("CadData");

            WriteBoardSection(writer, board);

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private void WriteBoardSection(XmlWriter writer, EdaBoard board) {

            writer.WriteStartElement("Step");
            writer.WriteAttributeString("name", "board");

            writer.WriteStartElement("Datum");
            writer.WriteAttributeString("x", XmlConvert.ToString(board.Position.X / 1000000.0));
            writer.WriteAttributeString("y", XmlConvert.ToString(board.Position.Y / 1000000.0));
            writer.WriteEndElement();

            writer.WriteStartElement("Profile");
            writer.WriteStartElement("Polygon");

            var polygon = board.GetOutlinePolygon();
            if (polygon != null)
                WritePolygon(writer, polygon);

            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private void WritePolygon(XmlWriter writer, EdaPolygon polygon) {

            bool first = true;
            foreach (var point in polygon.Points) {

                if (first) {
                    first = false;
                    writer.WriteStartElement("PolyBegin");
                }
                else
                    writer.WriteStartElement("PolyStepSegment");

                writer.WriteAttributeString("x", XmlConvert.ToString(point.X / 1000000.0));
                writer.WriteAttributeString("y", XmlConvert.ToString(point.Y / 1000000.0));
                writer.WriteEndElement();
            }
        }
    }
}
