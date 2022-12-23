using System;
using MikroPic.EdaTools.v1.Cam.Generators;
using MikroPic.EdaTools.v1.Cam.Generators.Gerber;
using MikroPic.EdaTools.v1.Cam.Generators.GerberJob;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581;
using MikroPic.EdaTools.v1.Cam.Generators.IPC356;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Cam {

    public sealed class CamProcessor {

        private readonly Project _project;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="project">El projecte a procesar.</param>
        /// 
        public CamProcessor(Project project) {

            if (project == null)
                throw new ArgumentNullException(nameof(project));

            _project = project;
        }

        /// <summary>
        /// Procesa una placa.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="targetName">Nom del target a procesar. Si es null, els procesa tots.</param>
        /// <param name="outputFolder">Carpeta de sortida.</param>
        /// 
        public void Process(EdaBoard board, string targetName, string outputFolder) {

            foreach (var target in _project.Targets) {
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

                case "gerber-route":
                    return new GerberRouteGenerator(target);

                case "gerber-component":
                    return new GerberComponentGenerator(target);

                case "gerber-job":
                    return new GerberJobGenerator(target);

                case "ipc-D356":
                    return new IPC356Generator(target);

                case "ipc-2581":
                    return new IPC2581Generator(target);

                default:
                    throw new InvalidOperationException("Tipo de generador desconocido.");
            }
        }
    }
}

