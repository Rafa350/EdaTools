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
                wr.WriteStartElement("blocks");

                foreach (Block block in board.Blocks) {

                    wr.WriteStartElement("block");

                    foreach (Part part in board.Parts)
                        if (part.Block == block) {

                            wr.WriteStartElement("");
                            wr.WriteEndElement();
                        }

                    wr.WriteEndElement();
                }

                wr.WriteEndElement();
                wr.WriteEndElement();
                wr.WriteEndDocument();
            }
        }
    }
}
