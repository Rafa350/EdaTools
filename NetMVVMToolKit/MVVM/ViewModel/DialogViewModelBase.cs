namespace MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel {

    using System.Windows.Input;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Commands;

    /// <summary>
    /// Objecte ViewModel especifit per quadres de dialeg. Gestiona les
    /// comandes Accept, Apply i Cancell.
    /// </summary>
    public abstract class DialogViewModelBase: WindowViewModelBase {

        private string title;
        private ICommand acceptCommand;
        private ICommand cancelCommand;
        private ICommand applyCommand;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="parent">ViewModel pare.</param>
        public DialogViewModelBase(ViewModelBase parent)
            : base(parent) {
        }

        protected virtual void AcceptExecute() {

            CloseView();
        }

        protected virtual bool AcceptCanExecute() {

            return true;
        }

        protected virtual void CancelExecute() {

            CloseView();
        }

        protected virtual bool CancelCanExecute() {

            return true;
        }

        protected virtual void ApplyExecute() {
        }

        protected virtual bool ApplyCanExecute() {

            return true;
        }

        public string Title {
            get {
                return title;
            }
            set {
                if (title != value) {
                    title = value;
                    NotifyPropertyChange("Title");
                }
            }
        }

        /// <summary>
        /// Comanda per la accio 'Accept'.
        /// </summary>
        public ICommand AcceptCommand {
            get {
                if (acceptCommand == null)
                    acceptCommand = new RelayCommand(
                        x => AcceptExecute(),
                        x => AcceptCanExecute());
                return acceptCommand;
            }
        }

        /// <summary>
        /// Comanda per l'accio 'Cancel'.
        /// </summary>
        public ICommand CancelCommand {
            get {
                if (cancelCommand == null)
                    cancelCommand = new RelayCommand(
                        x => CancelExecute(),
                        x => CancelCanExecute());
                return cancelCommand;
            }
        }

        /// <summary>
        /// Comanda per l'accio 'Apply'.
        /// </summary>
        public ICommand ApplyCommand {
            get {
                if (applyCommand == null)
                    applyCommand = new RelayCommand(
                        x => ApplyExecute(),
                        x => ApplyCanExecute());
                return applyCommand;
            }
        }
    }
}