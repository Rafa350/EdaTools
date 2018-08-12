namespace MikroPic.EdaTools.v1.Pcb.Model.IO.Infrastructure {

    using System;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Clase amb metodes d'extensio per la clase XmlWriter
    /// </summary>
    /// 
    internal static class XmlWriterHelper {

        public static void WriteAttribute(this XmlWriter writer, string name, Enum value) {

            writer.WriteAttributeString(name, value.ToString());
        }

        public static void WriteAttribute(this XmlWriter writer, string name, double value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }
        
        public static void WriteAttribute(this XmlWriter writer, string name, int value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public static void WriteAttribute(this XmlWriter writer, string name, bool value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public static void WriteAttribute(this XmlWriter writer, string name, string value) {

            writer.WriteAttributeString(name, value);
        }

        public static void WriteAttribute(this XmlWriter writer, string name, string[] values) {

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
}
