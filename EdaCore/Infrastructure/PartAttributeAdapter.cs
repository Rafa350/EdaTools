using System;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Core.Infrastructure {

    public sealed class PartAttributeAdapter {

        private const string _startMacro = "{";
        private const string _endMacro = "}";
        private readonly PartAttribute _attribute;
        private readonly TextElement _text;

        public PartAttributeAdapter(Part part, TextElement text) {

            _text = text ?? throw new ArgumentNullException(nameof(text));
            
            if ((part != null) && 
                !String.IsNullOrEmpty(text.Value) && 
                text.Value.StartsWith(_startMacro) && 
                text.Value.EndsWith(_endMacro))
                _attribute = part.GetAttribute(text.Value.Substring(_startMacro.Length, text.Value.Length - _startMacro.Length - _endMacro.Length));
        }

        public string Value =>
            _attribute != null ? _text.Value.Replace(_text.Value, _attribute.Value) : _text.Value;

        public Point Position =>
            (_attribute != null) && _attribute.UsePosition ? _attribute.Position : _text.Position;

        public Angle Rotation =>
            (_attribute != null) && _attribute.UseRotation ? _attribute.Rotation : _text.Rotation;

        public int Height =>
            (_attribute != null) && _attribute.UseHeight ? _attribute.Height : _text.Height;

        public bool IsVisible =>
            _attribute == null ? true : _attribute.IsVisible;

        public HorizontalTextAlign HorizontalAlign =>
            (_attribute != null) && _attribute.UseAlign ? _attribute.HorizontalAlign : _text.HorizontalAlign;

        public VerticalTextAlign VerticalAlign =>
            (_attribute != null) && _attribute.UseAlign ? _attribute.VerticalAlign : _text.VerticalAlign;

        public PartAttribute Attribute =>
            _attribute;
    }
}
