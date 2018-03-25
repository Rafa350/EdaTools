namespace MikroPic.NetMVVMToolkit.v1.WindowState.Repository {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Xml;

    public sealed class WindowStateRepository: IWindowStateRepository {

        private const string repositoryFolder = @"MikroPic\NetMVVMToolkit\v1.0\WindowState";
        private const string repositoryExtension = ".xml";
        private const int version = 3;
        private readonly CultureInfo ci = CultureInfo.InvariantCulture;

        private string repositoryFileName;

        private Dictionary<string, WindowStateInfo> repository = new Dictionary<string, WindowStateInfo>();

        public void Load() {

            LoadFromFile(GetRepositoryFileName());
        }

        public void Save() {

            SaveToFile(GetRepositoryFileName());
        }

        public void Set(Window window, WindowStateInfo info) {

            if (window == null)
                throw new ArgumentNullException("window");

            if (info == null)
                throw new ArgumentNullException("info");

            string key = GetWindowKey(window);

            repository.Add(key, info);
        }

        public WindowStateInfo Get(Window window) {

            string key = GetWindowKey(window);

            WindowStateInfo info;
            if (!repository.TryGetValue(key, out info)) {
                info = new WindowStateInfo();
                repository.Add(key, info);
            }
            return info;
        }

        private string GetWindowKey(Window window) {

            return window.GetType().ToString();
        }

        private string GetRepositoryFileName() {

            if (String.IsNullOrEmpty(repositoryFileName)) {

                Assembly assembly = Assembly.GetEntryAssembly();
                string version = assembly.GetName().Version.ToString();
                string appFileName = assembly.Location;
                StringBuilder sb = new StringBuilder()
                    .Append(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
                    .Append(Path.DirectorySeparatorChar)
                    .Append(repositoryFolder)
                    .Append(Path.DirectorySeparatorChar)
                    .Append(Path.GetFileNameWithoutExtension(appFileName))
                    .Append('_')
                    .Append(version.GetHashCode().ToString("X8"))
                    .Append(repositoryExtension);
                repositoryFileName = sb.ToString();
            }

            return repositoryFileName;
        }

        private void LoadFromFile(string fileName) {

            try {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);

                XmlNode windowStateNode = doc.SelectSingleNode("windowState");

                foreach (XmlNode windowNode in windowStateNode.SelectNodes("window")) {

                    WindowStateInfo info = new WindowStateInfo();

                    string key = windowNode.Attributes["key"].Value;

                    // Procesa el node 'layout'
                    //
                    Size size = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
                    string xPath = String.Format(CultureInfo.InvariantCulture, "layouts/layout[@screen='{0}']", size);
                    XmlNode layoutNode = windowNode.SelectSingleNode(xPath);
                    if (layoutNode != null) {
                        info.Layout.Bounds = Rect.Parse(layoutNode.Attributes["bounds"].Value);
                        info.Layout.State = (WindowState) Enum.Parse(typeof(WindowState), layoutNode.Attributes["state"].Value);
                    }

                    // Procesa el node 'data'
                    //
                    try {
                        foreach (XmlNode valueNode in windowNode.SelectNodes("data/value")) {
                            string name = valueNode.Attributes["name"].Value;
                            Type type = Type.GetType(valueNode.Attributes["type"].Value);
                            info.Data[name] = Convert.ChangeType(valueNode.InnerText, type, ci);
                        }
                    }
                    catch {
                        // Ignora els errors dels nodes 'value'
                    }

                    repository.Add(key, info);
                }
            }
            catch {
                // Ignora els errors i continua. No es tan greu que no carregi
                // la posicio de les finestres
            }
        }

        private void SaveToFile(string fileName) {

            XmlDocument doc = new XmlDocument();

            // Crea la carpeta si no existeix
            //
            string path = Path.GetDirectoryName(Path.GetFullPath(fileName));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Llegeix el repositori original
            //
            if (File.Exists(fileName))
                doc.Load(fileName);

            // Actualitza el repository
            //
            foreach (KeyValuePair<string, WindowStateInfo> kv in repository) {

                string key = kv.Key;
                WindowStateInfo info = kv.Value;

                XmlElement windowStateNode = (XmlElement) doc.SelectSingleNode("windowState");
                if (windowStateNode == null) {
                    windowStateNode = doc.CreateElement("windowState");
                    doc.AppendChild(windowStateNode);
                }
                windowStateNode.SetAttribute("version", "3");

                XmlElement windowNode = (XmlElement) windowStateNode.SelectSingleNode(String.Format("window[@key='{0}']", key));
                if (windowNode == null) {
                    windowNode = doc.CreateElement("window");
                    windowStateNode.AppendChild(windowNode);
                }
                windowNode.SetAttribute("key", key); 

                // Actualitza el 'layout'
                //
                XmlElement layoutsNode = (XmlElement) windowNode.SelectSingleNode("layouts");
                if (layoutsNode == null) {
                    layoutsNode = doc.CreateElement("layouts");
                    windowNode.AppendChild(layoutsNode);
                }

                string screen = Convert.ToString(new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight), ci);
                XmlElement layoutNode = (XmlElement) layoutsNode.SelectSingleNode(String.Format("layout[@screen='{0}']", screen));
                if (layoutNode == null) {
                    layoutNode = doc.CreateElement("layout");
                    layoutsNode.AppendChild(layoutNode);
                }
                layoutNode.SetAttribute("screen", screen);
                layoutNode.SetAttribute("bounds", Convert.ToString(info.Layout.Bounds, ci));
                layoutNode.SetAttribute("state", Convert.ToString(info.Layout.State, ci));

                // Actualitza 'data'
                //
                XmlElement dataNode = (XmlElement) windowNode.SelectSingleNode("data");
                if (dataNode == null) {
                    dataNode = doc.CreateElement("data");
                    windowNode.AppendChild(dataNode);
                }

                if (info.Data.Count > 0) {
                    foreach (string name in info.Data.Names) {
                        XmlElement valueNode = (XmlElement) dataNode.SelectSingleNode(String.Format("value[@name='{0}']", name));

                        object value = info.Data[name];

                        if (value == null) {
                            if (valueNode != null)
                                dataNode.RemoveChild(valueNode);
                        }
                        else {
                            if (valueNode == null) {
                                valueNode = doc.CreateElement("value");
                                dataNode.AppendChild(valueNode);
                            }
                            valueNode.SetAttribute("name", name);
                            valueNode.SetAttribute("type", Convert.ToString(value.GetType(), ci));
                            valueNode.SetAttribute("value", Convert.ToString(value, ci));
                        }
                    }
                }
            }

            // Salva el repositori actualitzat
            //
            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);

            XmlWriterSettings settings = new XmlWriterSettings {
                Indent = true,
                IndentChars = "    ",
                CloseOutput = true
            };

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
                doc.Save(writer);
        }
    }
}