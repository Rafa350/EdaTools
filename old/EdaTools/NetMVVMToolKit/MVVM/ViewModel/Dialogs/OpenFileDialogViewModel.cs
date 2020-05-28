namespace MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel.Dialogs {

    public sealed class OpenFileDialogViewModel: FileDialogViewModelBase, IOpenFileDialogViewModel {

        private bool multiSelect = false;
        private bool showReadOnly = false;
        private bool readOnlyChecked = false;

        public OpenFileDialogViewModel(ViewModelBase parent)
            : base(parent) {
        }

        public bool MultiSelect {
            get {
                return multiSelect;
            }
            set {
                SetProperty<bool>(ref multiSelect, value, "MultiSelect");
            }
        }

        public bool ShowReadOnly {
            get {
                return showReadOnly;
            }
            set {
                SetProperty<bool>(ref showReadOnly, value, "ShowReadOnly");
            }
        }

        public bool ReadOnlyChecked {
            get {
                return readOnlyChecked;
            }
            set {
                SetProperty<bool>(ref readOnlyChecked, value, "ReadOnlyChecked");
            }
        }
    }
}
