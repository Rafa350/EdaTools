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

        /// <summary>
        /// Abandona l'aplicacio.
        /// </summary>
        public void Exit() {

            application.MainWindow.Close();
        }

        /// <summary>
        /// Crea una nova placa.
        /// </summary>
        /// 
        public void NewBoard() {

            fileName = "unnamed.xbrd";
            isDirty = true;

            board = new Board();
        }

        /// <summary>
        /// Obra una placa existent.
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la placa.</param>
        /// 
        public void OpenBoard(string fileName) {

            this.fileName = fileName;
            isDirty = false;

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                BoardStreamReader reader = new BoardStreamReader(stream);
                board = reader.Read();
            }
        }

        /// <summary>
        /// Guarda la placa actual.
        /// </summary>
        /// 
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

        /// <summary>
        /// Obte el nom del fitxer actual.
        /// </summary>
        /// 
        public string FileName {
            get {
                return fileName;
            }
        }

        /// <summary>
        /// Obte l'indicador de plada modificada.
        /// </summary>
        /// 
        public bool IsDirty {
            get {
                return isDirty;
            }
        }

        /// <summary>
        /// Obte la placa actual.
        /// </summary>
        /// 
        public Board Board {
            get {
                return board;
            }
        }
    }
}
