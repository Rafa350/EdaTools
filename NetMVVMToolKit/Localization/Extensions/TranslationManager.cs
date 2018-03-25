namespace Media.NetGui.v1.Localization.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Media.NetGui.v1.Localization.Providers;

    /// <summary>
    /// Clase que gestiona la traduccio de textes.
    /// </summary>
    public sealed class TranslationManager {

        private static TranslationManager instance;
        private ITranslationProvider translationProvider;
        public event EventHandler LanguageChanged;

        /// <summary>
        /// Constructor per defecte de la clase.
        /// </summary>
        private TranslationManager() {
        }

        /// <summary>
        /// Notifica canvis en el llenguatge, i dispare l'event LanguageChanged'
        /// </summary>
        private void OnLanguageChanged() {

            if (LanguageChanged != null) 
                LanguageChanged(this, EventArgs.Empty);            
        }

        /// <summary>
        /// Tradueix una cadena.
        /// </summary>
        /// <param name="key">Cadena a traduir.</param>
        /// <returns>La cadena traduida.</returns>
        public object Translate(string key) {

            if (translationProvider != null) {
                object translatedValue = translationProvider.Translate(key);
                if (translatedValue != null) 
                    return translatedValue;
            }
            return String.Format("<{0}>", key);
        }

        /// <summary>
        /// Obte o canvia el llenguatge actual.
        /// </summary>
        public CultureInfo CurrentLanguage {
            get { 
                return Thread.CurrentThread.CurrentUICulture;
            }
            set {
                if (value != Thread.CurrentThread.CurrentUICulture) {
                    Thread.CurrentThread.CurrentUICulture = value;
                    OnLanguageChanged();
                }
            }
        }

        /// <summary>
        /// Enumera els llenguatges disponibles.
        /// </summary>
        public IEnumerable<CultureInfo> Languages {
            get {
                if (translationProvider != null) 
                    return translationProvider.Languages;
                else
                    return Enumerable.Empty<CultureInfo>();
            }
        }

        /// <summary>
        /// Obte o asigna el proveidor de traduccions.
        /// </summary>
        public ITranslationProvider TranslationProvider {
            get {
                return translationProvider;
            }
            set {
                translationProvider = value;
            }
        }

        /// <summary>
        /// Crea una instancia de la clase (Singleton).
        /// </summary>
        public static TranslationManager Instance {
            get {
                if (instance == null)
                    instance = new TranslationManager();
                return instance;
            }
        }
    }
}