namespace MikroPic.NetMVVMToolkit.v1.Menus {

    using System.Windows;

    /// <summary>
    /// Interficie que cal que implementin les factories de menus.
    /// </summary>
    public interface IMenuFactory {

        /// <summary>
        /// Crea un menu.
        /// </summary>
        /// <param name="fileName">Nom del fitxer descriptor del menu.</param>
        /// <returns>Objecte que representa el menu.</returns>
        FrameworkElement CreateMenu(string fileName, string menuName);
    }
}
