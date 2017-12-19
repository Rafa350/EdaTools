namespace MikroPic.EdaTools.v1.Pcb.Model.IO {

    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Collections;

    public static class XmlWriterHelper {

        public static void WriteAttribute(this XmlWriter writer, string name, Point point) {

            writer.WriteAttributeString(
                name, 
                String.Format("{0}, {1}", XmlConvert.ToString(point.X), XmlConvert.ToString(point.Y)));
        }

        public static void WriteAttribute(this XmlWriter writer, string name, Size size) {

            writer.WriteAttributeString(
                name, 
                String.Format("{0}, {1}", XmlConvert.ToString(size.Width), XmlConvert.ToString(size.Height)));
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

            writer.WriteAttributeString(name, XmlConvert.ToString(value));
        }

        public static void WriteAttribute(this XmlWriter writer, string name, Layer layer) {

            if (layer != null)
                writer.WriteAttributeString(name, layer.Name);
        }

        public static void WriteAttribute(this XmlWriter writer, string name, LayerCollection layers) {

            if (layers != null && !layers.IsEmpty) {

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
