using MikroPic.EdaTools.v1.Core.IO;
using MikroPic.EdaTools.v1.Core.Model.Board;
using NetSerializer.V5;
using NetSerializer.V5.Storage.Xml;

namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Program {

        public static void Main(string[] args) {

            var board = Read("C:\\Users\\Rafael\\Documents\\Projectes\\EDA\\DSP04X\\Build\\dsp04x.xbrd");
            Write(board, "C:\\temp\\dsp04x.xml");

        }

        private static EdaBoard Read(string fileName) {

            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            var reader = new EdaBoardStreamReader(stream);
            return reader.ReadBoard();
        }

        private static void Write(EdaBoard board, string fileName) {

            var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            var writer = new XmlStorageWriter(stream, null);
            var serializer = new Serializer(writer, 100);
            try {
                serializer.Serialize(board, "Label");
            }
            finally {
                serializer.Close();
            }
        }
    }
}