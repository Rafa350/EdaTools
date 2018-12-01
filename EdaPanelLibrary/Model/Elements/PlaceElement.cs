namespace MikroPic.EdaTools.v1.Panel.Model.Elements {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.IO;
    using MikroPic.EdaTools.v1.Panel.Model;

    public sealed class PlaceElement: PanelElement {

        private static Dictionary<string, Board> boardCache = new Dictionary<string, Board>();

        private string fileName;
        private Board board;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la placa.</param>
        /// 
        public PlaceElement(string fileName):
            base() { 
            
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            this.fileName = fileName;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la placa.</param>
        /// <param name="position">Posicio de la placa d'ins del panell.</param>
        /// <param name="rotation">Angle de rotacio de la placa centrat en la posicio.</param>
        /// 
        public PlaceElement(string fileName, Point position, Angle rotation):
            base(position, rotation) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            this.fileName = fileName;
        }

        public override void AcceptVisitor(MikroPic.EdaTools.v1.Panel.Model.IVisitor visitor) {
        }

        /// <summary>
        /// Obte o carrega si cal, la placa.
        /// </summary>
        /// <returns>La placa.</returns>
        /// 
        private Board GetBoard() {

            if (!boardCache.TryGetValue(fileName, out board)) {
                using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                    BoardStreamReader reader = new BoardStreamReader(stream);
                    board = reader.Read();
                    boardCache.Add(fileName, board);
                }
            }
            return board;
        }

        /// <summary>
        /// Obte o asigna el nom del fitxer de la placa.
        /// </summary>
        /// 
        public string FileName {
            set {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("PlaveElement.FileName");

                fileName = value;
            }
            get {
                return fileName;
            }
        }

        /// <summary>
        /// Obte la placa.
        /// </summary>
        public Board Board {
            get {
                return GetBoard();
            }
        }
    }
}
