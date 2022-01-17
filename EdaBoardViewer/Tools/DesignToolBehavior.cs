namespace EdaBoardViewer.Tools {

    using Avalonia.Controls;
    using Avalonia.Input;
    using System;

    public sealed class DesignToolBehavior<TControl> where TControl : Control {

        private readonly TControl owner;

        public DesignToolBehavior(TControl owner) {

            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public void Attach() {

            owner.PointerPressed += Owner_PointerPressed;
            owner.PointerReleased += Owner_PointerReleased;
            owner.PointerMoved += Owner_PointerMoved;
        }

        public void Detach() {

            owner.PointerPressed -= Owner_PointerPressed;
            owner.PointerReleased -= Owner_PointerReleased;
            owner.PointerMoved -= Owner_PointerMoved;
        }

        private void Owner_PointerMoved(object sender, PointerEventArgs e) {
        }

        private void Owner_PointerReleased(object sender, PointerReleasedEventArgs e) {
        }

        private void Owner_PointerPressed(object sender, PointerPressedEventArgs e) {
        }
    }
}
