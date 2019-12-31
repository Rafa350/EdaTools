namespace MikroPic.EdaTools.v1.Cam.Generators {

    using System;
    using MikroPic.EdaTools.v1.Cam.Model;
    using MikroPic.EdaTools.v1.Core.Model.Board;

    public abstract class Generator {

        private readonly Target target;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="target">El target.</param>
        /// 
        public Generator(Target target) {

            if (target == null)
                throw new ArgumentNullException("target");

            this.target = target;
        }

        /// <summary>
        /// Genera el fitxer de contingut.
        /// </summary>
        /// <param name="board">La placa a procesar</param>
        /// <param name="outputFolder">Carpeta de sortida.</param>
        /// <param name="options">Opcions.</param>
        /// 
        public abstract void Generate(Board board, string outputFolder, GeneratorOptions options = null);

        /// <summary>
        /// Obte el target.
        /// </summary>
        /// 
        public Target Target {
            get {
                return target;
            }
        }
    }
}
