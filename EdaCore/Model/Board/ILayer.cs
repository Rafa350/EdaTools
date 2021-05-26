namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Interficie per tots els objectes que tenen capa explicita.
    /// </summary>
    /// 
    public interface ILayer {

        LayerId LayerId { get; set; }
    }
}
