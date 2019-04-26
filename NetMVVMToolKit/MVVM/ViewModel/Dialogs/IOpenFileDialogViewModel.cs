namespace MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel.Dialogs {

    public interface IOpenFileDialogViewModel: IFileDialogViewModel {

        bool MultiSelect { get; set; }
        bool ShowReadOnly { get; set; }
        bool ReadOnlyChecked { get; set; }
    }
}
