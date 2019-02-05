namespace MikroPic.EdaTools.v1.Core.Import {

    using System.IO;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Net;

    public abstract class Importer {

        public Board ReadBoard(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return ReadBoard(stream);
        }

        public Net ReadNet(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return ReadNet(stream);
        }

        public abstract Board ReadBoard(Stream stream);
        public abstract Net ReadNet(Stream stream);

    }
}
