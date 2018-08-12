namespace MikroPic.EdaTools.v1.Pcb.Model.IO.Infrastructure {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    
    /// <summary>
    /// Clase per lleigir un fitxer XML tag a tag de forma sequencial.
    /// </summary>
    /// 
    internal sealed class XmlTagReader {

        private const string schemaResourceName = "";

        private readonly XmlReader reader;
        private readonly IDictionary<string, string> attributes = new Dictionary<string, string>();
        private string tagName;
        private bool isEmpty;
        private bool isStart;

        public XmlTagReader(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            /*Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResourceName);
            if (resourceStream == null)
                throw new Exception(String.Format("No se encontro el recurso '{0}'", schemaResourceName));
            XmlSchema schema = XmlSchema.Read(resourceStream, null);
            */
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            settings.CloseInput = false;
            //settings.ValidationType = ValidationType.Schema;
            //settings.Schemas.Add(schema);
            settings.MaxCharactersInDocument = 1000000;
            settings.ConformanceLevel = ConformanceLevel.Document;

            reader = XmlTextReader.Create(stream, settings);
            isEmpty = false;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="reader">El lector XML.</param>
        /// 
        public XmlTagReader(XmlReader reader) {

            if (reader == null)
                throw new ArgumentNullException("reader");

            this.reader = reader;
            isEmpty = false;
        }

        /// <summary>
        /// Finalitza la lectura
        /// </summary>
        /// 
        public void Close() {

            reader.Close();
        }

        /// <summary>
        /// Avança al seguent tag
        /// </summary>
        /// <returns>True si hi ha tag. False en cas contrari.</returns>
        /// 
        public bool NextTag() {

            attributes.Clear();

            if (isEmpty) {
                isStart = false;
                isEmpty = false;
                return true;
            }
            else {
                do {
                    reader.Read();
                } while ((reader.NodeType != XmlNodeType.Element) &&
                         (reader.NodeType != XmlNodeType.EndElement) &&
                         (reader.NodeType != XmlNodeType.None));

                if (reader.NodeType == XmlNodeType.None)
                    return false;
                else {
                    tagName = reader.LocalName;
                    isStart = reader.NodeType == XmlNodeType.Element;
                    isEmpty = reader.IsEmptyElement;

                    if (reader.MoveToFirstAttribute()) {
                        do {
                            attributes.Add(reader.LocalName, reader.Value);
                        } while (reader.MoveToNextAttribute());
                        reader.MoveToElement();
                    }
                    return true;
                }
            }
        }

        /// <summary>
        /// Llegeig el contingut del tag actual.
        /// </summary>
        /// <returns>El contingut.</returns>
        /// 
        public string ReadTagContent() {

            reader.Read();
            if ((reader.NodeType != XmlNodeType.Text) &&
                (reader.NodeType != XmlNodeType.CDATA))
                throw new InvalidOperationException(
                    String.Format("El elemento '{0}' no tiene nodo de contenido", reader.Name));
                
            return reader.Value;
        }

        public override string ToString() {

            return String.Format(isStart ? "<{0}>" : "</{0}>", tagName);
        }

        /// <summary>
        /// Comprova si existeix un atribut.
        /// </summary>
        /// <param name="name">El nom de l'atribut.</param>
        /// <returns>True si existeix, false en cas contrari.</returns>
        /// 
        public bool AttributeExists(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return attributes.ContainsKey(name);
        }

        /// <summary>
        /// Obte el valor d'un atribut com string.
        /// </summary>
        /// <param name="name">Nom de l'atribut.</param>
        /// <param name="defValue">Valor per defecte.</param>
        /// <returns>El valor de l'atribut.</returns>
        /// 
        public string AttributeAsString(string name, string defValue = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (attributes.ContainsKey(name))
                return attributes[name];
            else
                return defValue;
        }

        /// <summary>
        /// Obte el valor de l'atribut en format array de strings.
        /// </summary>
        /// <param name="name">Nom de l'atribut.</param>
        /// <param name="defValue">Valor per defecte.</param>
        /// <returns>El valor de l'atribut.</returns>
        /// 
        public string[] AttributeAsStrings(string name, string[] defValue = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (attributes.ContainsKey(name))
                return attributes[name].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            else
                return defValue;
        }

        /// <summary>
        /// Obte el valor d'un atribut com a boolean.
        /// </summary>
        /// <param name="name">Nom de l'atribut.</param>
        /// <param name="defValue">Valor per defecte.</param>
        /// <returns>El valor de l'atribut.</returns>
        /// 
        public bool AttributeAsBoolean(string name, bool defValue = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (attributes.ContainsKey(name))
                return XmlConvert.ToBoolean(attributes[name]);
            else
                return defValue;
        }

        /// <summary>
        /// Obte el valor d'un atribut com a int
        /// </summary>
        /// <param name="name">Nom de l'atribut.</param>
        /// <param name="defValue">Valor per defecte.</param>
        /// <returns>El valor de l'atribut.</returns>
        /// 
        public int AttributeAsInteger(string name, int defValue = 0) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (attributes.ContainsKey(name))
                return XmlConvert.ToInt32(attributes[name]);
            else
                return defValue;
        }

        /// <summary>
        /// Obte el valor d'un atribut com a double
        /// </summary>
        /// <param name="name">Nom de l'atribut.</param>
        /// <param name="defValue">Valor per defecte.</param>
        /// <returns>El valor de l'atribut.</returns>
        /// 
        public double AttributeAsDouble(string name, double defValue = 0.0) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (attributes.ContainsKey(name))
                return XmlConvert.ToDouble(attributes[name]);
            else
                return defValue;
        }

        /// <summary>
        /// Obte el valor d'un atribut com a enumerador.
        /// </summary>
        /// <typeparam name="T">El tipus enumerador.</typeparam>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="defValue">Valor per defecte.</param>
        /// <returns>El valor de l'atribut.</returns>
        /// 
        public T AttributeAsEnum<T>(string name, T defValue) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (attributes.ContainsKey(name))
                return (T)Enum.Parse(typeof(T), attributes[name]);
            else
                return defValue;
        }

        public bool IsStartTag(string tagName) {

            if (String.IsNullOrEmpty(tagName))
                throw new ArgumentNullException("tagName");

            return (this.tagName == tagName) && isStart;
        }

        public bool IsStart {
            get {
                return isStart;
            }
        }

        public bool IsEnd {
            get {
                return !isStart;
            }
        }

        /// <summary>
        /// Obte el nom del tag actual.
        /// </summary>
        /// 
        public string TagName {
            get {
                return tagName;
            }
        }

        /// <summary>
        /// Indica si el tag actual conte atributs.
        /// </summary>
        /// 
        public bool HasAttributes {
            get {
                return attributes.Count > 0;
            }
        }
    }
}