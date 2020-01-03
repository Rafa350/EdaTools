namespace MikroPic.NetMVVMToolkit.v1.MVVM.UndoRedo {

    using System;

    internal sealed class CollectionOperation<T>: IOperation {

        private readonly T item;
        private readonly Action<T> applyAction;
        private readonly Action<T> revertAction;

        public CollectionOperation(T item, Action<T> applyAction, Action<T> revertAction) {

            if (applyAction == null)
                throw new ArgumentNullException("applyAction");

            if (revertAction == null)
                throw new ArgumentNullException("revertAction");

            this.item = item;
            this.applyAction = applyAction;
            this.revertAction = revertAction;
        }

        public void Apply() {

            applyAction(item);
        }

        public void Revert() {

            revertAction(item);
        }
    }
}
