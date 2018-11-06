namespace MikroPic.EdaTools.v1.CamTool {

    using MikroPic.EdaTools.v1.Cam;
    using System;
    using System.IO;

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

                string projectFileName = Path.GetFullPath(args[0]);
                bool pause = false;

                if (args.Length > 1) {

                    for (int i = 1; i < args.Length; i++) {
                        string arg = args[i];

                        if (arg == "/p")
                            pause = true;
                    }
                }

                ProcessProject(projectFileName);

                if (pause)
                    WaitKey();
            }
        }

        /// <summary>
        /// Mostra els credits.
        /// </summary>
        /// 
        private static void ShowCredits() {

        }

        /// <summary>
        /// Mostra l'ajuda.
        /// </summary>
        /// 
        private static void ShowHelp() {

            string help =
                "EdaCAMTool V1.1\r\n" +
                "---------------\r\n" +
                "edacamtool <project> [options]\r\n" +
                "   <project>             : Project file name.\r\n" +
                "   [options]             : Optional parameters.\r\n" +
                "     /z:<zip>            :   Output ZIP file name.\r\n" +
                "     /p                  :   Pause at end.\r\n";

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
        /// Procesa una placa
        /// </summary>
        /// <param name="fileName">Nom del fitxer del projecte.</param>
        /// 
        private static void ProcessProject(string fileName) {

            ProjectProcessor cg = new ProjectProcessor();
            cg.Process(fileName);
        }
    }
}
