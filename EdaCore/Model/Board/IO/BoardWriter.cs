using System.IO;
using NetSerializer;
using NetSerializer.Storage.Xml;

namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

    public sealed class BoardWriter {

        private const int _version = 500;

        public void Write(Stream stream, Board board) {

            var writer = new XmlStorageWriter(stream, null);
            var serializer = new Serializer(writer, _version);
            try {
                serializer.Serialize(board, "Board");
            }
            finally {
                serializer.Close();
            }
        }

        public static int Version =>
            _version;
    }
}
