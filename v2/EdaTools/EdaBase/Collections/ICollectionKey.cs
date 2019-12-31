namespace MikroPic.EdaTools.v1.Collections {

    /// <summary>
    /// Interficie que han de implementar els elements de les coleccions 'KeyCollection'
    /// </summary>
    /// <typeparam name="TKey">El tipus de la clau.</typeparam>
    /// 
    public interface ICollectionKey<TKey> { 

        /// <summary>
        /// Obte el valor de la clau a utilitzar en la col·leccio.
        /// </summary>
        /// <returns>El valor de la clau</returns>
        /// 
        TKey GetKey();
    }
}
