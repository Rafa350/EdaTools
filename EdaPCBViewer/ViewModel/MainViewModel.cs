namespace MikroPic.EdaTools.v1.Designer.ViewModel {

    using System;
    using System.Windows.Media;
    using System.Windows.Input;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Designer.Services;
    using MikroPic.EdaTools.v1.Designer.DrawEditor;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Commands;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Services;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;

    public sealed class MainViewModel: ViewModelBase {

        private const string title = "EdaTools Viewer";

        private ICommand newCommand;
        private ICommand openCommand;
        private ICommand saveCommand;
        private ICommand saveAsCommand;
        private ICommand exitCommand;

        private readonly IAppService appService;
        private readonly IDialogService dlgService;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// 
        public MainViewModel():
            base(null) {

            ServiceLocator serviceLocator = ServiceLocator.Instance;
            appService = serviceLocator.GetService<IAppService>();
            dlgService = serviceLocator.GetService<IDialogService>();
        }

        /// <summary>
        /// Executa la comanda 'NewCommand'
        /// </summary>
        /// <param name="param">Parametere opcional</param>
        /// 
        private void NewExecute(object param) {

            appService.NewBoard();

            NotifyPropertyChanges("Title", "VisualSource");
        }

        /// <summary>
        /// Executa la comanda 'OpenCommand'
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        /// 
        private void OpenExecute(object parameter) {

            OpenFileDialogViewModel data = new OpenFileDialogViewModel(null);
            data.Title = "Open PCB Board";
            data.MultiSelect = false;
            data.CheckFileExist = true;
            data.Filter = "PCB board|*.xml";
            data.FilterIndex = 0;
            data.DefaultExt = ".xml";
            data.AddExtension = true;

            if (dlgService.ShowOpenFileDialog(data)) {

                appService.OpenBoard(data.FileName);
                NotifyPropertyChanges("Title", "VisualSource");
            }
        }

        /// <summary>
        /// Executa la comanda 'SaveCommand'
        /// </summary>
        /// <param name="param">Parametre opcional</param>
        /// 
        private void SaveExecute(object param) {

            appService.SaveBoard();
        }


        private bool SaveCanExecute(object param) {

            return appService.IsDirty;
        }


        /// <summary>
        /// Executa la comanda 'SaveAsCommand'
        /// </summary>
        /// <param name="param">Parametre opcional</param>
        /// 
        private void SaveAsExecute(object param) {

            SaveFileDialogViewModel data = new SaveFileDialogViewModel(null);
            if (dlgService.ShowSaveFileDialog(data)) {
                appService.SaveAsBoard(data.FileName);

                NotifyPropertyChange("Title");
            }
        }

        /// <summary>
        /// Executa la comanda 'ExitCommand'
        /// </summary>
        /// <param name="parameter">Parametre opcional</param>
        /// 
        private void ExitExecute(object parameter) {

            appService.Exit();
        }

        private bool ExitCanExecute(object parameter) {

            return true;
        }

        public string Title {
            get {
                string fileName = appService.FileName;
                if (String.IsNullOrEmpty(fileName))
                    return title;
                else
                    return string.Format("{0} - {1}", appService.FileName, title);
            }
        }

        public Visual VisualSource {
            get {
                Board board = appService.Board;
                if (board != null) {
                    VisualGenerator generator = new VisualGenerator(board);
                    return generator.CreateVisual();
                }
                else
                    return null;
            }
        }

        public ICommand NewCommand {
            get {
                if (newCommand == null)
                    newCommand = new RelayCommand(NewExecute);
                return newCommand;
            }
        }

        public ICommand OpenCommand {
            get {
                if (openCommand == null)
                    openCommand = new RelayCommand(OpenExecute);
                return openCommand;
            }
        }

        public ICommand SaveCommand {
            get {
                if (saveCommand == null)
                    saveCommand = new RelayCommand(SaveExecute, SaveCanExecute);
                return saveCommand;
            }
        }

        public ICommand SaveAsCommand {
            get {
                if (saveAsCommand == null)
                    saveAsCommand = new RelayCommand(SaveAsExecute);
                return saveAsCommand;
            }
        }

        public ICommand ExitCommand {
            get {
                if (exitCommand == null)
                    exitCommand = new RelayCommand(ExitExecute, ExitCanExecute);
                return exitCommand;
            }
        }
    }
}
