namespace MikroPic.EdaTools.v1.Extractor {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using System;
    using System.IO;
    using System.Xml;

    public sealed class PartExtractor {

        private readonly Board board;
        private string ignoreAttributeName = "PARTLIST-IGNORE";

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        public PartExtractor(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }

        /// <summary>
        /// Genera les dades.
        /// </summary>
        /// <param name="writer">Es escriptor de sortida.</param>
        /// 
        public void Extract(TextWriter writer) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            XmlWriterSettings wrSettings = new XmlWriterSettings();
            wrSettings.Indent = true;
            wrSettings.IndentChars = "    ";
            wrSettings.CloseOutput = true;
            using (XmlWriter wr = XmlWriter.Create(writer, wrSettings)) {

                wr.WriteStartDocument();
                wr.WriteStartElement("board");
                wr.WriteAttributeString("units", "mm");
                wr.WriteStartElement("parts");

                foreach (Part part in board.Parts) {

                    if (part.GetAttribute(ignoreAttributeName) == null) {

                        wr.WriteStartElement("part");
                        wr.WriteAttributeString("name", part.Name);
                        wr.WriteAttributeString("position", FormatPoint(part.Position));
                        wr.WriteAttributeString("rotation", FormatAngle(part.Rotation));
                        wr.WriteAttributeString("flip", part.Flip.ToString());

                        bool hasAttributes = false;
                        foreach (PartAttribute attribute in part.Attributes) {
                            if (!hasAttributes) {
                                hasAttributes = true;
                                wr.WriteStartElement("attributes");
                            }
                            wr.WriteStartElement("attribute");
                            wr.WriteAttributeString("name", attribute.Name);
                            if (attribute.Value != null)
                                wr.WriteAttributeString("value", attribute.Value);
                            wr.WriteEndElement();
                        }
                        if (hasAttributes)
                            wr.WriteEndElement();

                        wr.WriteEndElement();
                    }
                }

                wr.WriteEndElement();
                wr.WriteEndElement();
                wr.WriteEndDocument();
            }
        }

        /// <summary>
        /// Formateja una dimensio en mm
        /// </summary>
        /// <param name="value">Valor de la dimensio.</param>
        /// <returns>El valor formatejat.</returns>
        /// 
        private static string FormatNumber(int value) {

            return XmlConvert.ToString(value / 1000000.0);
        }

        /// <summary>
        /// Formateja un punt en mm
        /// </summary>
        /// <param name="point">El valor del punt.</param>
        /// <returns>El valor formatejat.</returns>
        /// 
        private static string FormatPoint(Point point) {

            return String.Format(
                "{0}, {1}",
                XmlConvert.ToString(point.X / 1000000.0),
                XmlConvert.ToString(point.Y / 1000000.0));
        }

        /// <summary>
        /// Formateja un angle en graus
        /// </summary>
        /// <param name="angle">El valor del angle</param>
        /// <returns>El angle formatejat.</returns>
        /// 
        private static string FormatAngle(Angle angle) {

            return XmlConvert.ToString(angle.Value / 100.0);
        }
    }
}
