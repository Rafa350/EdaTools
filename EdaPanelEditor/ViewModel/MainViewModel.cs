﻿namespace MikroPic.EdaTools.v1.PanelEditor.ViewModel {

    using System;
    using System.Windows.Input;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.PanelEditor.Services;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Commands;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Services;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;

    public sealed class MainViewModel: ViewModelBase {

        private const string title = "EdaTools Panel Editor";

        private ICommand newCommand;
        private ICommand openCommand;
        private ICommand saveCommand;
        private ICommand saveAsCommand;
        private ICommand exitCommand;

        private PanelEditorViewModel panelEditorViewModel;
        private PanelStructureViewModel panelStructureViewModel;

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

            appService.NewProject();

            panelEditorViewModel = null;
            panelStructureViewModel = null;

            NotifyPropertyChanges("Title", "PanelEditorViewModel", "PanelStructureViewModel");
        }

        /// <summary>
        /// Executa la comanda 'OpenCommand'
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        /// 
        private void OpenExecute(object parameter) {

            OpenFileDialogViewModel data = new OpenFileDialogViewModel(null);
            data.Title = "Open project";
            data.MultiSelect = false;
            data.CheckFileExist = true;
            data.Filter = "Panel project (*.xpnl)|*.xpnl";
            data.FilterIndex = 0;
            data.DefaultExt = ".xpnl";
            data.AddExtension = true;

            if (dlgService.ShowOpenFileDialog(data)) {

                appService.OpenProject(data.FileName);

                panelEditorViewModel = null;
                panelStructureViewModel = null;

                NotifyPropertyChanges("Title", "PanelEditorViewModel", "PanelStructureViewModel");
            }
        }

        /// <summary>
        /// Executa la comanda 'SaveCommand'
        /// </summary>
        /// <param name="param">Parametre opcional</param>
        /// 
        private void SaveExecute(object param) {

            appService.SaveProject();
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
                appService.SaveAsProject(data.FileName);

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

        public PanelEditorViewModel PanelEditorViewModel {
            get {
                if (panelEditorViewModel == null)
                    panelEditorViewModel = new PanelEditorViewModel(this);
                return panelEditorViewModel;
            }
        }

        public PanelStructureViewModel PanelStructureViewModel {
            get {
                if (panelStructureViewModel == null)
                    panelStructureViewModel = new PanelStructureViewModel(this);
                return panelStructureViewModel;
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
