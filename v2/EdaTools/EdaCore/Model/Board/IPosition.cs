namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Base.Geometry;

    /// <summary>
    /// Interficie per tots els objectes que tenen posicio.
    /// </summary>
    /// 
    public interface IPosition {

        Point Position { get; set; }
    }
}
