namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

    using MikroPic.EdaTools.v1.Base.Xml;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;

    public sealed class LibraryStreamReader {

        private static readonly XmlSchemaSet schemas;
        private readonly XmlReaderAdapter rd;

        /// <summary>
        /// Constructor estatic de la clase
        /// </summary>
        /// 
        static LibraryStreamReader() {

            schemas = new XmlSchemaSet();

            string schemaResourceName = "MikroPic.EdaTools.v1.Core.Model.Board.IO.Schemas.XLIB.xsd";
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", schemaResourceName));
            XmlSchema schema = XmlSchema.Read(resourceStream, null);
            schemas.Add(schema);

            schemas.Compile();
        }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="stream">Stream de lectura.</param>
        /// 
        public LibraryStreamReader(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead)
                throw new InvalidOperationException("El stream no es de lectura.");

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.CloseInput = false;
            settings.Schemas = schemas;
            settings.ValidationType = schemas == null ? ValidationType.None : ValidationType.Schema;
            settings.ConformanceLevel = ConformanceLevel.Document;

            XmlReader reader = XmlReader.Create(stream, settings);
            rd = new XmlReaderAdapter(reader);
        }

        public Library Read() {

            return null;
        }
    }
}
