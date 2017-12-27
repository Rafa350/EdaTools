namespace MikroPic.EdaTools.v1.Cam.Gerber.Builder {

    using System;
    using System.Text;

    public class Macro {

        private readonly int id;
        private readonly string text;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="text">La sequencia de comandes del macro.</param>
        /// 
        public Macro(int id, string text) {

            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            this.id = id;
            this.text = text;
        }

        /// <summary>
        /// Retorna la comanda Gerber per la definicio del macro.
        /// </summary>
        /// <returns>La comanda.</returns>
        /// 
        protected virtual string GetCommand() {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("%AMM{0}*", id);
            sb.Append(text);
            if (!text.EndsWith("%"))
                sb.Append('%');

            return sb.ToString();
        }

        /// <summary>
        /// Obte el ID del macro.
        /// </summary>
        /// 
        public int Id {
            get {
                return id;
            }
        }

        /// <summary>
        /// Obte la comanda Gerber per la definicio del macro.
        /// </summary>
        /// 
        public string Command {
            get { return GetCommand(); }
        }
    }
}
