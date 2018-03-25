namespace MikroPic.NetMVVMToolkit.v1.WindowState {

    using System;
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Gestiona l'estat d'una finestra. Permet enmagatzemar la posicio i l'estat
    /// de la finestra, aixi com dades adicionals.
    /// </summary>
    public sealed class WindowStateAgent: IDisposable {

        private readonly Window window;
        private readonly CancelEventHandler cancelEventHandler;
        private WindowStateInfo info;

        /// <summary>
        /// Contructor de la clase.
        /// </summary>
        /// <param name="window">Finestra a gestionar.</param>
        /// 
        public WindowStateAgent(Window window) {

            if (window == null)
                throw new ArgumentNullException("window");

            cancelEventHandler = new CancelEventHandler(WindowClosing);

            this.window = window;
            this.window.Closing += cancelEventHandler;

            // Recupera l'estat
            //
            info = WindowStateManager.Instance.GetInfo(window);
            if (!info.Layout.Bounds.IsEmpty) 
                info.Layout.SetTo(window);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        /// 
        ~WindowStateAgent() {

            Dispose(false);
        }

        /// <summary>
        /// Implementacio de IDisposable.
        /// </summary>
        /// 
        public void Dispose() {

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementacio de IDisposable.
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        private void Dispose(bool disposing) {

            if (disposing)
                window.Closing -= cancelEventHandler;
        }

        /// <summary>
        /// Procesa l'event de tancament de la finestra. Salva la posicio ,
        /// l'estat i les dades adicionals.
        /// </summary>
        /// <param name="sender">El objecte que envia l'event.</param>
        /// <param name="e">Parametres del event.</param>
        /// 
        private void WindowClosing(object sender, CancelEventArgs e) {

            info.Layout.GetFrom(window);

            // Si es la finestra principal de l'aplicacio, salva el repositori
            ///
            if (Application.Current.MainWindow == window)
                WindowStateManager.Instance.Close();
        }

        /// <summary>
        /// Afegeix dades adicionals.
        /// </summary>
        /// <param name="name">El nom de les dades.</param>
        /// <param name="value">Les dades.</param>
        /// 
        public void SetData(string name, object value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            info.Data[name] = value;
        }

        /// <summary>
        /// Recupera dades adicionals.
        /// </summary>
        /// <param name="name">El nom de les dades.</param>
        /// <returns>Les dades recuperades. Null si no les troba.</returns>
        /// 
        public object GetData(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return info.Data[name];
        }
    }
}
