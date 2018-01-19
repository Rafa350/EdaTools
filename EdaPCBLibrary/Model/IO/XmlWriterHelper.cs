namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    /// <summary>
    /// Clase amb metodes d'extensio per la clase XmlWriter
    /// </summary>
    /// 
    public static class XmlWriterHelper {

        /// <summary>
        /// Escriu un atribut de tipus Point
        /// </summary>
        /// <param name="writer">El objecte escriptor xml.</param>
        /// <param name="name">Nom del atribut.</param>
        /// <param name="point">El valor del atribut.</param>
        /// 
        public static void WriteAttribute(this XmlWriter writer, string name, Point point) {

            writer.WriteAttributeString(
                name, 
                String.Format(
                    "{0}, {1}", 
                    XmlConvert.ToString(point.X), 
                    XmlConvert.ToString(point.Y)));
        }

        /// <summary>
        /// Escriu un atribut de tipus Size.
        /// </summary>
        /// <param name="writer">El objecte escriptor xml.</param>
        /// <param name="name">El nom del atribut.</param>
        /// <param name="size">El valor del atribut.</param>
        /// 
        public static void WriteAttribute(this XmlWriter writer, string name, Size size) {

            writer.WriteAttributeString(
                name, 
                String.Format(
                    "{0}, {1}", 
                    XmlConvert.ToString(size.Width), 
                    XmlConvert.ToString(size.Height)));
        }

        public static void WriteAttribute(this XmlWriter writer, string name, Color color) {

            writer.WriteAttributeString(
                name,
                String.Format("{0}, {1}, {2}, {3}", color.A, color.R, color.G, color.B));
        }

        public static void WriteAttribute(this XmlWriter writer, string name, double value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }
        
        public static void WriteAttribute(this XmlWriter writer, string name, int value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public static void WriteAttribute(this XmlWriter writer, string name, bool value) {

            writer.WriteAttributeString(name, value ? "true" : "false");
        }

        public static void WriteAttribute(this XmlWriter writer, string name, Layer layer) {

            if (layer != null)
                writer.WriteAttributeString(name, layer.Name);
        }

        public static void WriteAttribute(this XmlWriter writer, string name, IEnumerable<Layer> layers) {

            if (layers != null) {

                StringBuilder sb = new StringBuilder();

                bool first = true;
                foreach (Layer layer in layers) {
                    if (first)
                        first = false;
                    else
                        sb.Append(", ");
                    sb.Append(layer.Name);
                }
                writer.WriteAttributeString(name, sb.ToString());
            }
        }
    }
}
