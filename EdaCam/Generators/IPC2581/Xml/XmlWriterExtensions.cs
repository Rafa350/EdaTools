using System.Collections.Generic;
using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Xml;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Xml {

    internal static class XmlWriterExtensions {

        public static void WriteAttributeAngle(this XmlWriter writer, string name, EdaAngle angle) {

            double value = angle.AsDegrees;
            if (value < 0)
                value = 360.0 + value;
            writer.WriteAttributeDouble(name, value);
        }

        public static void WritePointElement(this XmlWriter writer, string name, EdaPoint point, double scale) {

            writer.WriteStartElement(name);
            writer.WriteAttributeDouble("x", point.X / scale);
            writer.WriteAttributeDouble("y", point.Y / scale);
            writer.WriteEndElement();
        }

        public static void WritePolygonElement(this XmlWriter writer, EdaPolygon polygon, int fillDescId, double scale) {

            void WritePolygonPoints(IEnumerable<EdaPoint> points) {

                bool first = true;
                EdaPoint firstPoint = default;
                foreach (var point in points) {
                    if (first) {
                        first = false;
                        firstPoint = point;
                        writer.WritePointElement("PolyBegin", point, scale);
                    }
                    else
                        writer.WritePointElement("PolyStepSegment", point, scale);
                }
                writer.WritePointElement("PolyStepSegment", firstPoint, scale);
            }

            writer.WriteStartElement("Polygon");
            WritePolygonPoints(polygon.Outline);
            if (fillDescId != -1) {
                writer.WriteStartElement("FillDescRef");
                writer.WriteAttributeInteger("id", fillDescId);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // Polygon

            if (polygon.HasHoles) {
                foreach (var hole in polygon.Holes) {
                    writer.WriteStartElement("Cutout");
                    WritePolygonPoints(hole);
                    writer.WriteEndElement(); // Cutout
                }
            }
        }
    }
}
