namespace MikroPic.EdaTools.v1.Pcb.Import {

    using System.IO;
    using MikroPic.EdaTools.v1.Pcb.Model;

    public abstract class Importer {

        public Board Read(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return Read(stream);
        }

        public abstract Board Read(Stream stream);
    }
}
