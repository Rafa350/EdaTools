namespace MikroPic.NetMVVMToolkit.v1.MVVM.Commands.Generic {

    using System;

    /// <summary>
    /// Comanda gestionada per delegats. El parametre opcional es de tipus generic.
    /// </summary>
    /// <typeparam name="T">Tipus del parametre opcional.</typeparam>
    public class RelayCommand<T>: RelayCommand {

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="execute">Delegat per la funcio Execute.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null) {
        }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="execute">Delegat per la funcio Execute.</param>
        /// <param name="canExecute">Delegar per la funcio CanExecute.</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute):
            base(o => execute((T) o), o => canExecute((T) o)) {
        }

        /// <summary>
        /// Executa la comanda. S'ha d'implementar en les clases derivades.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        public virtual void Execute(T parameter) {

            base.Execute(parameter);
        }

        /// <summary>
        /// Comprova si es pot executar la comanda. S'ha d'implementar en les clases derivades.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        /// <returns>True si es pot executar. False en cas contrari.</returns>
        public virtual bool CanExecute(T parameter) {

            return base.CanExecute(parameter);
        }
    }
}
