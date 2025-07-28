using MikroPic.EdaTools.v1.Core.IO;
using MikroPic.EdaTools.v1.Core.Model.Board;
using NetSerializer.V6;
using NetSerializer.V6.Formatters.Xml;

namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Program {

        public static void Main(string[] args) {

            var board = Read(@"C:\Users\Rafael\Documents\Projectes\EDA\cpu04c\build\cpu04c.xbrd");
            Write(board, @"C:\temp\cpu04x.xml");

        }

        private static EdaBoard Read(string fileName) {

            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            var reader = new EdaBoardStreamReader(stream);
            return reader.ReadBoard();
        }

        private static void Write(EdaBoard board, string fileName) {

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (var writer = new XmlFormatWriter(stream, 300)) {
                    var serializer = new Serializer();
                    serializer.Serialize(writer, board);
                }
            }
        }
    }
}