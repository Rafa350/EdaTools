namespace MikroPic.EdaTools.v1.Extractor {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using System.IO;
    using System.Xml;

    public sealed class PartExtractor {

        private readonly Board board;

        public PartExtractor(Board board) {

            this.board = board;
        }

        public void Extract(TextWriter writer) {

            XmlWriterSettings wrSettings = new XmlWriterSettings();
            wrSettings.Indent = true;
            wrSettings.IndentChars = "    ";
            wrSettings.CloseOutput = true;
            using (XmlWriter wr = XmlWriter.Create(writer, wrSettings)) {

                wr.WriteStartDocument();
                wr.WriteStartElement("board");

                foreach (Part part in board.Parts) {
                    wr.WriteStartElement("part");
                    wr.WriteAttributeString("name", part.Name);
                    wr.WriteAttributeString("position", part.Name);
                    wr.WriteAttributeString("rotation", part.Name);
                    wr.WriteAttributeString("side", part.Name);

                    wr.WriteStartElement("attributes");
                    foreach (PartAttribute attribute in part.Attributes) {
                        wr.WriteStartElement("attribute");
                        wr.WriteAttributeString("name", attribute.Name);
                        if (attribute.Value != null)
                            wr.WriteAttributeString("value", attribute.Value);
                        wr.WriteEndElement();
                    }
                    wr.WriteEndElement();

                    wr.WriteEndElement();
                }

                wr.WriteEndElement();
                wr.WriteEndDocument();
            }
        }
    }
}
