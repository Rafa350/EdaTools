namespace MikroPic.NetMVVMToolkit.v1.WindowState {

    /// <summary>
    /// Mante l'estat d'una finestra
    /// </summary>
    public sealed class WindowStateInfo {

        private readonly LayoutInfo layout = new LayoutInfo();
        private readonly DataInfo data = new DataInfo();

        public DataInfo Data {
            get {
                return data;
            }
        }

        public LayoutInfo Layout {
            get {
                return layout;
            }
        }    
    }
}
