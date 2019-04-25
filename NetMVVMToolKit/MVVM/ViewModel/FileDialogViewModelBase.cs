namespace MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel {

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
                if (title != value) {
                    title = value;
                    OnPropertyChange("Title");
                }
            }
        }

        public string InitialDirectory {
            get {
                return initialDirectory;
            }
            set {
                if (initialDirectory != value) {
                    initialDirectory = value;
                    OnPropertyChange("InitialDirectory");
                }
            }
        }

        public string FileName {
            get {
                return fileName;
            }
            set {
                if (fileName != value) {
                    fileName = value;
                    OnPropertyChange("FileName");
                }
            }
        }

        public string[] FileNames {
            get {
                return fileNames;
            }
            set {
                if (fileNames != value) {
                    fileNames = value;
                    OnPropertyChange("FileName");
                }
            }
        }

        public string Filter {
            get {
                return filter;
            }
            set {
                if (filter != value) {
                    filter = value;
                    OnPropertyChange("Filter");
                }
            }
        }

        public int FilterIndex {
            get {
                return filterIndex;
            }
            set {
                if (filterIndex != value) {
                    filterIndex = value;
                    OnPropertyChange("FilterIndex");
                }
            }
        }

        public string DefaultExt {
            get {
                return defaultExt;
            }
            set {
                if (defaultExt != value) {
                    defaultExt = value;
                    OnPropertyChange("DefaultExt");
                }
            }
        }

        public bool CheckPathExist {
            get {
                return checkPathExist;
            }
            set {
                if (checkPathExist != value) {
                    checkPathExist = value;
                    OnPropertyChange("CheckPathExist");
                }
            }
        }

        public bool CheckFileExist {
            get {
                return checkFileExist;
            }
            set {
                if (checkFileExist != value) {
                    checkFileExist = value;
                    OnPropertyChange("CheckFileExist");
                }
            }
        }

        public bool AddExtension {
            get {
                return addExtension;
            }
            set {
                if (addExtension != value) {
                    addExtension = value;
                    OnPropertyChange("AddExtension2");
                }
            }
        }
    }
}
