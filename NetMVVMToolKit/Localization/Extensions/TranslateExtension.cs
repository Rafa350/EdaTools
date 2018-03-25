namespace Media.NetGui.v1.Localization.Extensions {

    using System;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// Extensio XAML per la traduccio de textes.
    /// </summary>
    public sealed class TranslateExtension: MarkupExtension {

        private string key;

        /// <summary>
        /// Constructor per defecte de la classe.
        /// </summary>
        public TranslateExtension() {
        }

        /// <summary>
        /// Constructor de la classe.
        /// </summary>
        /// <param name="key">La cadena a traduir.</param>
        public TranslateExtension(string key) {

            this.key = key;
        }

        /// <summary>
        /// Obte el valor de la cadena traduida.
        /// </summary>
        /// <param name="serviceProvider">Proveidor de serveis.</param>
        /// <returns>La cadena traduida.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider) {

            Binding binding = new Binding("Value");
            binding.Source = new TranslationData(key);
            return binding.ProvideValue(serviceProvider);
        }

        /// <summary>
        /// Asigna el valor de la cadena a traduir.
        /// </summary>
        [ConstructorArgument("key")]
        public string Key {
            get {
                return key;
            }
            set {
                key = value;
            }
        }
    }
}
