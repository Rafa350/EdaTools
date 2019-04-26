namespace MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel.Dialogs {

    public abstract class FileDialogViewModelBase: ViewModelBase, IFileDialogViewModel {

        private string title;
        private string filter;
        private int filterIndex;
        private string initialDirectory;
        private string defaultExt;
        private string fileName;
        private string[] fileNames;
        private bool checkFileExist;
        private bool checkPathExist;
        private bool addExtension;

        public FileDialogViewModelBase(ViewModelBase parent)
            : base(parent) {
        }

        public string Title {
            get {
                return title;
            }
            set {
                SetProperty<string>(ref title, value, "Title");
            }
        }

        public string InitialDirectory {
            get {
                return initialDirectory;
            }
            set {
                SetProperty<string>(ref initialDirectory, value, "InitialDirectory");
            }
        }

        public string FileName {
            get {
                return fileName;
            }
            set {
                SetProperty<string>(ref fileName, value, "FileName");
            }
        }

        public string[] FileNames {
            get {
                return fileNames;
            }
            set {
                SetProperty<string[]>(ref fileNames, value, "FileNames");
            }
        }

        public string Filter {
            get {
                return filter;
            }
            set {
                SetProperty<string>(ref filter, value, "Filter");
            }
        }

        public int FilterIndex {
            get {
                return filterIndex;
            }
            set {
                SetProperty<int>(ref filterIndex, value, "FilterIndex");
            }
        }

        public string DefaultExt {
            get {
                return defaultExt;
            }
            set {
                SetProperty<string>(ref defaultExt, value, "DefaultExt");
            }
        }

        public bool CheckPathExist {
            get {
                return checkPathExist;
            }
            set {
                SetProperty<bool>(ref checkPathExist, value, "CheckPathExist");
            }
        }

        public bool CheckFileExist {
            get {
                return checkFileExist;
            }
            set {
                SetProperty<bool>(ref checkFileExist, value, "CheckFileExist");
            }
        }

        public bool AddExtension {
            get {
                return addExtension;
            }
            set {
                SetProperty<bool>(ref addExtension, value, "AddExtension");
            }
        }
    }
}
