namespace MikroPic.EdaTools.v1.Xml {

    using System;
    using System.Xml;
    using System.Text;

    public sealed class XmlWriterAdapter {

        private readonly XmlWriter writer;

        /// <summary>
        /// Contructor de l'objecte.
        /// </summary>
        /// <param name="writer">Objecte XmlWriter.</param>
        /// 
        public XmlWriterAdapter(XmlWriter writer) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        /// <summary>
        /// Escriu un inici d'element.
        /// </summary>
        /// <param name="name">El nom de l'element.</param>
        /// 
        public void WriteStartElement(string name) {

            writer.WriteStartElement(name);
        }

        /// <summary>
        /// Escriu un inici d'element.
        /// </summary>
        /// <param name="name">El nom de l'element.</param>
        /// <param name="ns">El espai de noms.</param>
        /// 
        public void WriteStartElement(string name, string ns) {

            writer.WriteStartElement(name, ns);
        }

        /// <summary>
        /// Escriu el final d'element.
        /// </summary>
        /// 
        public void WriteEndElement() {

            writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu un atribut de tipus 'string'
        /// </summary>
        /// <param name="name">El nom del atribut.</param>
        /// <param name="value">El valor.</param>
        /// 
        public void WriteAttribute(string name, string value) {

            writer.WriteAttributeString(name, value);
        }

        /// <summary>
        /// Escriu un atribut de tipus 'string[]'
        /// </summary>
        /// <param name="name">El nom del atribut.</param>
        /// <param name="values">El valor.</param>
        /// 
        public void WriteAttribute(string name, string[] values) {

            if (values == null)
                writer.WriteAttributeString(name, "");
            else {
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (string value in values) {
                    if (first)
                        first = false;
                    else
                        sb.Append(", ");
                    sb.Append(value);
                }
                writer.WriteAttributeString(name, sb.ToString());
            }
        }

        /// <summary>
        /// Escriu un atribut de tipus 'bool'.
        /// </summary>
        /// <param name="name">En nom del atribut.</param>
        /// <param name="value">El valor.</param>
        /// 
        public void WriteAttribute(string name, bool value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        /// <summary>
        /// Escriu un atribut de tipus 'int'.
        /// </summary>
        /// <param name="name">El nom del atribut.</param>
        /// <param name="value">El valor.</param>
        /// 
        public void WriteAttribute(string name, int value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        /// <summary>
        /// Escriu un atribut de tipus 'double'.
        /// </summary>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="value">El valor.</param>
        /// 
        public void WriteAttribute(string name, double value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        /// <summary>
        /// Escriu un atribut de tipus 'enum'
        /// </summary>
        /// <param name="name">El nom del atribut.</param>
        /// <param name="value">El valor.</param>
        /// 
        public void WriteAttribute(string name, Enum value) {

            writer.WriteAttributeString(name, value.ToString().ToLowerInvariant());
        }
    }
}
