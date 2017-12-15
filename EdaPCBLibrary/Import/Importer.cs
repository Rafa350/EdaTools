namespace MikroPic.EdaTools.v1.Pcb.Import {

    using System.IO;
    using MikroPic.EdaTools.v1.Pcb.Model;

    public abstract class Importer {

        public Board LoadBoard(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return LoadBoard(stream);
        }

        public Library LoadLibrary(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return LoadLibrary(stream);
        }

        public abstract Board LoadBoard(Stream stream);

        public abstract Library LoadLibrary(Stream stream);
    }
}
