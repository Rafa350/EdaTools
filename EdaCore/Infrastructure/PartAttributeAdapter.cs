using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using System;

namespace MikroPic.EdaTools.v1.Core.Infrastructure {

    public sealed class PartAttributeAdapter {

        private const string _startMacro = "{";
        private const string _endMacro = "}";
        private readonly EdaPartAttribute _attribute;
        private readonly EdaTextElement _text;
        private readonly string _attrValue;

        public PartAttributeAdapter(EdaPart part, EdaTextElement text) {

            _text = text ?? throw new ArgumentNullException(nameof(text));

            if ((part != null) &&
                !String.IsNullOrEmpty(text.Value) &&
                text.Value.StartsWith(_startMacro) &&
                text.Value.EndsWith(_endMacro)) {

                _attribute = part.GetAttribute(text.Value.Substring(_startMacro.Length, text.Value.Length - _startMacro.Length - _endMacro.Length));
                _attrValue = _attribute.Value;
                if (_attrValue == "{%name}")
                    _attrValue = part.Name;
            }
        }

        public string Value =>
            _attribute != null ? _text.Value.Replace(_text.Value, _attrValue) : _text.Value;

        public EdaPoint Position =>
            (_attribute != null) && _attribute.UsePosition ? _attribute.Position : _text.Position;

        public EdaAngle Rotation =>
            (_attribute != null) && _attribute.UseRotation ? _attribute.Rotation : _text.Rotation;

        public int Height =>
            (_attribute != null) && _attribute.UseHeight ? _attribute.Height : _text.Height;

        public bool IsVisible =>
            _attribute == null || _attribute.IsVisible;

        public HorizontalTextAlign HorizontalAlign =>
            (_attribute != null) && _attribute.UseAlign ? _attribute.HorizontalAlign : _text.HorizontalAlign;

        public VerticalTextAlign VerticalAlign =>
            (_attribute != null) && _attribute.UseAlign ? _attribute.VerticalAlign : _text.VerticalAlign;

        public EdaPartAttribute Attribute =>
            _attribute;
    }
}
