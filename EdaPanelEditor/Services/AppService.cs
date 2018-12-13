namespace MikroPic.EdaTools.v1.PanelEditor.Services {

    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.IO;
    using System.IO;
    using System.Windows;

    public sealed class AppService : IAppService {

        private readonly Application application;
        private Project project;
        private string fileName;
        private bool isDirty;

        public AppService(Application application) {

            this.application = application;
        }

        /// <summary>
        /// Abandona l'aplicacio.
        /// </summary>
        /// 
        public void Exit() {

            application.MainWindow.Close();
        }

        /// <summary>
        /// Crea una nou projecte.
        /// </summary>
        /// 
        public void NewProject() {

            fileName = "unnamed.xpnl";
            isDirty = true;

            project = new Project();
        }

        /// <summary>
        /// Obra una projecte existent.
        /// </summary>
        /// <param name="fileName">Nom del fitxer del projecte.</param>
        /// 
        public void OpenProject(string fileName) {

            this.fileName = fileName;
            isDirty = false;

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                ProjectStreamReader reader = new ProjectStreamReader(stream);
                project = reader.Read();
            }
        }

        /// <summary>
        /// Guarda el projecte.
        /// </summary>
        /// 
        public void SaveProject() {

            isDirty = false;

            /*using (Stream outStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                XmlBoardWriter writer = new XmlBoardWriter(outStream);
                writer.Write(board);
            }*/
        }

        /// <summary>
        /// Guarda el projecte amb un altre nom.
        /// </summary>
        /// <param name="fileName">El nom del fitxer del projecte.</param>
        /// 
        public void SaveAsProject(string fileName) {

            this.fileName = fileName;
            isDirty = false;
        }

        /// <summary>
        /// Obte el nom del fitxer de l'ultim projecte obert.
        /// </summary>
        /// 
        public string FileName {
            get {
                return fileName;
            }
        }

        /// <summary>
        /// Indica si el projecte ha estat modificat.
        /// </summary>
        /// 
        public bool IsDirty {
            get {
                return isDirty;
            }
        }

        /// <summary>
        /// Obte el projecte.
        /// </summary>
        /// 
        public Project Project {
            get {
                return project;
            }
        }
    }
}
