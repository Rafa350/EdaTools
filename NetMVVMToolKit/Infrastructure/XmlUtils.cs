namespace MikroPic.NetMVVMToolkit.v1.Infrastructure {

    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Xsl;

    internal static class XmlUtils {

        public static XmlSchema GetSchemaFromResource(string resourceName, bool throwOnError = true) {

            if (String.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException("resourceName");

            try {
                Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                return XmlSchema.Read(s, null);
            }
            catch {
                if (throwOnError)
                    throw;
                else
                    return null;
            }
        }

        public static XslCompiledTransform GetTransformFromResource(string resourceName, bool throwOnError = true) {

            if (String.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException("resourceName");

            try {
                Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                XmlReader reader = XmlReader.Create(s);
                try {
                    XslCompiledTransform transform = new XslCompiledTransform();
                    transform.Load(reader, new XsltSettings(true, false), new XmlUrlResolver());
                    return transform;
                }
                finally {
                    reader.Close();
                }
            }
            catch {
                if (throwOnError)
                    throw;
                else
                    return null;
            }
        }

        public static XslCompiledTransform GetTransformFromFile(string fileName, bool throwOnError = true) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            try {
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(fileName, new XsltSettings(true, false), new XmlUrlResolver());
                return transform;
            }
            catch {
                if (throwOnError)
                    throw;
                else
                    return null;
            }
        }

        public static XmlReader GetReader(string fileName, XmlSchema schema = null) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            XmlReaderSettings rdSettings = new XmlReaderSettings();
            rdSettings.IgnoreComments = true;
            rdSettings.IgnoreProcessingInstructions = true;
            rdSettings.IgnoreWhitespace = true;
            rdSettings.CloseInput = true;
            if (schema != null) {
                rdSettings.ValidationType = ValidationType.Schema;
                rdSettings.Schemas.Add(schema);
            }
            else
                rdSettings.ValidationType = ValidationType.None;

            // Comprova si existeix un fitxer per la cultura actual, en cas contrari
            // utilitza el fitxer generic
            //
            string cultureFileName =
                Path.ChangeExtension(fileName,
                    String.Format("{0}{1}",
                        CultureInfo.CurrentCulture.Name,
                        Path.GetExtension(fileName)));
            if (File.Exists(cultureFileName))
                fileName = cultureFileName;

            return XmlReader.Create(fileName, rdSettings);
        }
    }
}
