namespace MikroPic.EdaTools.v1.XmlUtils {

    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;

    public static class XmlSchemaLoader {

        public static XmlSchema FromFile(string fileName) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            return XmlSchema.Read(stream, null);
        }

        public static XmlSchema FromResource(string resourceName) {

            if (String.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException("resourceName");

            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", resourceName));

            return XmlSchema.Read(resourceStream, null);
        }

        public static XmlSchema FromStream(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            return XmlSchema.Read(stream, null);
        }

        public static XmlSchema FromReader(XmlReader reader) {

            if (reader == null)
                throw new ArgumentNullException("reader");

            return XmlSchema.Read(reader, null);
        }
    }
}
