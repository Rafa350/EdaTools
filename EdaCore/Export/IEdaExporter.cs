using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Core.Export {

    public interface IEdaExporter {

        /// <summary>
        /// Escriu una llibreria en un fitxer
        /// </summary>
        /// <param name="targetPath">Destinacio.</param>
        /// <param name="library">La llibraria.</param>
        /// 
        void WriteLibrary(string targetPath, EdaLibrary library);
    }
}
