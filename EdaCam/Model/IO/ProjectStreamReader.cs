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

        private static readonly XmlSchemaSet _schemas;

        private readonly XmlReaderAdapter _rd;
        private int _version;

        /// <summary>
        /// Constructor estatic de la clase
        /// </summary>
        /// 
        static ProjectStreamReader() {

            string schemaResourceName = "MikroPic.EdaTools.v1.Cam.Model.IO.Schemas.CamDocument.xsd";
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", schemaResourceName));

            _schemas = new XmlSchemaSet();
            _schemas.Add(XmlSchema.Read(resourceStream, null));
            _schemas.Compile();
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
            settings.Schemas = _schemas;
            settings.ValidationType = _schemas == null ? ValidationType.None : ValidationType.Schema;
            settings.ConformanceLevel = ConformanceLevel.Document;

            XmlReader reader = XmlReader.Create(stream, settings);
            _rd = new XmlReaderAdapter(reader);
        }

        /// <summary>
        /// Llegeix un projecte CAM
        /// </summary>
        /// <returns>El projecte obtingut.</returns>
        /// 
        public Project Read() {

            _rd.NextTag();
            Project project = ParseDocumentNode();

            return project;
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// <returns>L'objecte 'Project' obtingut.</returns>
        /// 
        private Project ParseDocumentNode() {

            if (!_rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            _version = _rd.AttributeAsInteger("version");

            _rd.NextTag();
            Project project = ParseProjectNode();

            _rd.NextTag();
            if (!_rd.IsEndTag("document"))
                throw new InvalidDataException("Se esperaba </document>");

            return project;
        }

        /// <summary>
        /// Procesa un node 'project'
        /// </summary>
        /// <returns>L'objecte 'Project' obtingut.</returns>
        /// 
        private Project ParseProjectNode() {

            if (!_rd.IsStartTag("project"))
                throw new InvalidDataException("Se esperaba <project>");

            string name = _rd.AttributeAsString("name");
            string revision = _rd.AttributeAsString("revision");

            _rd.NextTag();
            IEnumerable<Target> targetList = ParseTargetsNode();

            _rd.NextTag();
            if (!_rd.IsEndTag("project"))
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

            if (!_rd.IsStartTag("targets"))
                throw new InvalidDataException("Se esperaba <targets>");

            List<Target> targets = new List<Target>();

            _rd.NextTag();
            while (_rd.IsStartTag("target")) {
                targets.Add(ParseTargetNode());
                _rd.NextTag();
            }
            if (!_rd.IsEndTag("targets"))
                throw new InvalidDataException("Se esperaba </targets>");

            return targets;
        }

        /// <summary>
        /// Procesa un node 'target'
        /// </summary>
        /// <returns>L'objecte 'Target' obtingut.</returns>
        /// 
        private Target ParseTargetNode() {

            if (!_rd.IsStartTag("target"))
                throw new InvalidDataException("Se esperaba <target>");

            string name = _rd.AttributeAsString("name");
            string fileName = _rd.AttributeAsString("output");
            string generatorName = _rd.AttributeAsString("generator");
            EdaPoint position = _rd.AttributeExists("position") ?
                EdaParser.ParsePoint(_rd.AttributeAsString("position")) :
                new EdaPoint(0, 0);
            EdaAngle rotation = _rd.AttributeExists("rotation") ?
                EdaParser.ParseAngle(_rd.AttributeAsString("rotation")) :
                EdaAngle.Zero;

            IEnumerable<string> layers = null;
            IEnumerable<TargetOption> options = null;

            _rd.NextTag();
            if (_rd.IsStartTag("layers")) {
                layers = ParseLayersNode();
                _rd.NextTag();
            }
            if (_rd.IsStartTag("options")) {
                options = ParseOptionsNode();
                _rd.NextTag();
            }
            if (!_rd.IsEndTag("target"))
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

            if (!_rd.IsStartTag("layers"))
                throw new InvalidDataException("Se esperaba <layers>");

            List<string> layerNames = new List<string>();

            _rd.NextTag();
            while (_rd.IsStartTag("layer")) {
                layerNames.Add(ParseLayerNode());
                _rd.NextTag();
            }
            if (!_rd.IsEndTag("layers"))
                throw new InvalidDataException("Se esperaba </layers>");

            return layerNames;
        }

        /// <summary>
        /// Procesa un node 'layer'.
        /// </summary>
        /// /// <returns>L'objecte 'string' obtingut.</returns>
        /// 
        private string ParseLayerNode() {

            if (!_rd.IsStartTag("layer"))
                throw new InvalidDataException("Se esperaba <layer>");

            string name = _rd.AttributeAsString("name");

            _rd.NextTag();
            if (!_rd.IsEndTag("layer"))
                throw new InvalidDataException("Se esperaba </layer>");

            return name;
        }

        /// <summary>
        /// Procesa un node 'options'.
        /// </summary>
        /// <returns>La llista d'objectes 'TargetOption' obtinguda.</returns>
        /// 
        private IList<TargetOption> ParseOptionsNode() {

            if (!_rd.IsStartTag("options"))
                throw new InvalidDataException("Se esperaba <options>");

            List<TargetOption> targetOptions = new List<TargetOption>();

            _rd.NextTag();
            while (_rd.IsStartTag("option")) {
                targetOptions.Add(ParseOptionNode());
                _rd.NextTag();
            }
            if (!_rd.IsEndTag("options"))
                throw new InvalidDataException("Se esperaba </options>");

            return targetOptions;
        }

        /// <summary>
        /// Procesa un node 'option'
        /// </summary>
        /// <returns>L'objecte 'targetOption' obtingut.</returns>
        /// 
        private TargetOption ParseOptionNode() {

            if (!_rd.IsStartTag("option"))
                throw new InvalidDataException("Se esperaba <option>");

            string name = _rd.AttributeAsString("name");
            string value = _rd.AttributeAsString("value");

            _rd.NextTag();
            if (!_rd.IsEndTag("option"))
                throw new InvalidDataException("Se esperaba </option>");

            TargetOption targetOption = new TargetOption(name, value);
            return targetOption;
        }
    }
}
