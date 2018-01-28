namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using System;
    using System.Globalization;
    using System.Xml;
    using System.Windows;
    using System.Windows.Media;

    internal static class XmlNodeHelper {

        public static string AttributeAsString(this XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return null;
            else
                return node.Attributes[name].Value;
        }

        public static string[] AttributeAsStrings(this XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return null;
            else
                return attribute.Value.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries );
        }

        public static int AttributeAsInteger(this XmlNode node, string name, int defValue = 0) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else
                return Int32.Parse(attribute.Value);
        }

        public static double AttributeAsDouble(this XmlNode node, string name, double defValue = 0) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else
                return Double.Parse(attribute.Value, CultureInfo.InvariantCulture);
        }

        public static Color AttributeAsColor(this XmlNode node, string name) {

            string colorString = node.Attributes[name].Value;

            string[] s = colorString.Split(',');
            byte a = Byte.Parse(s[0]);
            byte r = Byte.Parse(s[1]);
            byte g = Byte.Parse(s[2]);
            byte b = Byte.Parse(s[3]);

            return Color.FromArgb(a, r, g, b);
        }

        public static Point AttributeAsPoint(this XmlNode node, string name) {

            string pointString = node.Attributes[name].Value;

            string[] s = pointString.Split(',');
            double x = Double.Parse(s[0], CultureInfo.InvariantCulture);
            double y = Double.Parse(s[1], CultureInfo.InvariantCulture);

            return new Point(x, y);
        }

        public static Size AttributeAsSize(this XmlNode node, string name) {

            string pointString = node.Attributes[name].Value;

            string[] s = pointString.Split(',');
            double w = Double.Parse(s[0], CultureInfo.InvariantCulture);
            double h = Double.Parse(s[1], CultureInfo.InvariantCulture);

            return new Size(w, h);
        }

        public static T AttributeAsEnum<T>(this XmlNode node, string name, T defValue = default(T)) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defValue;
            else
                return (T)Enum.Parse(typeof(T), attribute.Value);
        }
    }
}
