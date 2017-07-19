namespace Eda.PCBTool {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using MikroPic.EdaTools.v1.Model;
    using Eda.PCBTool.XmlHelper;
    
    public sealed class EagleExportVisitor: DefaultVisitor {

        private const string headerFileName = @"..\..\..\Data\EagleLibraryHeaderData.txt";
        private const string footerFileName = @"..\..\..\Data\EagleLibraryFooterData.txt";
        
        private readonly XmlWriter writer;
        private Layer currentLayer;
        private readonly Dictionary<string, string> layerMap = new Dictionary<string, string>();

        public EagleExportVisitor(XmlWriter writer) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;

            layerMap.Add("signal", "1");
            layerMap.Add("place", "21");
            layerMap.Add("stop", null);
            layerMap.Add("cream", null);
        }

        public override void Visit(Component package) {

            WriteFileHeader();

            writer.WriteStartElement("library");
            writer.WriteStartElement("packages");
            writer.WriteStartElement("package");
            writer.WriteAttributeString("name", package.Name);
            writer.WriteStartElement("description");
            writer.WriteValue(package.Description);
            writer.WriteEndElement();
            base.Visit(package);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            WriteFileFooter();
        }

        public override void Visit(Layer layer) {

            if (layerMap[layer.Name] != null) {
                currentLayer = layer;
                base.Visit(layer);
            }
        }

        public override void Visit(SmdPad pad) {

            writer.WriteStartElement("smd");
            writer.WriteAttributeString("name", pad.Name);            
            writer.WriteAttributeDouble("x", pad.XPos);
            writer.WriteAttributeDouble("y", pad.YPos);            
            writer.WriteAttributeDouble("dx", pad.XSize);
            writer.WriteAttributeDouble("dy", pad.YSize);       
            if (pad.Rotate != 0)
                writer.WriteAttributeRotate("rot", pad.Rotate);           
            if (pad.Radius != 0)
                writer.WriteAttributeDouble("roundness", pad.Radius * 100.0);            
            writer.WriteAttributeString("layer", layerMap[currentLayer.Name]);
            writer.WriteEndElement();
        }

        public override void Visit(Line line) {

            writer.WriteStartElement("wire");
            writer.WriteAttributeDouble("x1", line.XStart);
            writer.WriteAttributeDouble("y1", line.YStart);
            writer.WriteAttributeDouble("x2", line.XEnd);
            writer.WriteAttributeDouble("y2", line.YEnd);
            writer.WriteAttributeDouble("width", line.Thickness);
            writer.WriteAttributeString("layer", layerMap[currentLayer.Name]);
            writer.WriteEndElement();
        }
        
        public override void Visit(Arc arc) {

            double sa = arc.StartAngle * Math.PI / 180;
            double sdx = arc.Radius * Math.Cos(sa);
            double sdy = arc.Radius * Math.Sin(sa);

            double ea = arc.EndAngle * Math.PI / 180;
            double edx = arc.Radius * Math.Cos(ea);
            double edy = arc.Radius * Math.Sin(ea);

            writer.WriteStartElement("wire");
            writer.WriteAttributeDouble("x1", arc.XCenter + sdx);
            writer.WriteAttributeDouble("y1", arc.YCenter + sdy);
            writer.WriteAttributeDouble("x2", arc.XCenter + edx);
            writer.WriteAttributeDouble("y2", arc.YCenter + edy);
            writer.WriteAttributeDouble("width", arc.Thickness);
            writer.WriteAttributeDouble("curve", arc.StartAngle - arc.EndAngle);
            writer.WriteAttributeString("layer", layerMap[currentLayer.Name]);
            writer.WriteEndElement();
        }

        public override void Visit(Text text) {

            writer.WriteStartElement("text");
            writer.WriteEndElement();
        }

        private void WriteFileHeader() {

            string data = File.ReadAllText(headerFileName);
            writer.WriteRaw(data);
        }

        private void WriteFileFooter() {

            string data = File.ReadAllText(footerFileName);
            writer.WriteRaw(data);
        }
    }
}
