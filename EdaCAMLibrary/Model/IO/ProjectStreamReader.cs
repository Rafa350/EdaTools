namespace MikroPic.EdaTools.v1.Cam.Model.IO {

    using MikroPic.EdaTools.v1.Xml;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;

    public sealed class ProjectStreamReader {

        private static readonly XmlSchemaSet schemas;
        private static XmlReaderSettings settings;

        private readonly XmlReaderAdapter rd;
        private int version;

        /// <summary>
        /// Constructor estatic de la clase
        /// </summary>
        /// 
        static ProjectStreamReader() {

            string schemaResourceName = "MikroPic.EdaTools.v1.Cam.Model.IO.Schemas.XCAM.xsd";
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", schemaResourceName));

            schemas = new XmlSchemaSet();
            schemas.Add(XmlSchema.Read(resourceStream, null));
            schemas.Compile();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.CloseInput = false;
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemas;
            settings.ConformanceLevel = ConformanceLevel.Document;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="stream">El stream de lectura.</param>
        /// 
        public ProjectStreamReader(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead)
                throw new InvalidOperationException("El stream no es de lectura.");

            XmlReader reader = XmlReader.Create(stream, settings);
            rd = new XmlReaderAdapter(reader);
        }

        /// <summary>
        /// Llegeix un projecte CAM
        /// </summary>
        /// <returns>El projecte obtingut.</returns>
        /// 
        public Project Read() {

            rd.NextTag();
            Project project = ParseDocumentNode();

            return project;
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// <returns>L'objectr 'Project' obtingut.</returns>
        /// 
        private Project ParseDocumentNode() {

            if (!rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            version = rd.AttributeAsInteger("version");

            rd.NextTag();
            Project project = ParseProjectNode();

            if (!rd.IsEndTag("document"))
                throw new InvalidDataException("Se esperaba </document>");

            rd.NextTag();

            return project;
        }

        /// <summary>
        /// Procesa un node 'project'
        /// </summary>
        /// <returns>L'objecte 'Project' obtingut.</returns>
        /// 
        private Project ParseProjectNode() {

            if (!rd.IsStartTag("project"))
                throw new InvalidDataException("Se esperaba <project>");

            string boardFileName = rd.AttributeAsString("board");

            rd.NextTag();
            IEnumerable<Target> targetList = ParseTargetsNode();

            Project project = new Project(boardFileName, targetList);

            if (!rd.IsEndTag("project"))
                throw new InvalidDataException("Se esperaba </project>");

            rd.NextTag();

            return project;
        }

        /// <summary>
        /// Procesa un node 'targets'
        /// </summary>
        /// <returns>La llista d'objectes 'Target' obtinguda.</returns>
        /// 
        private IEnumerable<Target> ParseTargetsNode() {

            if (!rd.IsStartTag("targets"))
                throw new InvalidDataException("Se esperaba <targets>");

            rd.NextTag();

            List<Target> targetList = new List<Target>();
            while (rd.IsStartTag("target"))
                targetList.Add(ParseTargetNode());

            if (!rd.IsEndTag("targets"))
                throw new InvalidDataException("Se esperaba </targets>");

            rd.NextTag();

            return targetList;
        }

        /// <summary>
        /// Procesa un node 'target'
        /// </summary>
        /// <returns>L'objecte 'Target' obtingut.</returns>
        /// 
        private Target ParseTargetNode() {

            // Comprova que el node sigui correcte
            //
            if (!rd.IsStartTag("target"))
                throw new InvalidDataException("Se esperaba <target>");

            // Obte els atributs de la capa
            //
            string fileName = rd.AttributeAsString("output");
            string generatorName = rd.AttributeAsString("generator");

            IEnumerable<string> layerList = null;
            IEnumerable<TargetOption> optionList = null;

            rd.NextTag();

            if (rd.IsStartTag("layers"))
                layerList = ParseLayersNode();

            if (rd.IsStartTag("options")) 
                optionList = ParseOptionsNode();

            // Crea el target i l'afeigeix al projecte
            //
            Target target = new Target(fileName, generatorName, layerList, optionList);

            if (!rd.IsEndTag("target"))
                throw new InvalidDataException("Se esperaba </target>");

            rd.NextTag();

            return target;
        }

        /// <summary>
        /// Procesa un node 'layers'.
        /// </summary>
        /// <returns>La llista d'ojectes 'string' obtinguda.</returns>
        /// 
        private IEnumerable<string> ParseLayersNode() {

            if (!rd.IsStartTag("layers"))
                throw new InvalidDataException("Se esperaba <layers>");

            rd.NextTag();

            List<string> layerNameList = new List<string>();
            while (rd.IsStartTag("layer"))
                layerNameList.Add(ParseLayerNode());

            if (!rd.IsEndTag("layers"))
                throw new InvalidDataException("Se esperaba </layers>");

            rd.NextTag();

            return layerNameList;
        }

        /// <summary>
        /// Procesa un node 'layer'.
        /// </summary>
        /// /// <returns>L'objecte 'string' obtingut.</returns>
        /// 
        private string ParseLayerNode() {

            if (!rd.IsStartTag("layer"))
                throw new InvalidDataException("Se esperaba <layer>");

            // Obte els atributs de la capa
            //
            string name = rd.AttributeAsString("name");

            rd.NextTag();
            if (!rd.IsEndTag("layer"))
                throw new InvalidDataException("Se esperaba </layer>");

            rd.NextTag();

            return name;
        }

        /// <summary>
        /// Procesa un node 'options'.
        /// </summary>
        /// <returns>La llista d'objectes 'TargetOption' obtinguda.</returns>
        /// 
        private IList<TargetOption> ParseOptionsNode() {

            if (!rd.IsStartTag("options"))
                throw new InvalidDataException("Se esperaba <options>");

            rd.NextTag();

            List<TargetOption> targetOptionList = new List<TargetOption>();
            while (rd.IsStartTag("option"))
                targetOptionList.Add(ParseOptionNode());

            if (!rd.IsEndTag("options"))
                throw new InvalidDataException("Se esperaba </options>");

            rd.NextTag();

            return targetOptionList;
        }

        /// <summary>
        /// Procesa un node 'option'
        /// </summary>
        /// <returns>L'objecte 'targetOption' obtingut.</returns>
        /// 
        private TargetOption ParseOptionNode() {

            if (!rd.IsStartTag("option"))
                throw new InvalidDataException("Se esperaba <option>");

            // Obte els atributs de l'opcio
            //
            string name = rd.AttributeAsString("name");
            string value = rd.AttributeAsString("value");

            TargetOption targetOption = new TargetOption(name, value);

            rd.NextTag();
            if (!rd.IsEndTag("option"))
                throw new InvalidDataException("Se esperaba </option>");

            rd.NextTag();

            return targetOption;
        }
    }
}
