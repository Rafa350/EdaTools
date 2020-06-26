namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder {

    using System;
    using System.Text;

    /// <summary>
    /// Clase que representa un macro gerber.
    /// </summary>
    public class Macro {

        private readonly int _id;
        private readonly string _text;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="text">La sequencia de comandes del macro.</param>
        /// 
        public Macro(int id, string text) {

            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            _id = id;
            _text = text;
        }

        /// <summary>
        /// Retorna la comanda Gerber per la definicio del macro.
        /// </summary>
        /// <returns>La comanda.</returns>
        /// 
        protected virtual string GetCommand() {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("%AMM{0}*", _id);
            sb.Append(_text);
            if (!_text.EndsWith("%"))
                sb.Append('%');

            return sb.ToString();
        }

        /// <summary>
        /// Obte el ID del macro.
        /// </summary>
        /// 
        public int Id => _id;

        /// <summary>
        /// Obte la comanda Gerber per la definicio del macro.
        /// </summary>
        /// 
        public string Command => GetCommand();
    }
}
