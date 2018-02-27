namespace MikroPic.EdaTools.v1.Converter {

    using MikroPic.EdaTools.v1.Pcb.Import;
    using MikroPic.EdaTools.v1.Pcb.Import.Eagle;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;
    using System.IO;

    class Program {

        static void Main(string[] args) {

            string path = @"..\..\..\Data";

            Importer importer = new EagleImporter();
            Board board = importer.Read(Path.Combine(path, "board3.brd"));

            XmlBoardWriter boardWriter = new XmlBoardWriter(
                new FileStream(Path.Combine(path, "board3.xml"), FileMode.Create, FileAccess.Write, FileShare.None));
            boardWriter.Write(board);
        }
    }
}

