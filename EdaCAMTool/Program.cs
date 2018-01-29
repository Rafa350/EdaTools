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

                string inputFileName = Path.GetFullPath(args[0]);
                string folder = Path.GetDirectoryName(inputFileName);
                string name = Path.GetFileNameWithoutExtension(inputFileName);

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

        private static void ShowCredits() {

        }

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

        private static void WaitKey() {

            Console.WriteLine("Pres key for continue...");
            Console.ReadKey(true);
        }

        private static void ProcessBoard(string fileName) {

            Board board = LoadBoard(fileName);
            SaveBoard(board, @"..\..\..\Data\out.xml");
        }

        private static Board LoadBoard(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                XmlBoardReader reader = new XmlBoardReader(stream);
                return reader.Read();
            }
        }

        private static void SaveBoard(Board board, string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                XmlBoardWriter writer = new XmlBoardWriter(stream);
                writer.Write(board);
            }
        }
    }
}
