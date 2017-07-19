namespace MikroPic.EdaTools.v1.XmlUtils {

    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Xsl;

    public static class XmlTransformationLoader {

        public static XslCompiledTransform FromFile(string fileName) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            XmlReader reader = XmlReader.Create(fileName);
            return FromReader(reader);
        }

        public static XslCompiledTransform FromResource(string resourceName) {

            if (String.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException("resourceName");

            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", resourceName));
            
            return FromStream(resourceStream);
        }

        public static XslCompiledTransform FromStream(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            XmlReader reader = XmlReader.Create(stream);
            return FromReader(reader);
        }

        public static XslCompiledTransform FromReader(XmlReader reader) {

            if (reader == null)
                throw new ArgumentNullException("reader");

            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(reader, new XsltSettings(true, false), new XmlUrlResolver());

            return transform;
        }
    }
}
