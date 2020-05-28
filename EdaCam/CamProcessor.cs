namespace MikroPic.EdaTools.v1.Cam {

    using System;
    using MikroPic.EdaTools.v1.Cam.Generators;
    using MikroPic.EdaTools.v1.Cam.Generators.Gerber;
    using MikroPic.EdaTools.v1.Cam.Generators.Ipcd356;
    using MikroPic.EdaTools.v1.Cam.Model;
    using MikroPic.EdaTools.v1.Core.Model.Board;

    public sealed class CamProcessor {

        private readonly Project project;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="project">El projecte a procesar.</param>
        /// 
        public CamProcessor(Project project) {

            if (project == null)
                throw new ArgumentNullException(nameof(project));

            this.project = project;
        }

        /// <summary>
        /// Procesa una placa.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="targetName">Nom del target a procesar. Si es null, els procesa tots.</param>
        /// <param name="outputFolder">Carpeta de sortida.</param>
        /// 
        public void Process(Board board, string targetName, string outputFolder) {

            foreach (var target in project.Targets) {
                if ((targetName == null) || (target.Name == targetName)) {
                    Generator generator = LoadGenerator(target);
                    generator.Generate(board, outputFolder);
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
    }
}

