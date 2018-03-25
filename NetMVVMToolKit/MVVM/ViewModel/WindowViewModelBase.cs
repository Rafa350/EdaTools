namespace MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel {

    using System.Windows;
    using System.Windows.Input;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Commands;

    /// <summary>
    /// Objecte ViewModel especific per objectes Window. Gestions la
    /// comanda 'Close'.
    /// </summary>
    public abstract class WindowViewModelBase: ViewModelBase {

        private ICommand closeCommand;

        public WindowViewModelBase(ViewModelBase parent)
            : base(parent) {
        }

        /// <summary>
        /// Executa de la comanda Close.
        /// </summary>
        protected virtual void CloseExecute() {

            if (CanCloseView())
                CloseView();
        }

        /// <summary>
        /// Comprova si es pot executar la comanda Close.
        /// </summary>
        /// <returns>True si es posible executar la comanda.</returns>
        protected virtual bool CloseCanExecute() {

            return true;
        }
        
        /// <summary>
        /// Tanca la vista associada.
        /// </summary>
        protected virtual void CloseView() {

            Window view = View;
            if (view != null)
                view.Close();
        }

        /// <summary>
        /// Comprova si pot tancar la vista associada.
        /// </summary>
        /// <returns>True si la pot tancar. False en cas contrari.</returns>
        protected virtual bool CanCloseView() {

            return true;
        }

        /// <summary>
        /// Comanda Close.
        /// </summary>
        public ICommand CloseCommand {
            get {
                if (closeCommand == null)
                    closeCommand = new RelayCommand(
                        x => CloseExecute(),
                        x => CloseCanExecute());
                return closeCommand;
            }
        }
    }
}