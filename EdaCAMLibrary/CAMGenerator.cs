namespace MikroPic.EdaTools.v1.Cam {

    using MikroPic.EdaTools.v1.Cam.Gerber;
    using MikroPic.EdaTools.v1.Cam.Ipcd356;
    using MikroPic.EdaTools.v1.Cam.Model;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;
    using System;
    using System.IO;

    public sealed class CAMGenerator {

        /// <summary>
        /// Genera els fitxers per defecte.
        /// </summary>
        /// <param name="project">El projecte CAM a procesar.</param>
        /// <param name="folder">La carpeta on deixar els fitxers.</param>
        /// <param name="name">Prefix del nom dels fitxers de sortida.</param>
        /// 
        public void Generate(Project project, string folder, string name) {

            if (project == null)
                throw new ArgumentNullException("project");

            Board board = LoadBoard(project);

            foreach (var target in project.Targets) {
                Generator generator = LoadGenerator(target);
                generator.Generate(board);
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
        /// Carrega la placa.
        /// </summary>
        /// <param name="project">El projecte.</param>
        /// <returns>La placa.</returns>
        /// 
        private Board LoadBoard(Project project) {

            Board board;

            using (Stream stream = new FileStream(project.BoardFileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                BoardStreamReader reader = new BoardStreamReader(stream);
                board = reader.Read();
            }

            return board;
        }
    }
}

