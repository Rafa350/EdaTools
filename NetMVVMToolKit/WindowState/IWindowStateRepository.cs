namespace MikroPic.NetMVVMToolkit.v1.WindowState {

    using System.Windows;

    /// <summary>
    /// Interficie que cal que implementin els repositoris d'estat.
    /// </summary>
    public interface IWindowStateRepository {

        void Load();

        void Save();

        void Set(Window window, WindowStateInfo info);

        WindowStateInfo Get(Window window);
    }
}
