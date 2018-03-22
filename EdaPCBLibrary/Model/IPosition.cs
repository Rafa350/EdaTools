namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Geometry;

    /// <summary>
    /// Interficie per tots els objectes que tenen posicio.
    /// </summary>
    public interface IPosition {

        PointInt Position { get; set; }
    }
}
