﻿namespace MikroPic.EdaTools.v1.Panelizer {

    using MikroPic.EdaTools.v1.Base.IO;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.IO;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.IO;
    using System;
    using System.IO;

    class Program {

        /// <summary>
        /// Entrada a l'aplicacio.
        /// </summary>
        /// <param name="args">Arguments de la linia de comanda.</param>
        /// 
        static void Main(string[] args) {

            ShowCredits();

            if (args.Length == 0)
                ShowHelp();

            else {
                string targetPath = null;
                string sourceFolder = null;
                bool verbose = false;
                bool pause = false;

                foreach (string arg in args) {

                    if (arg.StartsWith("/o:"))
                        targetPath = arg.Substring(3);

                    else if (arg.StartsWith("/s:"))
                        sourceFolder = arg.Substring(3);

                    else if (arg == "/v")
                        verbose = true;

                    else if (arg == "/p")
                        pause = true;
                }

                string projectPath = Path.GetFullPath(args[0]);

                if (String.IsNullOrEmpty(targetPath))
                    targetPath = Path.ChangeExtension(projectPath, ".xbrd");

                if (String.IsNullOrEmpty(sourceFolder))
                    sourceFolder = Path.GetDirectoryName(projectPath);

                if (verbose) {
                    Console.WriteLine("Project path : {0}", projectPath);
                    Console.WriteLine("Target path  : {0}", targetPath);
                    Console.WriteLine("Source folder: {0}", sourceFolder);
                    Console.WriteLine();
                }

                Project project = LoadProject(projectPath);
                Board board = GenerateBoard(project, sourceFolder);
                SaveBoard(board, targetPath);

                if (pause) {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }
            }
        }

        /// <summary>
        /// Carrega el projecte.
        /// </summary>
        /// <param name="projectPath">Ruta del projecte projecte.</param>
        /// <returns>El projecte.</returns>
        /// 
        private static Project LoadProject(string projectPath) {

            using (Stream stream = new FileStream(projectPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {

                ProjectStreamReader reader = new ProjectStreamReader(stream);
                Project project = reader.Read();

                return project;
            }
        }

        /// <summary>
        /// Guarda una placa.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="boardPath">Ruta de la placa.</param>
        /// 
        private static void SaveBoard(Board board, string boardPath) {

            using (Stream stream = new FileStream(boardPath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                BoardStreamWriter writer = new BoardStreamWriter(stream);
                writer.Write(board);
            }
        }

        /// <summary>
        /// Genera la placa.
        /// </summary>
        /// <param name="project">El projecte.</param>
        /// <param name="sourceFolder">Carpeta dels fitxers d'entrada.</param>
        /// <returns>La placa generada.</returns>
        /// 
        private static Board GenerateBoard(Project project, string sourceFolder) {

            Board targetBoard = new Board();

            FileStreamLocator sourceLocator = new FileStreamLocator();
            sourceLocator.AddFolder(sourceFolder);

            Panelizer panelizer = new Panelizer(targetBoard);
            panelizer.Panelize(project, sourceLocator);

            return targetBoard;
        }

        /// <summary>
        /// Mostra els credits del programa.
        /// </summary>
        /// 
        private static void ShowCredits() {

            string credits =
                "EdaPanelizer V1.0\r\n" +
                "(c) rsr.openware@gmail.com\r\n" +
                "\r\n";

            Console.WriteLine(credits);
        }

        /// <summary>
        /// Mostra l'ajuda.
        /// </summary>
        /// 
        private static void  ShowHelp() {

            string help =
                "EdaPanelizer V1.0\r\n" +
                "------------------------------------------------\r\n" +
                "EdaPanelizer <project> [options]\r\n" +
                "   <project>             : Project path.\r\n" +
                "   [options]             : Optional parameters.\r\n" +
                "     /p                  :   Pause at end.\r\n" +
                "     /s                  :   Source folder.\r\n" +
                "     /o                  :   Output path.\r\n" +
                "\r\n";

            Console.WriteLine(help);
        }
    }
}
