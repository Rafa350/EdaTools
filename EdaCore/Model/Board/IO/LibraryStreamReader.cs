using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using MikroPic.EdaTools.v1.Base.Xml;

namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

    public sealed class LibraryStreamReader {

        private static readonly XmlSchemaSet _schemas;
        private readonly XmlReaderAdapter _rd;
        private Library _library;
        private int _version;

        /// <summary>
        /// Constructor estatic de la clase
        /// </summary>
        /// 
        static LibraryStreamReader() {

            _schemas = new XmlSchemaSet();

            string schemaResourceName = "MikroPic.EdaTools.v1.Core.Model.Board.IO.Schemas.XLIB.xsd";
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", schemaResourceName));
            XmlSchema schema = XmlSchema.Read(resourceStream, null);
            _schemas.Add(schema);

            _schemas.Compile();
        }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="stream">Stream de lectura.</param>
        /// 
        public LibraryStreamReader(Stream stream) {

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

        public Library Read() {

            _library = new Library("unnamed");

            _rd.NextTag();
            ParseDocumentNode();

            return _library;
        }

        /// <summary>
        /// Procesa el node 'document'
        /// </summary>
        /// 
        private void ParseDocumentNode() {

            if (!_rd.IsStartTag("document"))
                throw new InvalidDataException("Se esperaba <document>");

            _rd.NextTag();
            ParseLibraryNode();

            _rd.NextTag();
            if (!_rd.IsEndTag("document"))
                throw new InvalidDataException("Se esperaba </document>");
        }

        /// <summary>
        /// Procesa el node 'board'
        /// </summary>
        /// 
        private void ParseLibraryNode() {

            if (!_rd.IsStartTag("library"))
                throw new InvalidDataException("Se esperaba <library>");

            _version = _rd.AttributeAsInteger("version");

            _rd.NextTag();
            if (_rd.TagName == "components") {
                _library.AddComponents(ParseComponentsNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("library"))
                throw new InvalidDataException("Se esperaba </library>");
        }

        /// <summary>
        /// Procesa el node 'components'
        /// </summary>
        /// <returns>La llista d'objectes 'Component' obtinguda.</returns>
        /// 
        private IEnumerable<Component> ParseComponentsNode() {

            if (!_rd.IsStartTag("components"))
                throw new InvalidDataException("Se esperaba <components>");

            var components = new List<Component>();

            _rd.NextTag();
            while (_rd.IsStartTag("component")) {
                components.Add(ParseComponentNode());
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("components"))
                throw new InvalidDataException("Se esperaba </components>");

            return components;
        }

        /// <summary>
        /// Procesa el node 'component'.
        /// </summary>
        /// <returns>L'objecte 'Component' obtingut.</returns>
        /// 
        private Component ParseComponentNode() {

            if (!_rd.IsStartTag("component"))
                throw new InvalidDataException("Se esperaba <component>");

            string name = _rd.AttributeAsString("name");
            string description = _rd.AttributeAsString("description");

            var component = new Component(name);
            component.Description = description;

            _rd.NextTag();
            component.AddElements(ParseComponentElementsNode());
            _rd.NextTag();

            if (!_rd.IsEndTag("component"))
                throw new InvalidDataException("Se esperaba </component>");

            return component;
        }

        /// <summary>
        /// Procesa el node 'elements'
        /// </summary>
        /// <returns>La llista d'objectres 'Element' obtinguda.</returns>
        /// 
        private IEnumerable<Element> ParseComponentElementsNode() {

            if (!_rd.IsStartTag("elements"))
                throw new InvalidDataException("Se esperaba <elements>");

            List<Element> elements = new List<Element>();

            _rd.NextTag();
            while (_rd.IsStart) {
                switch (_rd.TagName) {
                    case "line":
                        elements.Add(ElementParser.Line(_rd));
                        break;

                    case "arc":
                        elements.Add(ElementParser.Arc(_rd));
                        break;

                    case "rectangle":
                        elements.Add(ElementParser.Rectangle(_rd));
                        break;

                    case "circle":
                        elements.Add(ElementParser.Circle(_rd));
                        break;

                    case "region":
                        elements.Add(ElementParser.Region(_rd));
                        break;

                    case "tpad":
                        elements.Add(ElementParser.TPad(_rd));
                        break;

                    case "spad":
                        elements.Add(ElementParser.SPad(_rd));
                        break;

                    case "hole":
                        elements.Add(ElementParser.Hole(_rd));
                        break;

                    case "text":
                        elements.Add(ElementParser.Text(_rd));
                        break;

                    default:
                        throw new InvalidDataException("Se esperaba <line>, <arc>, <rectangle>, <circle>, <tpad>, <spad>, <via>, <text>, <region> o <hole>");
                }
                _rd.NextTag();
            }

            if (!_rd.IsEndTag("elements"))
                throw new InvalidDataException("Se esperaba </elements>");

            return elements;
        }
    }
}
