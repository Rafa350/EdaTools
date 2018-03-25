namespace MikroPic.NetMVVMToolkit.v1.Menus {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Markup;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Xsl;
    using MikroPic.NetMVVMToolkit.v1.Infrastructure;
    using MikroPic.NetMVVMToolkit.v1.Utils;

    /// <summary>
    /// Factoria de menus.
    /// </summary>
    public class MenuFactory: IMenuFactory {

        private static MenuFactory instance;
        private readonly XmlSchema schema;
        private readonly XslCompiledTransform transform;
        private readonly Dictionary<int, FrameworkElement> cache = new Dictionary<int, FrameworkElement>();

        private const string schemaResourceName = "MikroPic.NetMVVMToolkit.v1.Menus.XmlResources.MenuSchema.xsd";
        private const string schemaFileName = @"MenuSchema.xsd";

        private const string transformResourceName = "MikroPic.NetMVVMToolkit.v1.Menus.XmlResources.MenuTemplate.xslt";
        private const string transformFileName = @"MenuTemplate.xslt";

        private MenuFactory() {

            schema = XmlUtils.GetSchemaFromResource(schemaResourceName, false);
            if (File.Exists(transformFileName))
                transform = XmlUtils.GetTransformFromFile(transformFileName);
            else
                transform = XmlUtils.GetTransformFromResource(transformResourceName);
        }

        /// <summary>
        /// Obte un menu a partir del seu descriptor.
        /// </summary>
        /// <param name="path">Nom del fitxer que conte el descriptor del menu.</param>
        /// <returns>El menu</returns>
        public FrameworkElement CreateMenu(string fileName, string menuName) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (String.IsNullOrEmpty(menuName))
                throw new ArgumentNullException("menuName");

            CultureInfo ci = CultureInfo.CurrentUICulture;
            string path = FileLocator.LocateFile("ui", fileName, ci);
            
            FrameworkElement menu;

            int fileHash = String.Format("{0}${1}", path.GetHashCode(), menuName).GetHashCode();
            if (!cache.TryGetValue(fileHash, out menu)) {

                using (Stream stream = new MemoryStream()) {

                    using (XmlReader rd = XmlUtils.GetReader(path, schema)) {
                        using (XmlWriter wr = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true, IndentChars = "    " })) {
                            XsltArgumentList arguments = new XsltArgumentList();
                            arguments.AddParam("language", "", ci.Name);
                            arguments.AddParam("name", "", menuName);
                            transform.Transform(rd, arguments, wr);
                        }
                    }

                    stream.Position = 0;
                    StreamReader reader = new StreamReader(stream);
                    string text = reader.ReadToEnd();

                    stream.Position = 0;
                    menu = (FrameworkElement) XamlReader.Load(stream, new ParserContext());

                    cache.Add(fileHash, menu);
                }
            }

            return menu;
        }

        /// <summary>
        /// Obte una instancia de la clase.
        /// </summary>
        public static IMenuFactory Instance {
            get {
                if (instance == null)
                    instance = new MenuFactory();
                return instance;
            }
        }
    }
}