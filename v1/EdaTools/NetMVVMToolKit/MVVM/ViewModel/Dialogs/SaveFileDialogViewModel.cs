﻿namespace MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel.Dialogs {

    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;

    public sealed class SaveFileDialogViewModel: FileDialogViewModelBase, ISaveFileDialogViewModel {

        public SaveFileDialogViewModel(ViewModelBase parent)
            : base(parent) {
        }
    }
}