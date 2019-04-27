namespace MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel {

    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;

    /// <summary>
    /// Clase base pels objectes ViewModel.
    /// </summary>
    /// 
    public abstract class ViewModelBase: INotifyPropertyChanged {

        private ViewModelBase parent;
        private Window view;
        private bool initialized = false;
        private int loadCount = 0;
        private static bool notifing = false;

        private readonly bool throwOnInvalidPropertyName = true;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="parent">ViewModel pare.</param>
        /// 
        public ViewModelBase(ViewModelBase parent) {

            this.parent = parent;
        }

        /// <summary>
        /// Notifica que s'ha d'inicialitzar.
        /// </summary>
        /// 
        public void Initialize() {

            if (!initialized) {
                initialized = true;
                OnInitialize();
            }
        }

        /// <summary>
        /// Notifica que s'ha carregat la vista.
        /// </summary>
        /// 
        public void Load() {

            if (loadCount == 0) {
                OnLoad();
                Initialize();
            }
            loadCount++;
        }

        /// <summary>
        /// Notifica que s'ha descarregat la vista.
        /// </summary>
        /// 
        public void Unload() {

            if (loadCount > 0) {
                if (loadCount == 1) {
                    OnUnload();
                    //Deinitialize();
                }
                loadCount--;
            }
        }

        /// <summary>
        /// Asigna un valor a una propietat.
        /// </summary>
        /// <typeparam name="T">La tipus de propietat.</typeparam>
        /// <param name="storage">La propietat.</param>
        /// <param name="value">El valor a asignar.</param>
        /// <param name="propName">El nom de la propietat per notificar.</param>
        /// <returns>True si s'ha canviat el valor de la propietat.</returns>
        /// 
        public bool SetProperty<T>(ref T storage, T value, string propName) {
             
            if (object.Equals(storage, value))
                return false;

            else {
                storage = value;
                OnPropertyChanged(propName);
                return true;
            }
        }

        /// <summary>
        /// Notifica que s'ha d'inicialitzar.
        /// </summary>
        /// 
        protected virtual void OnInitialize() {

            // Per defecte no fa res
        }

        protected virtual void OnLoad() {

            // Per defecte no fa res
        }

        protected virtual void OnUnload() {

            // Per defecte no fa res
        }

        /// <summary>
        /// Notifica els canvis en diverses propietats del ViewModel.
        /// </summary>
        /// <param name="propNames">Llista de propietats que han canviat.</param>
        /// 
        protected void OnMultiplePropertyChanged(params string[] propNames) {

            OnMultiplePropertyChanged(false, true, propNames);
        }

        /// <summary>
        /// Notifica els canvis en diverses propietats del ViewModel.
        /// </summary>
        /// <param name="notifyParentChain">Indica si cal notificar als pares en cadena.</param>
        /// <param name="localProperties">Indica si son propietats locals.</param>
        /// <param name="propNames">Llista de propietats que han canviat.</param>
        /// 
        protected void OnMultiplePropertyChanged(bool notifyParentChain, bool localProperties, params string[] propNames) {

            if (propNames == null)
                throw new ArgumentNullException("propNames");

            foreach (string propName in propNames)
                OnPropertyChanged(notifyParentChain, localProperties, propName);
        }

        /// <summary>
        /// Notifica canvis en una propietat del ViewModel.
        /// </summary>
        /// <param name="propName">La propietat que ha canviat.</param>
        /// 
        protected void OnPropertyChanged(string propName) {

            OnPropertyChanged(false, true, propName);
        }

        /// <summary>
        /// Notifica canvis en una propietat del ViewModel.
        /// </summary>
        /// <param name="notifyParentChain">Indica si cal notificar als pares en cadena.</param>
        /// <param name="localProperty">Indica si les propietat es local.</param>
        /// <param name="propName">La propietat que ha canviat.</param>
        /// 
        protected void OnPropertyChanged(bool notifyParentChain, bool localProperty, string propName) {

            // Comprovas els parametres
            //
            if (String.IsNullOrEmpty(propName))
                throw new ArgumentNullException("propName");

            // Notifica del canvi al WPF
            //
            if ((PropertyChanged != null) && localProperty)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));

            if (notifyParentChain) {

                // No permet reentrada
                //
                if (notifing)
                    throw new InvalidOperationException(
                        String.Format("No es posible llamar a 'NotifyPropertyChange(\"{0}\")', dentro del proceso de notificación.", propName));

                notifing = true;
                try {
                    ViewModelBase vm = parent;
                    while (vm != null) {
                        vm.OnNotifyChange(this, propName);
                        vm = vm.parent;
                    }
                }
                finally {
                    notifing = false;
                }
            }
        }

        /// <summary>
        /// Reb la notifica d'un canvi en una propietat.
        /// </summary>
        /// <param name="viewModel">El ViewModel que ha generat la notificacio.</param>
        /// <param name="propName">La propietat que ha canviat.</param>
        /// 
        protected virtual void OnNotifyChange(ViewModelBase viewModel, string propName) {
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName) {

            VerifyPropertyName(this, propertyName);
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(object obj, string propertyName) {

            if (!String.IsNullOrEmpty(propertyName)) {
                if (obj.GetType().GetProperty(propertyName) == null) {
                    string msg = String.Format("Nombre de propiedad incorrecto: '{0}'.", propertyName);
                    if (throwOnInvalidPropertyName)
                        throw new ArgumentException(msg);
                    else
                        Debug.Fail(msg);
                }
            }
        }

        /// <summary>
        /// Indica si esta inicialitzat.
        /// </summary>
        /// 
        public bool IsInitialized {
            get {
                return initialized;
            }
        }

        /// <summary>
        /// Indica si s'ha carregat la.
        /// </summary>
        /// 
        public bool IsLoaded {
            get {
                return loadCount > 0;
            }
        }

        /// <summary>
        /// Obte el ViewModel pare.
        /// </summary>
        /// 
        public ViewModelBase Parent {
            get {
                return parent;
            }
        }

        /// <summary>
        ///  Obte la vista associada.
        /// </summary>
        /// 
        public Window View {
            get {
                if (view == null) {
                    foreach (Window window in Application.Current.Windows) {
                        if (ReferenceEquals(window.DataContext, this)) {
                            view = window;
                            break;
                        }
                    }
                }
                return view;
            }
        }
    }
}
