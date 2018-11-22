namespace MikroPic.EdaTools.v1.Cam {

    using MikroPic.EdaTools.v1.Cam.Gerber;
    using MikroPic.EdaTools.v1.Cam.Ipcd356;
    using MikroPic.EdaTools.v1.Cam.Model;
    using MikroPic.EdaTools.v1.Cam.Model.IO;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;
    using System;
    using System.IO;

    public sealed class ProjectProcessor {

        /// <summary>
        /// Procesa un projecte.
        /// </summary>
        /// <param name="projectFileName">Nom del fitxer del projecte.</param>
        /// <param name="targetName">Nom del target a procesar. Si es null, es procesan tots.</param>
        /// 
        public void Process(string projectFileName, string targetName) {

            if (String.IsNullOrEmpty(projectFileName))
                throw new ArgumentNullException("projectFileName");

            string projectFolder = Path.GetDirectoryName(projectFileName);
            Project project = LoadProject(projectFileName);
            Process(project, targetName, projectFolder);
        }

        /// <summary>
        /// Procesa un projecte.
        /// </summary>
        /// <param name="project">El projecte.</param>
        /// <param name="targetName">Nom del target a procesar. Si es null, els procesa tots.</param>
        /// <param name="projectFolder">Carpeta del projecte.</param>
        /// 
        public void Process(Project project, string targetName, string projectFolder) {

            if (project == null)
                throw new ArgumentNullException("project");

            Board board = LoadBoard(project, projectFolder);

            foreach (var target in project.Targets) {
                if ((targetName == null) || (target.Name == targetName)) {
                    Generator generator = LoadGenerator(target);
                    generator.Generate(board, projectFolder);
                }
            }
        }

        /// <summary>
        /// Carrega un generador.
        /// </summary>
        /// <param name="target">El target.</param>
        /// <returns>El generador.</returns>
        /// 
        private Generator LoadGenerator(Target target) {

            switch (target.GeneratorName) {
                case "gerber-image":
                    return new GerberImageGenerator(target);

                case "gerber-drill":
                    return new GerberDrillGenerator(target);

                case "ipc-D356":
                    return new Ipcd356Generator(target);

                default:
                    throw new InvalidOperationException("Tipo de generador desconocido.");
            }
        }

        /// <summary>
        /// Carrega un projecte.
        /// </summary>
        /// <param name="projectFileName">Nom del fitxer del projecte.</param>
        /// <returns>El projecte.</returns>
        /// 
        private Project LoadProject(string projectFileName) {

            Project project;
            using (Stream stream = new FileStream(projectFileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                ProjectStreamReader reader = new ProjectStreamReader(stream);
                project = reader.Read();
            }
            return project;
        }

        /// <summary>
        /// Carrega una placa.
        /// </summary>
        /// <param name="project">El projecte.</param>
        /// <param name="projectFolder">La carpeta.</param>
        /// <returns>La placa.</returns>
        /// 
        private Board LoadBoard(Project project, string projectFolder) {

            string boardFileName = Path.Combine(projectFolder, project.BoardFileName);

            Board board;
            using (Stream stream = new FileStream(boardFileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                BoardStreamReader reader = new BoardStreamReader(stream);
                board = reader.Read();
            }
            return board;
        }
    }
}

