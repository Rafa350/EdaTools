namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System;
    using System.Text;

    public sealed class Macro {

        private static int __id = 0;
        private readonly int id;
        private readonly string text;

        /// <summary>
        /// Construictor del objecte.
        /// </summary>
        /// <param name="text">La sequencia de comandes del macro.</param>
        /// 
        public Macro(string text) {

            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            this.id = __id++;
            this.text = text;
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
            get {

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("%AMM{0}*", id);
                sb.Append(text);
                if (!text.EndsWith("%"))
                    sb.Append('%');

                return sb.ToString();
            }
        }
    }
}
