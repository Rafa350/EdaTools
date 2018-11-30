namespace MikroPic.EdaTools.v1.Designer.Services {

    using MikroPic.EdaTools.v1.Core.Model.Board;

    public interface IAppService {

        void NewBoard();
        void OpenBoard(string fileName);
        void SaveBoard();
        void SaveAsBoard(string filename);
        void Exit();

        string FileName { get; }
        bool IsDirty { get; }
        Board Board { get; }
    }
}
