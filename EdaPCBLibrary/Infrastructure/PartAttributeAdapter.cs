namespace MikroPic.EdaTools.v1.Pcb.Infrastructure {

    using System;
    using System.Windows;
    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class PartAttributeAdapter {

        private readonly PartAttribute attribute;
        private readonly TextElement text;

        public PartAttributeAdapter(Part part, TextElement text) {

            if (text == null)
                throw new ArgumentNullException("text");

            this.text = text;
            if (part != null && !String.IsNullOrEmpty(text.Value) && text.Value.StartsWith(">"))
                attribute = part.GetAttribute(text.Value.Substring(1));
        }

        public string Value {
            get {
                return attribute != null ? text.Value.Replace(attribute.Name, attribute.Value) : text.Value;
            }
        }

        public PointInt Position {
            get {
                return (attribute != null) && attribute.UsePosition ? attribute.Position : text.Position;
            }
        }

        public Angle Rotation {
            get {
                return (attribute != null) && attribute.UseRotation ? attribute.Rotation : text.Rotation;
            }
        }

        public TextAlign Align {
            get {
                return (attribute != null) && attribute.UseAlign ? attribute.Align: text.Align;
            }
        }
    }
}
