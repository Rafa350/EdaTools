namespace MikroPic.EdaTools.v1.CamTool {

    using MikroPic.EdaTools.v1.Cam;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;
    using MikroPic.EdaTools.v1.Pcb.Model.PanelElements;
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

                string inputFileName = Path.GetFullPath(args[0]);
                string folder = Path.GetDirectoryName(inputFileName);
                string name = Path.GetFileNameWithoutExtension(inputFileName);
                bool pause = false;

                if (args.Length > 1) {

                    for (int i = 1; i < args.Length; i++) {
                        string arg = args[i];

                        if (arg.StartsWith("/n:"))
                            name = arg.Substring(3);

                        else if (arg.StartsWith("/f:"))
                            folder = arg.Substring(3);

                        else if (arg == "/p")
                            pause = true;
                    }
                }

                string extension = Path.GetExtension(inputFileName);
                if (!String.IsNullOrEmpty(extension))
                    switch (extension.ToUpper()) {
                        case ".XBRD":
                            ProcessBoard(inputFileName, folder, name);
                            break;

                        case ".XPNL":
                            ProcessPanel(inputFileName, folder, name);
                            break;
                    }

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
                "edacamtool <board> [options]\r\n" +
                "   <input>               : Board/Panel input file name.\r\n" +
                "   [options]             : Optional parameters.\r\n" +
                "     /f:<folder>         :   Output folder.\r\n" +
                "     /n:<name>           :   Output file name prefix.\r\n" +
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
        /// <param name="fileName">Nom del fitxer de la placa.</param>
        /// 
        private static void ProcessBoard(string fileName, string folder, string name) {

            Panel panel = new Panel();
            panel.AddElement(new PlaceElement(fileName));

            CAMGenerator cg = new CAMGenerator();
            cg.Generate(panel, folder, name);
        }

        /// <summary>
        /// Procesa un panell
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la placa.</param>
        /// 
        private static void ProcessPanel(string fileName, string folder, string name) {

            Panel panel;

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                PanelStreamReader reader = new PanelStreamReader(stream);
                panel = reader.Read();
            }

            foreach (var element in panel.Elements) {
                if (element is PlaceElement) {
                    PlaceElement place = (PlaceElement) element;
                    place.FileName = Path.Combine(folder, place.FileName);
                }
            }

            CAMGenerator cg = new CAMGenerator();
            cg.Generate(panel, folder, name);
        }
    }
}
