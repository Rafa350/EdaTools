namespace Media.NetGui.v1.Localization.Providers {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using MikroPic.NetMVVMToolkit.v1.Utils;

    public sealed class FileTranslationProvider: ITranslationProvider {

        private readonly string fileName;
        private Dictionary<int, string> dictionary;
        private List<string> errors;

        public FileTranslationProvider(string fileName, bool registerErrors) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            this.fileName = fileName;
            if (registerErrors)
                errors = new List<string>();
        }

        private void LoadDictionary(string fileName) {

            CultureInfo ci = CultureInfo.CurrentUICulture;

            string path = FileLocator.LocateFile("ui", fileName, ci);

            XmlDocument doc = new XmlDocument();
            doc.Load(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None));

            dictionary = new Dictionary<int, string>();
            string xPath = String.Format("resources/strings[@language='{0}']/string", ci.Name);
            foreach (XmlNode stringNode in doc.SelectNodes(xPath)) {
                try {
                    XmlAttribute attr = stringNode.Attributes["hash"];
                    if (attr != null) {
                        int hash = Int32.Parse(attr.Value);
                        string value = stringNode.Attributes["value"].Value;
                        dictionary.Add(hash, value);
                    }
                    else {
                        attr = stringNode.Attributes["key"];
                        if (attr != null) {
                            string key = attr.Value;
                            string value = stringNode.Attributes["value"].Value;
                            dictionary.Add(key.GetHashCode(), value);
                        }
                    }
                }
                catch {
                    // Descarta els errors. Ignora les entrades erronies.
                }
            }
        }

        public object Translate(string key) {

            if (dictionary == null)
                LoadDictionary(fileName);

            string text;
            if (dictionary.TryGetValue(key.GetHashCode(), out text))
                return text;
            else {
                if (errors != null) {
                    if (!errors.Contains(key))
                        errors.Add(key);
                }
                return key;
            }
        }

        public void SaveDictionary(string fileName, bool includeErrors) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (dictionary != null) {

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "    ";
                XmlWriter writer = XmlWriter.Create(new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None), settings);
                try {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("resources");
                    writer.WriteStartElement("strings");
                    writer.WriteAttributeString("language", CultureInfo.CurrentUICulture.Name);

                    foreach (KeyValuePair<int, string> kv in dictionary) {
                        writer.WriteStartElement("string");
                        writer.WriteAttributeString("hash", kv.Key.ToString());
                        writer.WriteAttributeString("value", kv.Value);
                        writer.WriteEndElement();
                    }

                    if (includeErrors & (errors != null)) {
                        foreach (string error in errors) {
                            writer.WriteStartElement("string");
                            writer.WriteAttributeString("key", error);
                            writer.WriteAttributeString("value", String.Format("{0}...", error.Substring(0, Math.Min(10, error.Length))));
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                finally {
                    writer.Close();
                }
            }
        }

        public IEnumerable<CultureInfo> Languages {
            get {
                return new CultureInfo[] { CultureInfo.CurrentUICulture };
            }
        }

        public IEnumerable<string> Errors {
            get {
                return errors != null ? errors : null;
            }
        }
    }
}
