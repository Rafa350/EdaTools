namespace MikroPic.EdaTools.v1.CamTool {

    using System;
    using System.IO;
    using MikroPic.EdaTools.v1.Cam;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;

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

                string inputFileName = Path.GetFullPath(args[0]);
                string folder = Path.GetDirectoryName(inputFileName);
                string name = Path.GetFileNameWithoutExtension(inputFileName);

                if (args.Length > 1) {
                    for (int i = 1; i < args.Length; i++) {
                        string arg = args[i];

                        if (arg.StartsWith("/n:"))
                            name = arg.Substring(3);

                        else if (arg.StartsWith("/f:"))
                            folder = arg.Substring(3);
                    }
                }

                ProcessBoard(inputFileName, folder, name);
            }

            WaitKey();
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
                "EdaCAMTool V1.0\r\n" +
                "---------------\r\n" +
                "edacamtool <board> [options]\r\n" +
                "   <board>               : PCB input file name.\r\n" +
                "   [options]             : Optional parameters.\r\n" +
                "     /f:<folder>         :   Output folder.\r\n" +
                "     /n:<name>           :   Output file name prefix.\r\n" +
                "     /z:<zip>;           :   Output ZIP file name.\r\n";

            Console.WriteLine(help);            
        }

        /// <summary>
        /// Espera que es premi una tecla per continuar.
        /// </summary>
        /// 
        private static void WaitKey() {

            Console.WriteLine("Pres key for continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Procesa la placa
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la plada.</param>
        /// 
        private static void ProcessBoard(string fileName, string folder, string name) {

            Board board = LoadBoard(fileName);

            CAMGenerator cg = new CAMGenerator();
            cg.Generate(board, folder, name);
        }

        /// <summary>
        /// Carrega la placa desde un fitxer.
        /// </summary>
        /// <param name="fileName">Nom de fitxer.</param>
        /// <returns>la placa carregada.</returns>
        /// 
        private static Board LoadBoard(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                XmlBoardReader reader = new XmlBoardReader(stream);
                return reader.Read();
            }
        }

        /// <summary>
        /// Salva una plada en un fitxer.
        /// </summary>
        /// <param name="board">La placa a salvar.</param>
        /// <param name="fileName">El nom del fitxer.</param>
        /// 
        private static void SaveBoard(Board board, string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                XmlBoardWriter writer = new XmlBoardWriter(stream);
                writer.Write(board);
            }
        }
    }
}
