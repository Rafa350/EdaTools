using System;
using System.Xml;

namespace MikroPic.EdaTools.v1.Base.Xml {

    public static class XmlWriterExtensions {

        public static void WriteAttributeBool(this XmlWriter writer, string name, bool value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public static void WriteAttributeInteger(this XmlWriter writer, string name, int value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public static void WriteAttributeDouble(this XmlWriter writer, string name, double value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public static void WriteAttributeEnum(this XmlWriter writer, string name, Enum value) {

            writer.WriteAttributeString(name, value.ToString().ToLowerInvariant());
        }
    }
}
