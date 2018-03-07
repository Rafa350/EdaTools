﻿namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using System;
    using System.Xml;
    using System.Windows;
    using System.Windows.Media;

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

        public static int AttributeAsDouble(this XmlNode node, string name, double defValue = 0) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return (int) defValue;
            else
                return (int) (XmlConvert.ToDouble(attribute.Value) * 1000000.0);
        }

        public static Color AttributeAsColor(this XmlNode node, string name, Color defColor = default(Color)) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defColor;

            else {
                string colorString = node.Attributes[name].Value;

                string[] s = colorString.Split(',');
                byte a = XmlConvert.ToByte(s[0]);
                byte r = XmlConvert.ToByte(s[1]);
                byte g = XmlConvert.ToByte(s[2]);
                byte b = XmlConvert.ToByte(s[3]);

                return Color.FromArgb(a, r, g, b);
            }
        }

        public static Angle AttributeAsAngle(this XmlNode node, string name, Angle defAngle = default(Angle)) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return defAngle;

            else {
                double value = XmlConvert.ToDouble(node.Attributes[name].Value);
                return Angle.FromDegrees((int)(value * 100.0));
            }
        }

        public static PointInt AttributeAsPoint(this XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null) 
                return default(PointInt);
            
            else {
                string pointString = attribute.Value;

                string[] s = pointString.Split(',');
                double x = XmlConvert.ToDouble(s[0]) * 1000000.0;
                double y = XmlConvert.ToDouble(s[1]) * 1000000.0;

                return new PointInt((int)x, (int)y);
            }
        }

        public static SizeInt AttributeAsSize(this XmlNode node, string name) {

            XmlAttribute attribute = node.Attributes[name];
            if (attribute == null)
                return default(SizeInt);

            else {
                string pointString = attribute.Value;

                string[] s = pointString.Split(',');
                double w = XmlConvert.ToDouble(s[0]) * 1000000.0;
                double h = XmlConvert.ToDouble(s[1]) * 1000000.0;

                return new SizeInt((int)w, (int)h);
            }
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
