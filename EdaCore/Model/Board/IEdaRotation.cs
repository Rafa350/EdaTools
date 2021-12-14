namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Base.Geometry;

    /// <summary>
    /// Interficie per tots els elements que tenen rotacio.
    /// </summary>
    /// 
    public interface IEdaRotation {

        EdaAngle Rotation { get; set; }
    }
}
