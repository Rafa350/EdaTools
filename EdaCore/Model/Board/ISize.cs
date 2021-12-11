using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Interficie per a tots els objectes que tenen tamany.
    /// </summary>
    /// 
    public interface ISize {

        EdaSize Size { get; set; }
    }
}
