namespace MikroPic.NetMVVMToolkit.v1.MVVM.Commands {

    using MikroPic.NetMVVMToolkit.v1.MVVM.Infrastructure;
    using System;

    /// <summary>
    /// Comanda gestionada per delegats. El parametre opcional es de tipus generic.
    /// </summary>
    /// <typeparam name="T">Tipus del parametre opcional.</typeparam>
    public class RelayCommand: CommandBase {

        private readonly WeakAction<object> execute;
        private readonly WeakFunc<object, bool> canExecute;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="execute">Delegat per la funcio Execute.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null) {
        }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="execute">Delegat per la funcio Execute.</param>
        /// <param name="canExecute">Delegar per la funcio CanExecute.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute) {

            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = new WeakAction<object>(execute);
            if (canExecute != null)
                this.canExecute = new WeakFunc<object,bool>(canExecute);
        }

        /// <summary>
        /// Executa la comanda. S'ha d'implementar en les clases derivades.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        public override void Execute(object parameter) {

            execute.Execute(parameter);
        }

        /// <summary>
        /// Comprova si es pot executar la comanda. S'ha d'implementar en les clases derivades.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        /// <returns>True si es pot executar. False en cas contrari.</returns>
        public override bool CanExecute(object parameter) {

            return canExecute == null ? true : canExecute.Execute(parameter);
        }
    }
}
