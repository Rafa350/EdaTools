using System;
using System.IO;
using System.Xml;

using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public sealed class IPC2581Generator : Generator {

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

                WriteEcadSection(writer, board);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

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

        private void WritePolygon(XmlWriter writer, Polygon polygon) {

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
