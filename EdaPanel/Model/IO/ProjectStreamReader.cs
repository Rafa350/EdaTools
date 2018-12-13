namespace MikroPic.EdaTools.v1.Panel.Model.IO {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Xml;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// Clase per la lectura de panells des d'un stream
    /// </summary>
    /// 
    public sealed class ProjectStreamReader {

        private static readonly XmlSchemaSet schemas;

        private readonly XmlReaderAdapter rd;
        private int version;

        /// <summary>
        /// Constructor estatic de l'objecte.
        /// </summary>
        /// 
        static ProjectStreamReader() {

            schemas = new XmlSchemaSet();
            string schemaResourceName = "MikroPic.EdaTools.v1.Panel.Model.IO.Schemas.XPNL.xsd";
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", schemaResourceName));
            XmlSchema schema = XmlSchema.Read(resourceStream, null);
            schemas.Add(schema);

            schemas.Compile();
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="stream">Stream de lectura.</param>
        /// 
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
            settings.Schemas = schemas;
            settings.ValidationType = schemas == null ? ValidationType.None : ValidationType.Schema;
            settings.ConformanceLevel = ConformanceLevel.Document;

            XmlReader reader = XmlReader.Create(stream, settings);
            rd = new XmlReaderAdapter(reader);
        }

        /// <summary>
        /// Llegeix una panell.
        /// </summary>
        /// <returns>El panell.</returns>
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
            Project project = ParsePanelNode();

            rd.NextTag();
            if (!rd.IsEndTag("document"))
                throw new InvalidDataException("Se esperaba </document>");

            return project;
        }

        /// <summary>
        /// Procesa el node 'panel'
        /// </summary>
        /// <returns>L'objecte 'Project' obtingut.</returns>
        /// 
        private Project ParsePanelNode() {

            if (!rd.IsStartTag("panel"))
                throw new InvalidDataException("Se esperaba <panel>");

            Size size = XmlTypeParser.ParseSize(rd.AttributeAsString("size"));

            rd.NextTag();
            IEnumerable<ProjectItem> elements = ParsePanelElementsNode();

            rd.NextTag();
            if (!rd.IsEndTag("panel"))
                throw new InvalidDataException("Se esperaba </panel>");

            Project project = new Project();
            project.Size = size;
            project.AddElements(elements);

            return project;
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <returns>La coleccio d'objectes 'ProjectItem' obtinguda.</returns>
        /// 
        private IEnumerable<ProjectItem> ParsePanelElementsNode() {

            if (!rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            List<ProjectItem> items = new List<ProjectItem>();

            rd.NextTag();
            while (rd.IsStart) {
                switch (rd.TagName) {
                    case "place":
                        items.Add(ParsePlaceNode());
                        break;

                    case "milling":
                        items.Add(ParseMilling());
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <place> o <milling>");
                }
                rd.NextTag();
            }

            if (!rd.IsEndTag("elements"))
                throw new InvalidDataException("Se esperaba </elements>");

            return items;
        }

        /// <summary>
        /// Procesa el node 'place'
        /// </summary>
        /// <returns>L'objecte 'PcbItem' obtingut.</returns>
        /// 
        private PcbItem ParsePlaceNode() {

            if (!rd.IsStartTag("place"))
                throw new InvalidDataException("Se esperaba <place>");

            string fileName = rd.AttributeAsString("board");
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation"));

            rd.NextTag();
            if (!rd.IsEndTag("place"))
                throw new InvalidDataException("Se esperaba </place>");

            return new PcbItem(fileName, position, rotation);
        }

        /// <summary>
        /// Procesa un node 'milling'.
        /// </summary>
        /// <returns>L'objecte 'CutItem' obtingut.</returns>
        /// 
        private CutItem ParseMilling() {

            if (!rd.IsStartTag("milling"))
                throw new InvalidDataException("Se esperaba <milling>");

            Point startPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("startPosition"));
            Point endPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("endPosition"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness"));
            int cutSpacing = XmlTypeParser.ParseNumber(rd.AttributeAsString("cutSpacing"));
            int holeSpacing = XmlTypeParser.ParseNumber(rd.AttributeAsString("holeSpacing"));
            int holeDiameter = XmlTypeParser.ParseNumber(rd.AttributeAsString("holeDiameter"));
            int margin = rd.AttributeExists("margin") ?
                XmlTypeParser.ParseNumber(rd.AttributeAsString("margin")) :
                0;
            int cuts = rd.AttributeExists("cuts") ?
                rd.AttributeAsInteger("cuts") :
                1;
            int holes = rd.AttributeExists("holes") ?
                rd.AttributeAsInteger("holes") :
                5;

            rd.NextTag();
            if (!rd.IsEndTag("milling"))
                throw new InvalidDataException("Se esperaba </milling>");

            return new CutItem(startPosition, endPosition, thickness, margin, cuts, cutSpacing, 
                holes, holeDiameter, holeSpacing);
        }
    }
}