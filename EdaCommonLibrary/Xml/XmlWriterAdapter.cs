namespace MikroPic.EdaTools.v1.Xml {

    using System;
    using System.Xml;
    using System.Text;

    public sealed class XmlWriterAdapter {

        private readonly XmlWriter writer;

        public XmlWriterAdapter(XmlWriter writer) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        public void WriteStartElement(string name) {

            writer.WriteStartElement(name);
        }

        public void WriteEndElement() {

            writer.WriteEndElement();
        }

        public void WriteAttribute(string name, string value) {

            writer.WriteAttributeString(name, value);
        }

        public void WriteAttribute(string name, string[] values) {

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

        public void WriteAttribute(string name, bool value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public void WriteAttribute(string name, int value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public void WriteAttribute(string name, double value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public void WriteAttribute(string name, Enum value) {

            writer.WriteAttributeString(name, value.ToString());
        }
    }
}
