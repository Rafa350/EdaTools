namespace MikroPic.NetMVVMToolkit.v1.MVVM.UndoRedo {
    
    /// <summary>
    /// Interficie per les operacions UNDO/REDO.
    /// </summary>
    internal interface IOperation {

        /// <summary>
        /// Aplica l'operacio.
        /// </summary>
        /// 
        void Apply();

        /// <summary>
        /// Reverteix l'operacio.
        /// </summary>
        /// 
        void Revert();
    }
}
