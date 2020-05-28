namespace MikroPic.NetMVVMToolkit.v1.WindowState {

    using System;
    using System.ComponentModel;
    using System.Windows;
    using MikroPic.NetMVVMToolkit.v1.WindowState.Repository;

    /// <summary>
    /// Gestiona l'estat de les finestres. Aquesta clase s'implementa com un singleton.
    /// </summary>
    public sealed class WindowStateManager {

        private static WindowStateManager instance;

        private readonly CancelEventHandler cancelEventHandler;
        private IWindowStateRepository repository;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// 
        private WindowStateManager() {

            cancelEventHandler = new CancelEventHandler(WindowClosing);
        }

        /// <summary>
        /// Registra la finestra a gestionar.
        /// </summary>
        /// <param name="window">La finestra.</param>
        /// 
        private void Register(Window window) {

            // Asigna els events a la finestra
            //
            window.Closing += cancelEventHandler;

            // Carrega el repositori si encara no s'ha fet
            //
            if (repository == null) {
                repository = new WindowStateRepository();
                repository.Load();
            }

            // Inicialitza la finestra amb la informacio del repositori.
            //
            WindowStateInfo info = repository.Get(window);
            if (!info.Layout.Bounds.IsEmpty)
                info.Layout.SetTo(window);
        }

        /// <summary>
        /// Captura el event 'Closing' d'una finestra.
        /// </summary>
        /// <param name="sender">La finestra que envia el event.</param>
        /// <param name="e">Arguments del event.</param>
        /// 
        private void WindowClosing(object sender, CancelEventArgs e) {

            Window window = sender as Window;
            if (window != null) {

                // Desasigna els events de la finestra
                //
                window.Closing -= cancelEventHandler;

                // Actualitza la informacio del repository.
                //
                WindowStateInfo info = repository.Get(window);
                info.Layout.GetFrom(window);

                // Si es la finestra principal la que es tanca,
                // aleshores salva el repositori.
                //
                if (window == Application.Current.MainWindow)
                    repository.Save();
            }
        }

        /// <summary>
        /// Registra la finestra a gestionar.
        /// </summary>
        /// <param name="window">La finestra.</param>
        /// 
        public static void RegisterWindow(Window window) {

            if (window == null)
                throw new ArgumentNullException("window");

            Instance.Register(window);
        }

        /// <summary>
        /// Obte una instancia unica d'aquesta clase.
        /// </summary>
        /// 
        public static WindowStateManager Instance {
            get {
                if (instance == null)
                    instance = new WindowStateManager();
                return instance;
            }
        }
    }
}
