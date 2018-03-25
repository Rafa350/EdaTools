namespace MikroPic.NetMVVMToolkit.v1.MVVM.UndoRedo {

    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel;
    using MikroPic.NetMVVMToolkit.v1.MVVM.Commands;

    /// <summary>
    /// Clase que controla les operacions UNDO/REDO
    /// </summary>
    public sealed class UndoRedoManager {

        private ICommand undoCommand;
        private ICommand redoCommand;

        private readonly Stack<IOperation> undoStack = new Stack<IOperation>();
        private readonly Stack<IOperation> redoStack = new Stack<IOperation>();
        private static UndoRedoManager instance;

        /// <summary>
        /// Contructor de la clase. Es privat per implementar-ho com singleton.
        /// </summary>
        /// 
        private UndoRedoManager() {
        }

        /// <summary>
        /// Gestiona l'edicio d'una propietat.
        /// </summary>
        /// <typeparam name="T">Tipus de la propietat.</typeparam>
        /// <param name="newValue">El nou valor</param>
        /// <param name="oldValue">El antic valor</param>
        /// <param name="action">Accio a realitzar en APPLY i/o REVERT</param>
        /// 
        public void EditProperty<T>(T newValue, T oldValue, Action<T> action) {

            if (!newValue.Equals(oldValue)) {
                IOperation operation = new PropertyOperation<T>(newValue, oldValue, action);
                operation.Apply();
                undoStack.Push(operation);
            }
        }

        /// <summary>
        /// Gestiona l'edicio d'una coleccio.
        /// </summary>
        /// <typeparam name="T">El tipus d'item.</typeparam>
        /// <param name="item">El item</param>
        /// <param name="applyAction">Accio a realitzar en APPLY.</param>
        /// <param name="revertAction">Accio a realitzar en REVERT.</param>
        /// 
        public void EditCollection<T>(T item, Action<T> applyAction, Action<T> revertAction) {

            IOperation operation = new CollectionOperation<T>(item, applyAction, revertAction);
            operation.Apply();
            undoStack.Push(operation);
        }

        /// <summary>
        /// Neteja les piles UNDO/REDO.
        /// </summary>
        public void Clear() {

            undoStack.Clear();
            redoStack.Clear();
        }

        /// <summary>
        /// Realitza l'operacio UNDO.
        /// </summary>
        public void Undo() {

            if (undoStack.Count == 0)
                throw new InvalidOperationException("Undo stack overflow.");

            IOperation change = undoStack.Pop();
            redoStack.Push(change);
            change.Revert();
        }

        /// <summary>
        /// Realitza l'operacio REDO.
        /// </summary>
        /// 
        public void Redo() {

            if (redoStack.Count == 0)
                throw new InvalidOperationException("Redo stack overflow.");

            IOperation change = redoStack.Pop();
            undoStack.Push(change);
            change.Apply();
        }

        /// <summary>
        /// Obte el numero d'operacions UNDO disponibles.
        /// </summary>
        /// 
        public int UndoCount {
            get {
                return undoStack.Count;
            }
        }

        /// <summary>
        /// Obte el numero d'operacions REDO disposibles.
        /// </summary>
        /// 
        public int RedoCount {
            get {
                return redoStack.Count;
            }
        }

        /// <summary>
        /// Comprova si es por realitar l'operacio UNDO.
        /// </summary>
        public bool CanUndo {
            get {
                return undoStack.Count > 0;
            }
        }

        /// <summary>
        /// Comprova si es pot fer l'operacio REDO.
        /// </summary>
        /// 
        public bool CanRedo {
            get {
                return redoStack.Count > 0;
            }
        }

        /// <summary>
        /// Obte la comanda UNDO
        /// </summary>
        public ICommand UndoCommand {
            get {
                if (undoCommand == null) 
                    undoCommand = new RelayCommand(
                        x => Undo(),
                        x => CanUndo);
                
                return undoCommand;
            }
        }

        /// <summary>
        /// Obte la comanda REDO.
        /// </summary>
        public ICommand RedoCommand {
            get {
                if (redoCommand == null) 
                    redoCommand = new RelayCommand(
                        x => Redo(),
                        x => CanRedo);
                
                return redoCommand;
            }
        }

        /// <summary>
        /// Obte una instancia unica (Singleton) de la clase.
        /// </summary>
        /// 
        public static UndoRedoManager Instance {
            get {
                if (instance == null)
                    instance = new UndoRedoManager();
                return instance;
            }
        }
    }
}
