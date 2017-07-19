namespace MikroPic.EdaTools.v1 {

    using System;
    using System.IO;
    using MikroPic.EdaTools.v1.Model;
    using MikroPic.EdaTools.v1.Model.IO;

    class Program {

        static void Main(string[] args) {

            BoardLoader loader = new BoardLoader();
            Board board = loader.Load(@"c:\temp\board.brd");

            XmlBoardWriter boardWriter = new XmlBoardWriter(
                new FileStream(@"c:\temp\board.xml", FileMode.Create, FileAccess.Write, FileShare.None));
            boardWriter.Write(board);

        }
    }
}
