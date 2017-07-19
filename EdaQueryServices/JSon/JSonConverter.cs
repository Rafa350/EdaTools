namespace MikroPic.EdaTools.v1.JSon {

    using System;
    using System.Xml;
    using MikroPic.EdaTools.v1.JSon.Model;

    public static class JSonConverter {

        private sealed class Visitor: JSonDefaultVisitor {

            private readonly XmlWriter writer;

            public Visitor(XmlWriter writer) {

                this.writer = writer;
            }

            public override void Visit(JSonObject jsonObject) {

                writer.WriteStartElement("object");
                if (!String.IsNullOrEmpty(jsonObject.ClassName))
                    writer.WriteAttributeString("className", jsonObject.ClassName);

                foreach (JSonProperty jsonProperty in jsonObject.Properties)
                    jsonProperty.AcceptVisitor(this);

                writer.WriteEndElement();
            }

            public override void Visit(JSonArray jsonArray) {

                writer.WriteStartElement("array");
                foreach (JSonValue jsonValue in jsonArray.Values)
                    jsonValue.AcceptVisitor(this);
                writer.WriteEndElement();
            }

            public override void Visit(JSonProperty jsonProperty) {

                writer.WriteStartElement("property");
                writer.WriteAttributeString("name", jsonProperty.Name);
                if (jsonProperty.Value != null)
                    jsonProperty.Value.AcceptVisitor(this);
                writer.WriteEndElement();
            }

            public override void Visit(JSonString jsonString) {

                writer.WriteStartElement("value");
                writer.WriteAttributeString("type", "string");
                writer.WriteAttributeString("value", jsonString.Value);
                writer.WriteEndElement();
            }

            public override void Visit(JSonInteger jsonInteger) {

                writer.WriteStartElement("value");
                writer.WriteAttributeString("type", "integer");
                writer.WriteAttributeString("value", XmlConvert.ToString(jsonInteger.Value));
                writer.WriteEndElement();
            }

            public override void Visit(JSonReal jsonReal) {

                writer.WriteStartElement("value");
                writer.WriteAttributeString("type", "real");
                writer.WriteAttributeString("value", XmlConvert.ToString(jsonReal.Value));
                writer.WriteEndElement();
            }

            public override void Visit(JSonBoolean jsonBoolean) {

                writer.WriteStartElement("value");
                writer.WriteAttributeString("type", "boolean");
                writer.WriteAttributeString("value", XmlConvert.ToString(jsonBoolean.Value));
                writer.WriteEndElement();
            }
        }

        public static void ConverToXml(XmlWriter writer, JSonObject root) {

            writer.WriteStartDocument();

            Visitor visitor = new Visitor(writer);
            root.AcceptVisitor(visitor);

            writer.WriteEndDocument();
        }
    }
}
