namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Geometry;

    /// <summary>
    /// Interficie per a tots els objectes que tenen tamany.
    /// </summary>
    /// 
    public interface ISize {

        SizeInt Size { get; set; }
    }
}
