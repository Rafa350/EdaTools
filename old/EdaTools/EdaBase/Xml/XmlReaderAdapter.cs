namespace MikroPic.EdaTools.v1.Base.Xml {

    using System;
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Clase per lleigir un fitxer XML tag a tag de forma sequencial.
    /// </summary>
    /// 
    public sealed class XmlReaderAdapter {

        private readonly XmlReader reader;
        private readonly IDictionary<string, string> attributes = new Dictionary<string, string>();
        private string tagName;
        private bool isEmpty;
        private bool isStart;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="reader">El lector XML.</param>
        /// 
        public XmlReaderAdapter(XmlReader reader) {

            if (reader == null)
                throw new ArgumentNullException("reader");

            this.reader = reader;
            isEmpty = false;
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
                    try {
                        reader.Read();
                    }
                    catch (Exception e) {
                        throw new Exception(
                           String.Format("Error en el tag '<{0}>'.", tagName), e);
                    }
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
        public IEnumerable<string> AttributeAsStrings(string name, string[] defValue = null) {

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
                return (T)Enum.Parse(typeof(T), attributes[name], true);
            else
                return defValue;
        }

        public bool IsStartTag(string tagName) {

            if (String.IsNullOrEmpty(tagName))
                throw new ArgumentNullException("tagName");

            return (this.tagName == tagName) && isStart;
        }

        public bool IsEndTag(string tagName) {

            if (String.IsNullOrEmpty(tagName))
                throw new ArgumentNullException("tagName");

            return (this.tagName == tagName) && !isStart;
        }

        public bool IsStart {
            get {
                return isStart;
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