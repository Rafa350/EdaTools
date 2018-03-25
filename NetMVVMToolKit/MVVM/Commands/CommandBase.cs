namespace MikroPic.NetMVVMToolkit.v1.MVVM.Commands {

    using System;
    using System.Windows.Input;

    /// <summary>
    /// Clase base per les comandes.
    /// </summary>
    public abstract class CommandBase: ICommand {

        /// <summary>
        /// Executa la comanda.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        public virtual void Execute(object parameter) {
        }

        /// <summary>
        /// Comprova si pot executar la comanda.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        /// <returns>True si es pot executar. False en cas contrari.</returns>
        public virtual bool CanExecute(object parameter) {

            return true;
        }

        /// <summary>
        /// Event per notificar els canvis.
        /// </summary>
        public event EventHandler CanExecuteChanged {
            add {
                CommandManager.RequerySuggested += value;
            }
            remove {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
