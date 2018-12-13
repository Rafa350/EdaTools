namespace MikroPic.EdaTools.v1.PanelEditor.Services {

    using MikroPic.EdaTools.v1.Panel.Model;

    public interface IAppService {

        void NewProject();
        void OpenProject(string fileName);
        void SaveProject();
        void SaveAsProject(string filename);
        void Exit();

        string FileName { get; }
        bool IsDirty { get; }
        Project Project { get; }
    }
}
