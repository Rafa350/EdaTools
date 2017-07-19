namespace Eda.PCBTool.XmlHelper {

    using System;
    using System.Globalization;
    using System.Xml;

    public static class XmlWriterHelper {

        public static void WriteAttributeDouble(this XmlWriter writer, string name, double value) {

            writer.WriteAttributeString(
                name,
                String.Format(CultureInfo.InvariantCulture, "{0}", value));
        }

        public static void WriteAttributeDoublePair(this XmlWriter writer, string name, double value1, double value2) {

            writer.WriteAttributeString(
                name,
                String.Format(CultureInfo.InvariantCulture, "{0};{1}", value1, value2));
        }

        public static void WriteAttributeRotate(this XmlWriter writer, string name, double value) {

            writer.WriteAttributeString(
                name,
                String.Format(CultureInfo.InvariantCulture, "R{0}", value));
        }
    }

}
