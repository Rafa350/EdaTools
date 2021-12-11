namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System.Collections.Generic;

    using MikroPic.EdaTools.v1.Base.Geometry;

    public static class EdaBoardExtensions {

        /// <summary>
        /// Obte els elements d'ins del rectangle especificat.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="r">El rectangle</param>
        /// <returns>Enumera ela elements trobats. Null si no hi ha cap.</returns>
        /// 
        public static IEnumerable<EdaElement> FindElement(this EdaBoard board, Rect r) {

            return null;
        }
    }
}
