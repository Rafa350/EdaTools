namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.PanelElements;
    using MikroPic.EdaTools.v1.Xml;
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
        private Panel panel;
        private int version;

        /// <summary>
        /// Constructor estatic de l'objecte.
        /// </summary>
        /// 
        static PanelStreamReader() {

            schemas = new XmlSchemaSet();

            string schemaResourceName = "MikroPic.EdaTools.v1.Pcb.Model.IO.Schemas.XPNL.xsd";
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
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemas;
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

            panel = new Panel();

            rd.NextTag();
            ParseDocumentNode(panel);

            return panel;
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// <param name="panel">El panell.</param>
        /// 
        private void ParseDocumentNode(Panel panel) {

            if (!rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            rd.NextTag();
            ParsePanelNode(panel);

            // Llegeix el tag final
            //
            rd.NextTag();
        }

        /// <summary>
        /// Procesa el node 'panel'
        /// </summary>
        /// <param name="panel">El panell.</param>
        /// 
        private void ParsePanelNode(Panel panel) {

            if (!rd.IsStartTag("panel"))
                throw new InvalidDataException("Se esperaba <panel>");

            version = rd.AttributeAsInteger("version");

            rd.NextTag();
            ParsePanelElementsNode(panel);
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void ParsePanelElementsNode(Panel panel) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            // Obte els elements
            //
            List<PanelElement> elementList = new List<PanelElement>();
            while (rd.NextTag() && rd.IsStart) {
                switch (rd.TagName) {
                    case "place":
                        ParsePlaceNode(elementList);
                        break;

                    case "join":
                        ParseJoinNode(elementList);
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <place> o <join>");
                }
            }

            // Afegeix els elements al bloc
            //
            panel.AddElements(elementList);
        }

        /// <summary>
        /// Procesa el node 'place'
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParsePlaceNode(IList<PanelElement> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("place"))
                throw new InvalidDataException("Se esperaba <place>");

            // Obte els atributs del element
            //
            string fileName = rd.AttributeAsString("fileName");
            Point position = rd.AttributeAsPoint("position");
            Angle rotation = rd.AttributeAsAngle("rotation");

            // Crea l'element i l'afegeix a la llista
            //
            PlaceElement line = new PlaceElement(fileName, position, rotation);
            elementList.Add(line);

            // Llegeix el final del node
            //
            rd.NextTag();
        }

        /// <summary>
        /// Procesa un node 'join'.
        /// </summary>
        /// <param name="elementList">La llista d'elements.</param>
        /// 
        private void ParseJoinNode(IList<PanelElement> elementList) {

            // Comprova que el node sigui el correcte
            //
            if (!rd.IsStartTag("join"))
                throw new InvalidDataException("Se esperaba <join>");

            // Obte els atributs de l'element
            //
            Point position = rd.AttributeAsPoint("position");
            Angle rotation = rd.AttributeAsAngle("rotation");

            // Crea l'element i l'afegeix a la llista
            //
            JoinElement join = new JoinElement(position, rotation);
            elementList.Add(join);

            rd.NextTag();
        }

        /// <summary>
        /// Procesa un atribut de tipus 'Number'
        /// </summary>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="defValue">El valor per defecte.</param>
        /// <returns>El valor de l'atribut, o el valor per defecte si no existeix.</returns>
        /// 
        private int ParseNumberAttribute(string name, int defValue = 0) {

            if (rd.AttributeExists(name)) {
                double v = rd.AttributeAsDouble(name, defValue);
                return (int)(v * 1000000.0);
            }
            else
                return defValue;
        }
    }
}
