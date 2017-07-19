namespace MikroPic.EdaTools.v1.Model.IO {

    using System;
    using System.Windows;
    using System.Xml;

    public static class XmlWriterHelper {

        public static void WriteAttribute(this XmlWriter writer, string name, Point point) {

            writer.WriteAttributeString(
                name, 
                String.Format("{0}, {1}", XmlConvert.ToString(point.X), XmlConvert.ToString(point.Y)));
        }

        public static void WriteAttribute(this XmlWriter writer, string name, Size size) {

            writer.WriteAttributeString(
                name, 
                String.Format("{0}, {1}", XmlConvert.ToString(size.Width), XmlConvert.ToString(size.Height)));
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
    }
}
