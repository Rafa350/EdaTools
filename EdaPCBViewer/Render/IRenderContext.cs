namespace MikroPic.EdaTools.v1.Designer.Render {

    using MikroPic.EdaTools.v1.Core.Model.Board;

    public interface IRenderContext {

       ISceneGraph Render(Board board);
    }
}
