namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System;
    using MikroPic.EdaTools.v1.Pcb.Model;

    public abstract class GerberGenerator {

        private readonly Board board;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="board">La placa a procesar.</param>
        /// 
        public GerberGenerator(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }

        /// <summary>
        /// Obte la placa.
        /// </summary>
        /// 
        public Board Board {
            get {
                return board;
            }
        }
    }
}
