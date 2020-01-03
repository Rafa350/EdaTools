namespace MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel.Dialogs {
   
    public interface IFileDialogViewModel {

        string Title { get; set; }
        string Filter { get; set; }
        int FilterIndex { get; set; }
        string InitialDirectory { get; set; }
        string DefaultExt { get; set; }
        string FileName { get; set; }
        string[] FileNames { get; set; }
        bool CheckPathExist { get; set; }
        bool CheckFileExist { get; set; }
        bool AddExtension { get; set; }
    }
}
