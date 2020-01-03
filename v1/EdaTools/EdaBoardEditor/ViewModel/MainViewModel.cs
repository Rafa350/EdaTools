namespace MikroPic.EdaTools.v1.BoardEditor.ViewModel {

    using System;
    using System.Windows.Input;
    using MikroPic.EdaTools.v1.BoardEditor.Services;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Commands;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Services;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel.Dialogs;

    public sealed class MainViewModel: ViewModelBase {

        private const string title = "EdaTools Board Editor";

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

            OnMultiplePropertyChanged("Title", "Board");
        }

        /// <summary>
        /// Executa la comanda 'OpenCommand'
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        /// 
        private void OpenExecute(object parameter) {

            OpenFileDialogViewModel data = new OpenFileDialogViewModel(null);
            data.Title = "Open board";
            data.MultiSelect = false;
            data.CheckFileExist = true;
            data.Filter = "Board (*.xbrd)|*.xbrd";
            data.FilterIndex = 0;
            data.DefaultExt = ".xbrd";
            data.AddExtension = true;

            if (dlgService.ShowOpenFileDialog(data)) {

                appService.OpenBoard(data.FileName);
                OnMultiplePropertyChanged("Title", "Board");
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

                OnPropertyChanged("Title");
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

        public Board Board {
            get {
                return appService.Board;
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
