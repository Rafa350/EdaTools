using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace EdaComponentGenerator.Generators {

    public sealed class DilGenerator {

        private EdaSize _padSize = new(600000, 1550000);
        private int _padSpacing = 1270000;
        private int _pinCount = 8;
        private int _rowSpacing = 5400000;

        public EdaComponent Generate() {

            var component = new EdaComponent();

            // Genera els pads
            //
            var dx = (_padSpacing * (_pinCount / 2) - 1) / 2;
            var dy = _rowSpacing / 2;

            for (int n = 0; n < _pinCount; n++) {

                var x = (n * _padSpacing) - dx;
                var y = ((n < _pinCount / 2) ? 0 : _rowSpacing) - dy;

                var pad = new EdaSmtPadElement {
                    Name = $"{n + 1}",
                    Position = new EdaPoint(x, y),
                    Size = _padSize,
                    CornerShape = EdaSmtPadElement.CornerShapeType.Round
                };
                
                component.AddElement(pad);
            }

            return component;
        }
    }
}
