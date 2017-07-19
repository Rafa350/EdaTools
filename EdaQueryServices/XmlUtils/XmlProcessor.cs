namespace MikroPic.EdaTools.v1.XmlUtils {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Xsl;

    public sealed class XmlProcessor {

        public void Transform(
            TextReader input,
            XslCompiledTransform transformation,
            XmlSchema schema,
            TextWriter output) {

            Transform(input, transformation, schema, output, null, null);
        }

        public void Transform(
            TextReader input, 
            XslCompiledTransform transformation,
            XmlSchema schema, 
            TextWriter output, 
            IDictionary<string, string> parameters,
            IDictionary<string, object> extensions) {

            if (input == null)
                throw new ArgumentNullException("input");

            if (transformation == null)
                throw new ArgumentNullException("transformation");

            if (output == null)
                throw new ArgumentNullException("output");

            // Prepara els arguments de la transformacio
            //
            XsltArgumentList arguments = new XsltArgumentList();
            if (parameters != null)
                foreach (KeyValuePair<string, string> kv in parameters)
                    arguments.AddParam(kv.Key, "", kv.Value);

            if (extensions != null)
                foreach (KeyValuePair<string, object> kv in extensions)
                    arguments.AddExtensionObject(kv.Key, kv.Value);

            // Prepara les opcions del lector Xml
            //
            XmlReaderSettings rdSettings = new XmlReaderSettings();
            if (schema != null) {
                rdSettings.Schemas.Add(schema);
                rdSettings.ValidationType = ValidationType.Schema;
            }
            else
                rdSettings.ValidationType = ValidationType.None;
            rdSettings.IgnoreWhitespace = true;
            rdSettings.IgnoreComments = true;
            rdSettings.IgnoreProcessingInstructions = true;
            rdSettings.CloseInput = true;
            rdSettings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader reader = XmlReader.Create(input, rdSettings);
            try {
                XmlWriterSettings wrSettings = transformation.OutputSettings.Clone();
                wrSettings.IndentChars = "    ";
                XmlWriter writer = XmlWriter.Create(output, wrSettings);
                try {
                    transformation.Transform(reader, arguments, writer);
                }
                finally {
                    writer.Close();
                }
            }
            finally {
                reader.Close();
            }
        }
    }
}



