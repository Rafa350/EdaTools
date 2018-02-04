namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
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
        /// Escriu un atribut de tipus 'Point'
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
        /// Escriu un atribut de tipus 'Size'.
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

        /// <summary>
        /// Escriu un atribut de tipus 'Color'
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="name"></param>
        /// <param name="color"></param>
        public static void WriteAttribute(this XmlWriter writer, string name, Color color) {

            writer.WriteAttributeString(
                name,
                String.Format(
                    "{0}, {1}, {2}, {3}", 
                    XmlConvert.ToString(color.A),
                    XmlConvert.ToString(color.R),
                    XmlConvert.ToString(color.G),
                    XmlConvert.ToString(color.B)));
        }

        /// <summary>
        /// Escriu un atribut de tipus 'Angle'.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="name"></param>
        /// <param name="angle"></param>
        public static void WriteAttribute(this XmlWriter writer, string name, Angle angle) {

            writer.WriteAttributeString(
                name,
                XmlConvert.ToString(angle.Degrees));
        }

        public static void WriteAttribute(this XmlWriter writer, string name, double value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }
        
        public static void WriteAttribute(this XmlWriter writer, string name, int value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public static void WriteAttribute(this XmlWriter writer, string name, bool value) {

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public static void WriteAttribute(this XmlWriter writer, string name, string value) {

            writer.WriteAttributeString(name, value);
        }

        public static void WriteAttribute(this XmlWriter writer, string name, string[] values) {

            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (string value in values) {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(value);
            }

            writer.WriteAttributeString(name, sb.ToString());
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
