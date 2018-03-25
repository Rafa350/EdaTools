namespace MikroPic.EdaTools.v1.Designer.Services {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;
    using System.IO;
    using System.Windows;

    public sealed class AppService : IAppService {

        private readonly Application application;
        private Board board;
        private string fileName;
        private bool isDirty;

        public AppService(Application application) {

            this.application = application;
        }

        public void Exit() {

            application.MainWindow.Close();
        }

        public void NewBoard() {

            fileName = "unnamed.xml";
            isDirty = true;

            board = new Board();
        }

        public void OpenBoard(string fileName) {

            this.fileName = fileName;
            isDirty = false;

            using (Stream inStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                XmlBoardReader reader = new XmlBoardReader(inStream);
                board = reader.Read();
            }
        }

        public void SaveBoard() {

            isDirty = false;

            /*using (Stream outStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                XmlBoardWriter writer = new XmlBoardWriter(outStream);
                writer.Write(board);
            }*/
        }

        public void SaveAsBoard(string fileName) {

            this.fileName = fileName;
            isDirty = false;
        }

        public string FileName {
            get {
                return fileName;
            }
        }

        public bool IsDirty {
            get {
                return isDirty;
            }
        }

        public Board Board {
            get {
                return board;
            }
        }
    }
}
