namespace MikroPic.EdaTools.v1.Panel.Model.IO {

    using MikroPic.EdaTools.v1.Panel.Model.Visitors;
    using System;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Clase per la escriptura de plaques en un stream.
    /// </summary>
    /// 
    public sealed class PanelStreamWriter {

        private Stream stream;

        private class Visitor: DefaultVisitor {

            private readonly XmlWriter writer;
            private readonly Panel panel;

            /// <summary>
            /// Constructor del objecte. Visita els objectes d'un panel,
            /// per generar el stream de sortida.
            /// </summary>
            /// <param name="panel">El panel a visitar.</param>
            /// <param name="writer">Objecte per escriure el stream de sortida.</param>
            /// 
            public Visitor(Panel panel, XmlWriter writer) {

                this.panel = panel;
                this.writer = writer;
            }

            /// <summary>
            /// Executa el visitador.
            /// </summary>
            /// 
            public override void Run() {

                panel.AcceptVisitor(this);
            }

            /// <summary>
            /// Visita una placa.
            /// </summary>
            /// <param name="board">La placa a visitar.</param>
            /// 
            public override void Visit(Panel panel) {

                writer.WriteStartElement("panel");

                if (panel.HasElements) {
                    writer.WriteStartElement("elements");
                    foreach (var element in panel.Elements)
                        element.AcceptVisitor(this);
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
        public PanelStreamWriter(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

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
                throw new ArgumentNullException("panel");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.CloseOutput = true;

            using (XmlWriter writer = XmlWriter.Create(stream, settings)) {
                writer.WriteStartDocument();

                writer.WriteStartElement("document", "http://MikroPic.com/schemas/edatools/v1/XBRD.xsd");

                writer.WriteAttributeString("version", "211");
                writer.WriteAttributeString("documentType", "panel");
                writer.WriteAttributeString("distanceUnits", "mm");
                writer.WriteAttributeString("angleUnits", "deg");

                IVisitor visitor = new Visitor(panel, writer);
                visitor.Run();

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }
    }
}
