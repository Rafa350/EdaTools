namespace MikroPic.EdaTools.v1.Pcb.Model.PanelElements {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.IO;

    public sealed class PlaceElement: PanelElement, IPosition {

        private static Dictionary<string, Board> boardCache = new Dictionary<string, Board>();

        private string fileName;
        private Point position;
        private Angle orientation;
        private Board board;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la plada.</param>
        /// 
        public PlaceElement(string fileName):
            this(fileName, new Point(0, 0), Angle.Zero) { 
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la placa.</param>
        /// <param name="position">Posicio de la placa d'ins del panell.</param>
        /// <param name="orientation">Orientacio de la placa centrat en la posicio.</param>
        /// 
        public PlaceElement(string fileName, Point position, Angle orientation):
            base(position, orientation) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            this.fileName = fileName;
            this.position = position;
            this.orientation = orientation;
        }

        /// <summary>
        /// Obte o carrega si cal, la placa.
        /// </summary>
        /// <returns>La placa.</returns>
        /// 
        private Board GetBoard() {

            if (!boardCache.TryGetValue(fileName, out board)) {
                using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                    BoardReader reader = new BoardReader(stream);
                    board = reader.Read();
                    boardCache.Add(fileName, board);
                }
            }
            return board;
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
            set {
                position = value;
            }
            get {
                return position;
            }
        }

        public Angle Orientation {
            get {
                return orientation;
            }
        }

        public Board Board {
            get {
                return GetBoard();
            }
        }
    }
}
