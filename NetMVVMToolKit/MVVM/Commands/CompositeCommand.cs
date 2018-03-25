namespace MikroPic.NetMVVMToolkit.v1.MVVM.Commands {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    
    /// <summary>
    /// Implementa una comanda formada per una llista de comandes.
    /// </summary>
    public class CompositeCommand: CommandBase {

        private readonly List<ICommand> registeredCommands = new List<ICommand>();

        /// <summary>
        /// Afegeix una comanda a la llista.
        /// </summary>
        /// <param name="command">La comanda a afeigir.</param>
        public void RegisterCommand(ICommand command) {

            if (command == null)
                throw new ArgumentNullException("command");

            if (command == this)
                throw new InvalidOperationException("No es posible registrarse a si mismo.");

            lock (registeredCommands) {
                if (registeredCommands.Contains(command))
                    throw new InvalidOperationException("El comando ya ha sido registrado.");

                registeredCommands.Add(command);
            }
        }

        /// <summary>
        /// Retira una comanda de la llista.
        /// </summary>
        /// <param name="command">La comanda a retirar.</param>
        public void UnregisterCommand(ICommand command) {

            if (command == null)
                throw new ArgumentNullException("command");

            lock (registeredCommands) {
                if (!registeredCommands.Contains(command))
                    throw new InvalidOperationException("El comando no esta registrado.");

                registeredCommands.Remove(command);
            }
        }

        /// <summary>
        /// Executa la comanda.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        public override void Execute(object parameter) {
        
            Queue<ICommand> commands;
            lock (registeredCommands) {
                commands = new Queue<ICommand>(this.registeredCommands.Where(ShouldExecute).ToList());
            }

            while (commands.Count > 0) {
                ICommand command = commands.Dequeue();
                command.Execute(parameter);
            }
        }

        /// <summary>
        /// Comprova si es pot executar la comanda.
        /// </summary>
        /// <param name="parameter">Parametre opcional.</param>
        /// <returns></returns>
        public override bool CanExecute(object parameter) {

            bool hasEnabledCommandsThatShouldBeExecuted = false;

            ICommand[] commandList;
            
            lock (registeredCommands) {
                commandList = registeredCommands.ToArray();
            }

            foreach (ICommand command in commandList) {
                if (ShouldExecute(command)) {
                    if (!command.CanExecute(parameter)) 
                        return false;
                    
                    hasEnabledCommandsThatShouldBeExecuted = true;
                }
            }

            return hasEnabledCommandsThatShouldBeExecuted;
        }

        protected virtual bool ShouldExecute(ICommand command) {

            return true;
        }
    }
}
