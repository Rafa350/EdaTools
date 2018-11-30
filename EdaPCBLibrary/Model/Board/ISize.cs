namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Geometry;

    /// <summary>
    /// Interficie per a tots els objectes que tenen tamany.
    /// </summary>
    /// 
    public interface ISize {

        Size Size { get; set; }
    }
}
