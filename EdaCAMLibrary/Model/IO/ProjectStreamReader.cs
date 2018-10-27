namespace MikroPic.EdaTools.v1.Cam.Model.IO {

    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Reflection;
    using MikroPic.EdaTools.v1.Xml;

    public sealed class ProjectStreamReader {

        private static readonly XmlSchemaSet schemas;

        private readonly XmlReaderAdapter rd;
        private Project project;
        private int version;

        /// <summary>
        /// Constructor estatic de la clase
        /// </summary>
        /// 
        static ProjectStreamReader() {

            schemas = new XmlSchemaSet();

            string schemaResourceName = "MikroPic.EdaTools.v1.Cam.Model.IO.Schemas.XCAM.xsd";
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", schemaResourceName));
            XmlSchema schema = XmlSchema.Read(resourceStream, null);
            schemas.Add(schema);

            schemas.Compile();
        }

        public ProjectStreamReader(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead)
                throw new InvalidOperationException("El stream no es de lectura.");

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.CloseInput = false;
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemas;
            settings.ConformanceLevel = ConformanceLevel.Document;

            XmlReader reader = XmlReader.Create(stream, settings);
            rd = new XmlReaderAdapter(reader);
        }

        public Project Read() {

            project = new Project();

            rd.NextTag();
            ParseDocumentNode(project);

            return project;
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// <param name="project">La placa.</param>
        /// 
        private void ParseDocumentNode(Project project) {

            if (!rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            rd.NextTag();
            ParseProjectNode(project);

            // Llegeix el tag final
            //
            rd.NextTag();
        }
    }
}
