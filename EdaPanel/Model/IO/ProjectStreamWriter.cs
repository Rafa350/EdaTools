namespace MikroPic.EdaTools.v1.Panel.Model.IO {

    using System;
    using System.IO;
    using System.Xml;
    using MikroPic.EdaTools.v1.Base.Xml;
    using MikroPic.EdaTools.v1.Panel.Model.Visitors;

    /// <summary>
    /// Clase per la escriptura de plaques en un stream.
    /// </summary>
    /// 
    public sealed class ProjectStreamWriter {

        private const int version = 211;
        private const string distanceUnits = "mm";
        private const string angleunits = "deg";

        private readonly Stream stream;

        private class Visitor : DefaultPanelVisitor {

            private readonly XmlWriter writer;

            /// <summary>
            /// Constructor del objecte. Visita els objectes d'un panel,
            /// per generar el stream de sortida.
            /// </summary>
            /// <param name="writer">Objecte per escriure el stream de sortida.</param>
            /// 
            public Visitor(XmlWriter writer) {

                this.writer = writer;
            }

            /// <summary>
            /// Visita un panell.
            /// </summary>
            /// <param name="panel">El panell a visitar.</param>
            /// 
            public override void Visit(Panel panel) {

                writer.WriteStartElement("panel");

                if (panel.HasItems) {
                    writer.WriteStartElement("elements");
                    foreach (var item in panel.Items)
                        item.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="stream">Stream de sortida.</param>
        /// 
        public ProjectStreamWriter(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new InvalidOperationException("El stream no es de escritura.");

            this.stream = stream;
        }

        /// <summary>
        /// Escriu la placa en el stream de sortida.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        public void Write(Panel panel) {

            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.CloseOutput = true;

            using (XmlWriter writer = XmlWriter.Create(stream, settings)) {
                writer.WriteStartDocument();

                writer.WriteStartElement("document", "http://MikroPic.com/schemas/edatools/v1/PanelDocument.xsd");

                writer.WriteAttributeInteger("version", version);
                writer.WriteAttributeString("documentType", "panel");
                writer.WriteAttributeString("distanceUnits", distanceUnits);
                writer.WriteAttributeString("angleUnits", angleunits);

                IPanelVisitor visitor = new Visitor(writer);
                panel.AcceptVisitor(visitor);

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }
    }
}
