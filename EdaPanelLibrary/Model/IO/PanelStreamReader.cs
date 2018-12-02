namespace MikroPic.EdaTools.v1.Panel.Model.IO {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Xml;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.Elements;
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
    public sealed class PanelStreamReader {

        private static readonly XmlSchemaSet schemas;

        private readonly XmlReaderAdapter rd;
        private int version;

        /// <summary>
        /// Constructor estatic de l'objecte.
        /// </summary>
        /// 
        static PanelStreamReader() {

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
        public PanelStreamReader(Stream stream) {

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
        public Panel Read() {

            rd.NextTag();
            Panel panel = ParseDocumentNode();

            return panel;
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// <returns>L'objecte 'Panel' obtingut.</returns>
        /// 
        private Panel ParseDocumentNode() {

            if (!rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            int version = rd.AttributeAsInteger("version");

            rd.NextTag();
            Panel panel = ParsePanelNode();

            rd.NextTag();
            if (!rd.IsEndTag("document"))
                throw new InvalidDataException("Se esperaba </document>");

            return panel;
        }

        /// <summary>
        /// Procesa el node 'panel'
        /// </summary>
        /// <returns>L'objecte 'Panel' obtingut.</returns>
        /// 
        private Panel ParsePanelNode() {

            if (!rd.IsStartTag("panel"))
                throw new InvalidDataException("Se esperaba <panel>");

            version = rd.AttributeAsInteger("version");

            Panel panel = new Panel();

            rd.NextTag();
            panel.AddElements(ParsePanelElementsNode());

            rd.NextTag();
            if (!rd.IsEndTag("panel"))
                throw new InvalidDataException("Se esperaba </panel>");

            return panel;
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <returns>La coleccio d'objectes 'PanelElement' obtinguda.</returns>
        /// 
        private IEnumerable<PanelElement> ParsePanelElementsNode() {

            if (!rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            List<PanelElement> elements = new List<PanelElement>();

            rd.NextTag();
            while (rd.IsStart) {
                switch (rd.TagName) {
                    case "place":
                        elements.Add(ParsePlaceNode());
                        break;

                    case "join":
                        elements.Add(ParseJoinNode());
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <place> o <join>");
                }
                rd.NextTag();
            }

            if (!rd.IsEndTag("elements"))
                throw new InvalidDataException("Se esperaba </elements>");

            return elements;
        }

        /// <summary>
        /// Procesa el node 'place'
        /// </summary>
        /// <returns>L'objecte 'PlaceElement' obtingut.</returns>
        /// 
        private PlaceElement ParsePlaceNode() {

            if (!rd.IsStartTag("place"))
                throw new InvalidDataException("Se esperaba <place>");

            string fileName = rd.AttributeAsString("board");
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation"));

            rd.NextTag();
            if (!rd.IsEndTag("place"))
                throw new InvalidDataException("Se esperaba </place>");

            PlaceElement place = new PlaceElement(fileName, position, rotation);

            return place;
        }

        /// <summary>
        /// Procesa un node 'join'.
        /// </summary>
        /// <returns>L'objecte 'JoinElement' obtingut.</returns>
        /// 
        private JoinElement ParseJoinNode() {

            if (!rd.IsStartTag("join"))
                throw new InvalidDataException("Se esperaba <join>");

            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation"));

            rd.NextTag();
            if (!rd.IsEndTag("join"))
                throw new InvalidDataException("Se esperaba </join>");

            JoinElement join = new JoinElement(position, rotation);

            return join;
        }
    }
}