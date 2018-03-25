namespace MikroPic.NetMVVMToolkit.v1.WindowState {

    using System;
    using System.Windows;
    using MikroPic.NetMVVMToolkit.v1.WindowState.Repository;

    /// <summary>
    /// Gestiona l'estat de les finestres. Aquesta clase s'implementa com un singleton.
    /// </summary>
    public sealed class WindowStateManager {
        
        private static WindowStateManager instance;
        private IWindowStateRepository repository;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// 
        private WindowStateManager() {
        }

        /// <summary>
        /// Tanca i finalitza les operacions.
        /// </summary>
        /// 
        public void Close() {

            if (repository != null)
                repository.Save();
        }

        /// <summary>
        /// Obte la informacio d'estat d'una finestra.
        /// </summary>
        /// <param name="window">La finestra.</param>
        /// <returns>Informacio d'estat de la finestra..</returns>
        /// 
        public WindowStateInfo GetInfo(Window window) {

            if (window == null)
                throw new ArgumentNullException("window");

            if (repository == null) {
                repository = new WindowStateRepository();
                repository.Load();
            }

            return repository.Get(window);
        }

        /// <summary>
        /// Obte una instancia d'aquesta clase.
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
