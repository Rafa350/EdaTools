namespace MikroPic.NetMVVMToolkit.v1.ToolBars {

    using System.Windows;

    /// <summary>
    /// Interficie que cal que implementint les factories de barras d'eines.
    /// </summary>
    public interface IToolBarFactory {

        /// <summary>
        /// Crea una barra d'eines.
        /// </summary>
        /// <param name="fileName">Nom del firxer que conte el descriptor del la barra 
        /// d'eines</param>
        /// <returns>L'objecte que representa la barra d'eines.</returns>
        FrameworkElement CreateToolBar(string fileName, string toolbarName);
    }
}
