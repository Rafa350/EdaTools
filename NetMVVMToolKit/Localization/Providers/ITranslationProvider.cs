namespace Media.NetGui.v1.Localization.Providers {

    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Interface pels proveidors de traduccio.
    /// </summary>
    public interface ITranslationProvider {

        /// <summary>
        /// Tradueix una text.
        /// </summary>
        /// <param name="key">El text a traduir.</param>
        /// <returns>El text traduit.</returns>
        object Translate(string key);

        /// <summary>
        /// Obte els llenguatges disponibles.
        /// </summary>
        /// <value>un enumerador amb la llista de llanguatges.</value>
        IEnumerable<CultureInfo> Languages { get; }
    }
}