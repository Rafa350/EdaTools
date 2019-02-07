namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

    using MikroPic.EdaTools.v1.Core.Model.Net;
    using MikroPic.EdaTools.v1.Core.Model.Net.Visitors;
    using System;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Clase per la escriptura de plaques en un stream.
    /// </summary>
    /// 
    public sealed class NetStreamWriter {

        private readonly Stream stream;

        private class Visitor : NetDefaultVisitor {

            private readonly XmlWriter writer;

            /// <summary>
            /// Constructor del objecte. Visita els objectes d'una netlist,
            /// per generar el stream de sortida.
            /// </summary>
            /// <param name="wr">Objecte per escriure el stream de sortida.</param>
            /// 
            public Visitor(XmlWriter writer) {

                this.writer = writer;
            }

            /// <summary>
            /// Visita un objecte 'Net'
            /// </summary>
            /// <param name="net">L'objecte a visitar.</param>
            /// 
            public override void Visit(Net net) {

                writer.WriteStartElement("netlist");

                if (net.HasParts) {
                    writer.WriteStartElement("parts");
                    foreach (var part in net.Parts)
                        part.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                if (net.HasSignals) {
                    writer.WriteStartElement("signals");
                    foreach (var signal in net.Signals)
                        signal.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte 'NetSignal'
            /// </summary>
            /// <param name="signal">L'objecte a visitar.</param>
            /// 
            public override void Visit(NetSignal signal) {

                writer.WriteStartElement("signal");
                writer.WriteAttributeString("name", signal.Name);

                if (signal.HasConnections) {
                    writer.WriteStartElement("connections");
                    foreach (var connection in signal.Connections)
                        connection.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte 'NetConnection'
            /// </summary>
            /// <param name="connection">L'objecte a visitar.</param>
            /// 
            public override void Visit(NetConnection connection) {

                writer.WriteStartElement("connection");
                writer.WriteAttributeString("part", connection.PartName);
                writer.WriteAttributeString("pad", connection.PadName);
                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte 'NetPart'
            /// </summary>
            /// <param name="part">L'objecte a visitar.</param>
            /// 
            public override void Visit(NetPart part) {

                writer.WriteStartElement("part");
                writer.WriteAttributeString("name", part.Name);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="stream">Stream de sortida.</param>
        /// 
        public NetStreamWriter(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanWrite)
                throw new InvalidOperationException("El stream no es de escritura.");

            this.stream = stream;
        }

        /// <summary>
        /// Escriu el netlist en el stream de sortida.
        /// </summary>
        /// <param name="net">El netlist.</param>
        /// 
        public void Write(Net net) {

            if (net == null)
                throw new ArgumentNullException("net");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.CloseOutput = true;

            using (XmlWriter writer = XmlWriter.Create(stream, settings)) {

                writer.WriteStartDocument();

                writer.WriteStartElement("document", "http://MikroPic.com/schemas/edatools/v1/XNET.xsd");
                writer.WriteAttributeString("version", "100");
                writer.WriteAttributeString("documentType", "netlist");

                INetVisitor visitor = new Visitor(writer);
                net.AcceptVisitor(visitor);

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }
    }
}
