namespace MikroPic.EdaTools.v1.Extractor {

    using System;
    using System.IO;
    using System.Xml;

    using MikroPic.EdaTools.v1.Core.Model.Board;

    public sealed class NetExtractor {

        private readonly EdaBoard board;

        public NetExtractor(EdaBoard board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }

        public void Extract(TextWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            XmlWriterSettings wrSettings = new XmlWriterSettings();
            wrSettings.Indent = true;
            wrSettings.IndentChars = "    ";
            wrSettings.CloseOutput = true;
            using (XmlWriter wr = XmlWriter.Create(writer, wrSettings)) {
            }
        }
    }
}

