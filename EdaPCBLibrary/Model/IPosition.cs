namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System.Windows;

    /// <summary>
    /// Interficie per tots els elements que tenen posicio.
    /// </summary>
    public interface IPosition {

        Point Position { get; set; }
    }
}
