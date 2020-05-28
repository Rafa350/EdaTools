namespace MikroPic.NetMVVMToolkit.v1.WindowState {

    using System;
    using System.Windows;

    public sealed class LayoutInfo {

        private Rect bounds = Rect.Empty;
        private WindowState state = WindowState.Normal;

        public void SetTo(Window window) {

            if (window == null)
                throw new ArgumentNullException("window");

            window.WindowState = state;
            window.Left = bounds.Left;
            window.Top = bounds.Top;
            window.Width = bounds.Width;
            window.Height = bounds.Height;
        }

        public void GetFrom(Window window) {

            if (window == null)
                throw new ArgumentNullException("window");

            state = window.WindowState;
            bounds = window.RestoreBounds;
        }

        public Rect Bounds {
            get {
                return bounds;
            }
            set {
                bounds = value;
            }
        }

        public WindowState State {
            get {
                return state;
            }
            set {
                state = value;
            }
        }
    }
}
