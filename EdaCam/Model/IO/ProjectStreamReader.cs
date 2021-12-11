using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.IO;

namespace MikroPic.EdaTools.v1.Cam.Model.IO {

    public sealed class ProjectStreamReader {

        private static readonly XmlSchemaSet schemas;

        private readonly XmlReaderAdapter rd;
        private int version;

        /// <summary>
        /// Constructor estatic de la clase
        /// </summary>
        /// 
        static ProjectStreamReader() {

            string schemaResourceName = "MikroPic.EdaTools.v1.Cam.Model.IO.Schemas.CamDocument.xsd";
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", schemaResourceName));

            schemas = new XmlSchemaSet();
            schemas.Add(XmlSchema.Read(resourceStream, null));
            schemas.Compile();
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="stream">El stream de lectura.</param>
        /// 
        public ProjectStreamReader(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

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
        /// <returns>L'objecte 'Project' obtingut.</returns>
        /// 
        private Project ParseDocumentNode() {

            if (!rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            version = rd.AttributeAsInteger("version");

            rd.NextTag();
            Project project = ParseProjectNode();

            rd.NextTag();
            if (!rd.IsEndTag("document"))
                throw new InvalidDataException("Se esperaba </document>");

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

            rd.NextTag();
            IEnumerable<Target> targetList = ParseTargetsNode();

            rd.NextTag();
            if (!rd.IsEndTag("project"))
                throw new InvalidDataException("Se esperaba </project>");

            Project project = new Project(targetList);
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

            List<Target> targets = new List<Target>();

            rd.NextTag();
            while (rd.IsStartTag("target")) {
                targets.Add(ParseTargetNode());
                rd.NextTag();
            }
            if (!rd.IsEndTag("targets"))
                throw new InvalidDataException("Se esperaba </targets>");

            return targets;
        }

        /// <summary>
        /// Procesa un node 'target'
        /// </summary>
        /// <returns>L'objecte 'Target' obtingut.</returns>
        /// 
        private Target ParseTargetNode() {

            if (!rd.IsStartTag("target"))
                throw new InvalidDataException("Se esperaba <target>");

            string name = rd.AttributeAsString("name");
            string fileName = rd.AttributeAsString("output");
            string generatorName = rd.AttributeAsString("generator");
            EdaPoint position = rd.AttributeExists("position") ?
                EdaParser.ParsePoint(rd.AttributeAsString("position")) :
                new EdaPoint(0, 0);
            EdaAngle rotation = rd.AttributeExists("rotation") ?
                EdaParser.ParseAngle(rd.AttributeAsString("rotation")) :
                EdaAngle.Zero;

            IEnumerable<string> layers = null;
            IEnumerable<TargetOption> options = null;

            rd.NextTag();
            if (rd.IsStartTag("layers")) {
                layers = ParseLayersNode();
                rd.NextTag();
            }
            if (rd.IsStartTag("options")) {
                options = ParseOptionsNode();
                rd.NextTag();
            }
            if (!rd.IsEndTag("target"))
                throw new InvalidDataException("Se esperaba </target>");

            Target target = new Target(name, fileName, generatorName, position, rotation, layers, options);
            return target;
        }

        /// <summary>
        /// Procesa un node 'layers'.
        /// </summary>
        /// <returns>La llista d'objectes 'string' obtinguda.</returns>
        /// 
        private IEnumerable<string> ParseLayersNode() {

            if (!rd.IsStartTag("layers"))
                throw new InvalidDataException("Se esperaba <layers>");

            List<string> layerNames = new List<string>();

            rd.NextTag();
            while (rd.IsStartTag("layer")) {
                layerNames.Add(ParseLayerNode());
                rd.NextTag();
            }
            if (!rd.IsEndTag("layers"))
                throw new InvalidDataException("Se esperaba </layers>");

            return layerNames;
        }

        /// <summary>
        /// Procesa un node 'layer'.
        /// </summary>
        /// /// <returns>L'objecte 'string' obtingut.</returns>
        /// 
        private string ParseLayerNode() {

            if (!rd.IsStartTag("layer"))
                throw new InvalidDataException("Se esperaba <layer>");

            string name = rd.AttributeAsString("name");

            rd.NextTag();
            if (!rd.IsEndTag("layer"))
                throw new InvalidDataException("Se esperaba </layer>");

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

            List<TargetOption> targetOptions = new List<TargetOption>();

            rd.NextTag();
            while (rd.IsStartTag("option")) {
                targetOptions.Add(ParseOptionNode());
                rd.NextTag();
            }
            if (!rd.IsEndTag("options"))
                throw new InvalidDataException("Se esperaba </options>");

            return targetOptions;
        }

        /// <summary>
        /// Procesa un node 'option'
        /// </summary>
        /// <returns>L'objecte 'targetOption' obtingut.</returns>
        /// 
        private TargetOption ParseOptionNode() {

            if (!rd.IsStartTag("option"))
                throw new InvalidDataException("Se esperaba <option>");

            string name = rd.AttributeAsString("name");
            string value = rd.AttributeAsString("value");

            rd.NextTag();
            if (!rd.IsEndTag("option"))
                throw new InvalidDataException("Se esperaba </option>");

            TargetOption targetOption = new TargetOption(name, value);
            return targetOption;
        }
    }
}
