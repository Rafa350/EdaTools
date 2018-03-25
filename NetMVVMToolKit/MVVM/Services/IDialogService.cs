namespace MikroPic.NetMVVMToolkit.v1.MVVM.Services {

    using System;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;

    public interface IDialogService {

        void Register(Type viewModelType, Type viewType);
        void ShowNotification(string title, string text);
        bool ShowAskOkCancel(string title, string text);
        bool ShowAskYesNo(string title, string text);
        bool ShowOpenFileDialog(IOpenFileDialogViewModel viewModel);
        bool ShowSaveFileDialog(ISaveFileDialogViewModel viewModel);
        bool ShowDialog(ViewModelBase viemModel);
    }
}
