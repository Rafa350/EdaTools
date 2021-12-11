using System;
using System.IO;

using MikroPic.EdaTools.v1.Cam;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Cam.Model.IO;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.IO;

namespace MikroPic.EdaTools.v1.CamTool {

    class Program {

        /// <summary>
        /// Entrada a l'aplicacio.
        /// </summary>
        /// <param name="args">Llista d'arguments.</param>
        /// 
        static void Main(string[] args) {

            ShowCredits();

            if (args.Length == 0)
                ShowHelp();

            else {

                string projectPath = args[0];
                string boardPath = null; ;
                string targetName = null;
                string outputFolder = null;
                bool pause = false;
                bool verbose = false;

                if (args.Length > 1) {

                    for (int i = 1; i < args.Length; i++) {
                        string arg = args[i];

                        if (arg.StartsWith("/t:"))
                            targetName = arg.Substring(3);

                        else if (arg.StartsWith("/b:"))
                            boardPath = arg.Substring(3);

                        else if (arg.StartsWith("/o:"))
                            outputFolder = arg.Substring(3);

                        else if (arg == "/v")
                            verbose = true;

                        else if (arg == "/p")
                            pause = true;
                    }
                }

                if (boardPath == null)
                    boardPath = Path.ChangeExtension(projectPath, ".xbrd");

                if (outputFolder == null)
                    outputFolder = Path.GetDirectoryName(projectPath);

                if (verbose) {
                    Console.WriteLine("| Target name  : {0}", targetName);
                    Console.WriteLine("| Project file : {0}", projectPath);
                    Console.WriteLine("| Board file   : {0}", boardPath);
                    Console.WriteLine("| Output folder: {0}", outputFolder);
                    Console.WriteLine("+--------------------------------------------------------------------");
                    Console.WriteLine();
                }

                Project project = LoadProject(projectPath);
                EdaBoard board = LoadBoard(boardPath);
                ProcessProject(project, board, targetName, outputFolder);

                if (pause)
                    WaitKey();
            }
        }

        /// <summary>
        /// Mostra els credits.
        /// </summary>
        /// 
        private static void ShowCredits() {

            string credits =
                "+--------------------------------------------------------------------\r\n" +
                "| EdaCamTool V2.1 - (c) 2019..2021 rsr.openware@gmail.com\r\n" +
                "+--------------------------------------------------------------------";

            Console.WriteLine(credits);
        }

        /// <summary>
        /// Mostra l'ajuda.
        /// </summary>
        /// 
        private static void ShowHelp() {

            string help =
                "| EdaCamTool <project> [options]\r\n" +
                "|     <project>   : Project file name.\r\n" +
                "|     [options]   : Optional parameters.\r\n" +
                "|         /o      :   Output path\r\n" +
                "|         /p      :   Pause at end.\r\n" +
                "|         /t      :   Target to process.\r\n" +
                "|         /v      :   Verbose.\r\n" +
                "|         /z      :   Output ZIP file name.\r\n" +
                "+--------------------------------------------------------------------";

            Console.WriteLine(help);
        }

        /// <summary>
        /// Espera que es premi una tecla per continuar.
        /// </summary>
        /// 
        private static void WaitKey() {

            Console.WriteLine("Press key for continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Carrega una placa.
        /// </summary>
        /// <param name="boardPath">La ruta de la placa.</param>
        /// <returns>La placa.</returns>
        /// 
        private static EdaBoard LoadBoard(string boardPath) {

            using (var stream = new FileStream(boardPath, FileMode.Open, FileAccess.Read, FileShare.None)) {
                var reader = new BoardStreamReader(stream);
                return reader.Read();
            }
        }

        /// <summary>
        /// Carrega el projecte.
        /// </summary>
        /// <param name="projectPath">La ruta del projecte.</param>
        /// <returns>El projecte.</returns>
        /// 
        private static Project LoadProject(string projectPath) {

            using (var stream = new FileStream(projectPath, FileMode.Open, FileAccess.Read, FileShare.None)) {
                var reader = new ProjectStreamReader(stream);
                return reader.Read();
            }
        }

        /// <summary>
        /// Processa el projecte
        /// </summary>
        /// <param name="project">Nom del fitxer del projecte.</param>
        /// <param name="board">Nom del target a procesar. Si es null, procesa tots.</param>
        /// <param name="targetName">El nom del target.</param>
        /// <param name="outputFolder">Carpeta de sortida.</param>
        /// 
        private static void ProcessProject(Project project, EdaBoard board, string targetName, string outputFolder) {

            CamProcessor camProcessor = new CamProcessor(project);
            camProcessor.Process(board, targetName, outputFolder);
        }
    }
}
