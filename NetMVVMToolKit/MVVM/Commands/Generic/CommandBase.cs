namespace MikroPic.NetMVVMToolkit.v1.MVVM.Commands.Generic {

    /// <summary>
    /// Clase base per les comandes.
    /// </summary>
    public abstract class CommandBase<T>: CommandBase {

        /// <summary>
        /// Executa la comanda.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        public override void Execute(object parameter) {

            Execute((T) parameter);
        }

        /// <summary>
        /// Comprova si pot executar la comanda.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        /// <returns>True si es pot executar. False en cas contrari.</returns>
        public override bool CanExecute(object parameter) {

            return CanExecute((T) parameter);
        }

        /// <summary>
        /// Executa la comanda.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        public virtual void Execute(T parameter) {
        }

        /// <summary>
        /// Comprova si pot executar la comanda.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        /// <returns>True si es pot executar. False en cas contrari.</returns>
        public virtual bool CanExecute(T parameter) {

            return true;
        }
    }
}
