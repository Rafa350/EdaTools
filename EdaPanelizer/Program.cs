namespace MikroPic.EdaTools.v1.Panelizer {

    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.IO;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using MikroPic.EdaTools.v1.Panel.Model.IO;
    using System;
    using System.IO;

    class Program {

        static void Main(string[] args) {

            ShowCredits();

            if (args.Length == 0)
                ShowHelp();

            else {
                string projectFileName = Path.GetFullPath(args[0]);
                string boardFileName = Path.ChangeExtension(projectFileName, ".xbrd");

                foreach (string arg in args) {

                }

                Project panel = LoadProject(projectFileName);
                Board board = GenerateBoard(panel);
                SaveBoard(board, boardFileName);
            }
        }

        /// <summary>
        /// Carrega el projecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer del projecte.</param>
        /// <returns>El projecte.</returns>
        /// 
        private static Project LoadProject(string fileName) {

            string folder = Path.GetDirectoryName(fileName);

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                ProjectStreamReader reader = new ProjectStreamReader(stream);

                Project project = reader.Read();

                foreach (var item in project.Items) {
                    if (item is PcbItem pcbItem) {
                        pcbItem.FileName = Path.Combine(folder, pcbItem.FileName);
                    }
                }

                return project;
            }
        }

        /// <summary>
        /// Guarda una placa.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="fileName">Nom del fitxer de la placa.</param>
        /// 
        private static void SaveBoard(Board board, string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                BoardStreamWriter writer = new BoardStreamWriter(stream);
                writer.Write(board);
            }
        }

        /// <summary>
        /// Genera la placa.
        /// </summary>
        /// <param name="project">El projecte.</param>
        /// <returns>La placa generada.</returns>
        /// 
        private static Board GenerateBoard(Project project) {

            Board board = new Board();

            Panelizer panelizer = new Panelizer(board);
            panelizer.Panelize(project);

            return board;
        }

        /// <summary>
        /// Mostra els credits del programa.
        /// </summary>
        /// 
        private static void ShowCredits() {

            string credits =
                "EdaPanelizer V1.0\r\n";

            Console.WriteLine(credits);
        }

        /// <summary>
        /// Mostra l'ajuda.
        /// </summary>
        /// 
        private static void  ShowHelp() {

            string help =
                "EdaPanelizer V1.0\r\n" +
                "---------------\r\n" +
                "EdaPanelizer <project> [options]\r\n" +
                "   <project>             : Project file name.\r\n" +
                "   [options]             : Optional parameters.\r\n" +
                "     /p                  :   Pause at end.\r\n" +
                "     /s                  :   Source files path.\r\n" +
                "     /o                  :   Output file name.\r\n";

            Console.WriteLine(help);
        }
    }
}
