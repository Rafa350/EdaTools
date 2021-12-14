using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Net;

namespace MikroPic.EdaTools.v1.Core.Import {

    public interface IEdaImporter {

        /// <summary>
        /// Importa una placa
        /// </summary>
        /// <param name="fileName">Nom del fitxer.</param>
        /// <returns>La placa.</returns>
        /// 
        EdaBoard ReadBoard(string fileName);

        /// <summary>
        /// Importa una llibraria
        /// </summary>
        /// <param name="fileName">Nom del fitxer.</param>
        /// <returns>La llibreria.</returns>
        /// 
        EdaLibrary ReadLibrary(string fileName);

        /// <summary>
        /// Importa una netlist
        /// </summary>
        /// <param name="fileName">Nom del fitxer.</param>
        /// <returns>La netlist.</returns>
        /// 
        Net ReadNet(string fileName);
    }
}
