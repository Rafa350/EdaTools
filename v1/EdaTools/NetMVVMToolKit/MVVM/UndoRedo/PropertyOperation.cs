namespace MikroPic.NetMVVMToolkit.v1.MVVM.UndoRedo {

    using System;
    
    /// <summary>
    /// Operacio UNDO/REDO aplicable a propietats.
    /// </summary>
    /// <typeparam name="T">Tipus de la propietat.</typeparam>
    internal sealed class PropertyOperation<T>: IOperation {

        private readonly T newValue;
        private readonly T oldValue;
        private readonly Action<T> action;

        /// <summary>
        /// Contructor de la clase.
        /// </summary>
        /// <param name="newValue">El nou valor.</param>
        /// <param name="oldValue">El antic valor.</param>
        /// <param name="action">Accio a realitzar en APPLY/REVERT.</param>
        /// 
        public PropertyOperation(T newValue, T oldValue, Action<T> action) {

            if (action == null)
                throw new ArgumentNullException("action");

            this.newValue = newValue;
            this.oldValue = oldValue;
            this.action = action;
        }

        /// <summary>
        /// Aplica el canvi.
        /// </summary>
        /// 
        public void Apply() {

            action(newValue);
        }

        /// <summary>
        /// Restaurar l'estat anterior al canvi.
        /// </summary>
        /// 
        public void Revert() {

            action(oldValue);
        }
    }
}
