﻿namespace MikroPic.EdaTools.v1.Panel.Model.IO {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Xml;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.Items;

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
        public Panel Read() {

            rd.NextTag();
            Panel project = ParseDocumentNode();

            return project;
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// <returns>L'objecte 'Project' obtingut.</returns>
        /// 
        private Panel ParseDocumentNode() {

            if (!rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            version = rd.AttributeAsInteger("version");

            rd.NextTag();
            Panel project = ParseProjectNode();

            rd.NextTag();
            if (!rd.IsEndTag("document"))
                throw new InvalidDataException("Se esperaba </document>");

            return project;
        }

        /// <summary>
        /// Procesa el node 'project'
        /// </summary>
        /// <returns>L'objecte 'Project' obtingut.</returns>
        /// 
        private Panel ParseProjectNode() {

            if (!rd.IsStartTag("project"))
                throw new InvalidDataException("Se esperaba <project>");

            Size size = XmlTypeParser.ParseSize(rd.AttributeAsString("size"));

            rd.NextTag();
            IEnumerable<PanelItem> elements = ParseItemsNode();

            rd.NextTag();
            if (!rd.IsEndTag("project"))
                throw new InvalidDataException("Se esperaba </project>");

            Panel project = new Panel();
            project.Size = size;
            project.AddItems(elements);

            return project;
        }

        /// <summary>
        /// Procesa el node 'items'
        /// </summary>
        /// <returns>La coleccio d'objectes 'ProjectItem' obtinguda.</returns>
        /// 
        private IEnumerable<PanelItem> ParseItemsNode() {

            if (!rd.IsStartTag("items"))
                throw new InvalidDataException("Se esperaba <items>");

            List<PanelItem> items = new List<PanelItem>();

            rd.NextTag();
            while (rd.IsStart) {
                switch (rd.TagName) {
                    case "board":
                        items.Add(ParseBoardNode());
                        break;

                    case "cut":
                        items.Add(ParseCutNode());
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <board> o <cut>");
                }
                rd.NextTag();
            }

            if (!rd.IsEndTag("items"))
                throw new InvalidDataException("Se esperaba </items>");

            return items;
        }

        /// <summary>
        /// Procesa el node 'board'
        /// </summary>
        /// <returns>L'objecte 'PcbItem' obtingut.</returns>
        /// 
        private PcbItem ParseBoardNode() {

            if (!rd.IsStartTag("board"))
                throw new InvalidDataException("Se esperaba <board>");

            string fileName = rd.AttributeAsString("file");
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Size size = rd.AttributeExists("size") ?
                XmlTypeParser.ParseSize(rd.AttributeAsString("size")) :
                new Size(0, 0);
            Angle rotation = rd.AttributeExists("rotation") ?
                XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation")) :
                Angle.Zero;

            rd.NextTag();
            if (!rd.IsEndTag("board"))
                throw new InvalidDataException("Se esperaba </board>");

            return new PcbItem(fileName, position, size, rotation);
        }

        /// <summary>
        /// Procesa un node 'cut'.
        /// </summary>
        /// <returns>L'objecte 'CutItem' obtingut.</returns>
        /// 
        private CutItem ParseCutNode() {

            if (!rd.IsStartTag("cut"))
                throw new InvalidDataException("Se esperaba <cut>");

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
            if (!rd.IsEndTag("cut"))
                throw new InvalidDataException("Se esperaba </cut>");

            return new CutItem(startPosition, endPosition, thickness, margin, cuts, cutSpacing, 
                holes, holeDiameter, holeSpacing);
        }
    }
}