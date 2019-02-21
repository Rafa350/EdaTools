namespace MikroPic.EdaTools.v1.PanelEditor.DrawEditor {

    /// <summary>
    /// Interficie pels conteridors visuals
    /// </summary>
    /// 
    public interface IVisualContainer {

        /// <summary>
        /// Afegeix un item al contenidor.
        /// </summary>
        /// <param name="visual">L'item a afeigit.</param>
        /// 
        void AddVisualItem(VisualItem visual);

        /// <summary>
        /// Elimina un item del contenidor.
        /// </summary>
        /// <param name="visual">L'item a eliminar.</param>
        /// 
        void RemoveVisualItem(VisualItem visual);
    }
}
