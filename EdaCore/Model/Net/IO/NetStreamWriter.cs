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
            /// Constructor del objecte. Visita els objectes d'una placa,
            /// per generar el stream de sortida.
            /// </summary>
            /// <param name="wr">Objecte per escriure el stream de sortida.</param>
            /// 
            public Visitor(XmlWriter writer) {

                this.writer = writer;
            }

            public override void Visit(Net net) {

                writer.WriteStartElement("netlist");

                if (net.HasSignals) {
                    writer.WriteStartElement("signals");
                    foreach (var signal in net.Signals)
                        signal.AcceptVisitor(this);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'NetSignal'
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

            public override void Visit(NetConnection pin) {

                writer.WriteStartElement("connection");
                writer.WriteAttributeString("part", pin.PartName);
                writer.WriteAttributeString("pad", pin.PadName);
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
        /// Escriu la placa en el stream de sortida.
        /// </summary>
        /// <param name="board">La placa.</param>
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
