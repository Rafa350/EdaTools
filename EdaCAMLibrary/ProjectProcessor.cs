namespace MikroPic.EdaTools.v1.Cam {

    using MikroPic.EdaTools.v1.Cam.Gerber;
    using MikroPic.EdaTools.v1.Cam.Ipcd356;
    using MikroPic.EdaTools.v1.Cam.Model;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;
    using System;
    using System.IO;

    public sealed class ProjectProcessor {

        /// <summary>
        /// Genera els fitxers per defecte.
        /// </summary>
        /// <param name="project">El projecte CAM a procesar.</param>
        /// <param name="inpFolder">La carpeta dels fitxers d'entrada.</param>
        /// <param name="outFolder">La carpeta on deixar els fitxers de sortida.</param>
        /// <param name="outPrefix">Prefix del nom dels fitxers de sortida.</param>
        /// 
        public void Process(Project project, string inpFolder, string outFolder, string outPrefix) {

            if (project == null)
                throw new ArgumentNullException("project");

            Board board = LoadBoard(project, inpFolder);

            foreach (var target in project.Targets) {
                Generator generator = LoadGenerator(target);
                generator.Generate(board, outFolder, outPrefix);
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
        /// Carrega una placa.
        /// </summary>
        /// <param name="project">El projecte.</param>
        /// <param name="inpFolder">La carpeta dles fitxers d'entrada.</param>
        /// <returns>La placa.</returns>
        /// 
        private Board LoadBoard(Project project, string inpFolder) {

            Board board;

            string fileName = Path.Combine(inpFolder, project.BoardFileName);

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                BoardStreamReader reader = new BoardStreamReader(stream);
                board = reader.Read();
            }

            return board;
        }
    }
}

