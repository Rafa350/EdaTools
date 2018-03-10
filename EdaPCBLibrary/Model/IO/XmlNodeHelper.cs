namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using System;
    using System.Xml;

    internal static class XmlNodeHelper {

        public static string AttributeAsString(this XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return null;
            else
                return attribute.Value;
        }

        public static string[] AttributeAsStrings(this XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return null;
            else
                return attribute.Value.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries );
        }

        public static bool AttributeAsBoolean(this XmlNode node, string name, bool defValue = false) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else
                return XmlConvert.ToBoolean(attribute.Value);
        }

        public static int AttributeAsInteger(this XmlNode node, string name, int defValue = 0) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else
                return XmlConvert.ToInt32(attribute.Value);
        }

        public static double AttributeAsDouble(this XmlNode node, string name, double defValue = 0) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else
                return XmlConvert.ToDouble(attribute.Value);
        }

        public static T AttributeAsEnum<T>(this XmlNode node, string name, T defValue = default(T)) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else
                return (T)Enum.Parse(typeof(T), attribute.Value, true);
        }

        public static bool AttributeExists(this XmlNode node, string name) {

            return node.Attributes[name] != null;
        }
    }
}
