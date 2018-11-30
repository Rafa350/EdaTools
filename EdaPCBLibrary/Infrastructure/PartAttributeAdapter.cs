namespace MikroPic.EdaTools.v1.Core.Infrastructure {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System;

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

        public Point Position {
            get {
                return (attribute != null) && attribute.UsePosition ? attribute.Position : text.Position;
            }
        }

        public Angle Rotation {
            get {
                return (attribute != null) && attribute.UseRotation ? attribute.Rotation : text.Rotation;
            }
        }

        public int Height {
            get {
                return (attribute != null) && attribute.UseHeight ? attribute.Height : text.Height;
            }
        }

        public HorizontalTextAlign HorizontalAlign {
            get {
                return (attribute != null) && attribute.UseAlign ? attribute.HorizontalAlign: text.HorizontalAlign;
            }
        }
        public VerticalTextAlign VerticalAlign {
            get {
                return (attribute != null) && attribute.UseAlign ? attribute.VerticalAlign : text.VerticalAlign;
            }
        }
    }
}
