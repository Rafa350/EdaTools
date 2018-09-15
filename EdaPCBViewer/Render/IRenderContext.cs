namespace MikroPic.EdaTools.v1.Designer.Render {

    using MikroPic.EdaTools.v1.Pcb.Model;

    public interface IRenderContext {

       ISceneGraph Render(Board board);
    }
}
