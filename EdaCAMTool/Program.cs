namespace MikroPic.EdaTools.v1.CamTool {

    using System;
    using System.IO;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;

    class Program {

        static void Main(string[] args) {

            ShowCredits();

            if (args.Length == 0)
                ShowHelp();

            else {

                string inputFileName = args[0];
                string folder = null;
                string name = null;

                if (args.Length > 1) {
                    for (int i = 2; i < args.Length; i++) {
                        string arg = args[i];

                        if (arg.StartsWith("/n:"))
                            name = arg.Substring(3);

                        else if (arg.StartsWith("/f:"))
                            folder = arg.Substring(3);
                    }
                }

                ProcessBoard(inputFileName);
            }

            WaitKey();
        }

        static void ShowCredits() {

        }

        static void ShowHelp() {

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

        static void WaitKey() {

            Console.WriteLine("Pres key for continue...");
            Console.ReadKey(true);
        }

        static void ProcessBoard(string fileName) {

            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            XmlBoardReader reader = new XmlBoardReader(stream);

            Board board = reader.Read();
        }
    }
}
