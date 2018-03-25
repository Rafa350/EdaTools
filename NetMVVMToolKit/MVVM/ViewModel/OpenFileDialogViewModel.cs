namespace MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel {

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
                if (multiSelect != value) {
                    multiSelect = value;
                    NotifyPropertyChange("MultiSelect");
                }
            }
        }

        public bool ShowReadOnly {
            get {
                return showReadOnly;
            }
            set {
                if (showReadOnly != value) {
                    showReadOnly = value;
                    NotifyPropertyChange("ShowReadOnly");
                }
            }
        }

        public bool ReadOnlyChecked {
            get {
                return readOnlyChecked;
            }
            set {
                if (readOnlyChecked != value) {
                    readOnlyChecked = value;
                    NotifyPropertyChange("ReadOnlyChecked");
                }
            }
        }
    }
}
