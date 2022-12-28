using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public static class EdaBoardExtensions {

        /// <summary>
        /// Obte els elements d'ins del rectangle especificat.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="r">El rectangle</param>
        /// <returns>Enumera ela elements trobats. Null si no hi ha cap.</returns>
        /// 
        public static IEnumerable<EdaElementBase> FindElement(this EdaBoard board, EdaRect r) {

            return null;
        }
    }
}
