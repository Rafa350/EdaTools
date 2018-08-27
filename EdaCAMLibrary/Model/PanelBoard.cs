namespace MikroPic.EdaTools.v1.Cam.Model {

    using System;
    using MikroPic.EdaTools.v1.Geometry;

    public sealed class PanelBoard: PanelElement {
    
        private string fileName;
        private Point position;
        private Angle orientation;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la plada.</param>
        /// 
        public PanelBoard(string fileName):
            this(fileName, new Point(0, 0), Angle.Zero) { 
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la placa.</param>
        /// <param name="position">Posicio de la placa d'ins del panell.</param>
        /// <param name="orientation">Orientacio de la placa centrat en la posicio.</param>
        /// 
        public PanelBoard(string fileName, Point position, Angle orientation):
            base(position, orientation) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            this.fileName = fileName;
            this.position = position;
            this.orientation = orientation;
        }

        /// <summary>
        /// Obte el nom del fitxer.
        /// </summary>
        /// 
        public string FileName {
            get {
                return fileName;
            }
        }

        public Point Position {
            get {
                return position;
            }
        }

        public Angle Orientation {
            get {
                return orientation;
            }
        }
    }
}
