namespace MikroPic.NetMVVMToolkit.v1.MVVM.Services {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using MikroPic.NetMVVMToolkit.v1.MVVM.View;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;
    using Microsoft.Win32;

    /// <summary>
    /// Gestiona la presentacio de cuadres de dialeg.
    /// </summary>
    /// 
    public sealed class DialogService: IDialogService {

        private static DialogService instance;
        private Dictionary<Type, Type> register;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// 
        private DialogService() {

            register = new Dictionary<Type, Type>();
        }

        /// <summary>
        /// Associa un ViewModel amb un View.
        /// </summary>
        /// <param name="viewModelType">El tipus de ViewModel.</param>
        /// <param name="viewType">El tipus de View.</param>
        /// 
        public void Register(Type viewModelType, Type viewType) {

            if (viewModelType == null)
                throw new ArgumentNullException("viewModelType");

            if (viewType == null)
                throw new ArgumentNullException("viewType");

            if (register.ContainsKey(viewModelType))
                throw new InvalidOperationException("Ya se registro el tipo de ViewModel.");

            register.Add(viewModelType, viewType);
        }

        /// <summary>
        /// Mostra ei quadre de dialeg estandar de notificacio.
        /// </summary>
        /// <param name="title">Titol.</param>
        /// <param name="text">Text a mostrar.</param>
        public void ShowNotification(string title, string text) {

            if (String.IsNullOrEmpty(title))
                throw new ArgumentNullException("title");

            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Mostra el quadre de dialeg estandard de confirmacio.
        /// </summary>
        /// <param name="title">Titol.</param>
        /// <param name="text">Text a mostrar.</param>
        /// <returns>True si es respon 'Ok', false en cas contrari.</returns>
        /// 
        public bool ShowAskOkCancel(string title, string text) {

            if (String.IsNullOrEmpty(title))
                throw new ArgumentNullException("title");

            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            return MessageBox.Show(text, title,
                MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
        }

        public bool ShowAskYesNo(string title, string text) {

            if (String.IsNullOrEmpty(title))
                throw new ArgumentNullException("title");

            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            return MessageBox.Show(text, title,
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public bool ShowOpenFileDialog(IOpenFileDialogViewModel viewModel) {

            if (viewModel == null)
                throw new ArgumentNullException("viewModel");

            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Title = viewModel.Title;
            dlg.Filter = viewModel.Filter;
            dlg.FilterIndex = viewModel.FilterIndex;
            dlg.InitialDirectory = viewModel.InitialDirectory;
            dlg.DefaultExt = viewModel.DefaultExt;
            dlg.FileName = viewModel.FileName;

            dlg.ReadOnlyChecked = viewModel.ReadOnlyChecked;
            dlg.ShowReadOnly = viewModel.ShowReadOnly;
            dlg.Multiselect = viewModel.MultiSelect;

            if (dlg.ShowDialog() == true) {

                viewModel.FileName = dlg.FileName;
                viewModel.FileNames = dlg.FileNames;
                
                return true;
            }
            else
                return false;
        }

        public bool ShowSaveFileDialog(ISaveFileDialogViewModel viewModel) {

            if (viewModel == null)
                throw new ArgumentNullException("viewModel");

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Title = viewModel.Title;
            dlg.Filter = viewModel.Filter;
            dlg.FilterIndex = viewModel.FilterIndex;
            dlg.InitialDirectory = viewModel.InitialDirectory;
            dlg.DefaultExt = viewModel.DefaultExt;
            dlg.FileName = viewModel.FileName;

            if (dlg.ShowDialog() == true) {
                
                viewModel.FileName = dlg.FileName;

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Presenta ua finestra modal.
        /// </summary>
        /// <param name="viewModel">El ViewModel</param>
        /// <returns>El resultat del tancament de la finestra.</returns>
        public bool ShowDialog(ViewModelBase viewModel) {

            if (viewModel == null)
                throw new ArgumentNullException("viewModel");

            Type viewType = register[viewModel.GetType()];
            Window window;

            // Si es un control d'usuari, crea una finestra que el contingui
            //
            if (typeof(UserControl).IsAssignableFrom(viewType)) {
                window = new DialogView();
                window.Content = (Control) Activator.CreateInstance(viewType);
            }

            // En cas contrari, si es una finestra normal, la crea.
            //
            else
                window = (Window) Activator.CreateInstance(viewType);

            // Asigna la finestra propietaria, i el seu icon
            //
            if (viewModel.Parent != null) {
                Window owner = viewModel.Parent.View;
                if (owner != null) {
                    window.Owner = owner;
                    window.Icon = owner.Icon;
                }
            }
            window.DataContext = viewModel;

            // Mostra la finesta en modus modal
            //
            if (window.ShowDialog() == true) {
             
                return true;
            }
            else
                return true;
        }

        /// <summary>
        /// Obte una instancia de la clase (Singleton)
        /// </summary>
        public static IDialogService Instance {
            get {
                if (instance == null)
                    instance = new DialogService();
                return instance;
            }
        }
    }
}
