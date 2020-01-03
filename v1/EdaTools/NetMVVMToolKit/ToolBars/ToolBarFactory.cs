namespace MikroPic.NetMVVMToolkit.v1.ToolBars {

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
    /// Factoria de barras d'eines.
    /// </summary>
    public sealed class ToolBarFactory: IToolBarFactory {

        private static ToolBarFactory instance;
        private readonly Dictionary<int, FrameworkElement> cache = new Dictionary<int, FrameworkElement>();
        private readonly XmlSchema schema;
        private readonly XslCompiledTransform transform;

        private const string schemaResourceName = "MikroPic.NetMVVMToolkit.v1.ToolBars.XmlResources.ToolBarSchema.xsd";
        private const string schemaFileName = @"ToolBarSchema.xsd";

        private const string transformResourceName = "MikroPic.NetMVVMToolkit.v1.ToolBars.XmlResources.ToolBarTemplate.xslt";
        private const string transformFileName = @"ToolBarTemplate.xslt";

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        private ToolBarFactory() {

            schema = XmlUtils.GetSchemaFromResource(schemaResourceName, false);
            if (File.Exists(transformFileName))
                transform = XmlUtils.GetTransformFromFile(transformFileName);
            else
                transform = XmlUtils.GetTransformFromResource(transformResourceName);
        }

        /// <summary>
        /// Obte una barra d'eines a partir del seu descriptor.
        /// </summary>
        /// <param name="path">Nom del fitxer.</param>
        /// <returns>La barra d'eines creada.</returns>
        public FrameworkElement CreateToolBar(string fileName, string toolbarName) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            CultureInfo ci = CultureInfo.CurrentUICulture;
            string path = FileLocator.LocateFile("ui", fileName, ci);

            FrameworkElement toolbar;

            int fileHash = String.Format("{0}${1}", path.GetHashCode(), toolbarName).GetHashCode();
            if (!cache.TryGetValue(fileHash, out toolbar)) {

                using (Stream stream = new MemoryStream()) {

                    using (XmlReader rd = XmlUtils.GetReader(path, schema)) {
                        using (XmlWriter wr = XmlWriter.Create(stream)) {
                            XsltArgumentList arguments = new XsltArgumentList();
                            arguments.AddParam("language", "", ci.Name);
                            arguments.AddParam("name", "", toolbarName);
                            transform.Transform(rd, null, wr);
                        }
                    }

                    stream.Position = 0;
                    toolbar = (FrameworkElement) XamlReader.Load(stream, new ParserContext());

                    cache.Add(fileHash, toolbar);
                }
            }

            return toolbar;
        }

        /// <summary>
        /// Obte una instancia de la clase.
        /// </summary>
        public static ToolBarFactory Instance {
            get {
                if (instance == null)
                    instance = new ToolBarFactory();
                return instance;
            }
        }
    }
}
