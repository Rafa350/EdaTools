namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Geometry;

    /// <summary>
    /// Interficie per tots els elements que tenen rotacio.
    /// </summary>
    /// 
    public interface IRotation {

        Angle Rotation { get; set; }
    }
}
