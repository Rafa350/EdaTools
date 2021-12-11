using System;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    public readonly struct Roundness {

        private readonly EdaRatio _topLeft;
        private readonly EdaRatio _topRight;
        private readonly EdaRatio _bottomLeft;
        private readonly EdaRatio _bottomRight;

        public Roundness(EdaRatio roundness) {

            _topLeft = roundness;
            _topRight = roundness;
            _bottomLeft = roundness;
            _bottomRight = roundness;
        }

        public Roundness(EdaRatio topLeft, EdaRatio topRight, EdaRatio bottomLeft, EdaRatio bottomRight) {

            _topLeft = topLeft;
            _topRight = topRight;
            _bottomLeft = bottomLeft;
            _bottomRight = bottomRight;
        }

        public override string ToString() =>
            String.Format("{0}, {1}, {2}, {3}", _topLeft, _topRight, _bottomLeft, _bottomRight);

        public static Roundness Parse(string source) {

            try {
                string[] s = source.Split(',');
                var topLeft = EdaRatio.Parse(s[0]);
                var topRight = EdaRatio.Parse(s[1]);
                var bottomLeft = EdaRatio.Parse(s[2]);
                var bottomRight = EdaRatio.Parse(s[3]);

                return new Roundness(topLeft, topRight, bottomLeft, bottomRight);
            }
            catch (Exception ex) {
                throw new InvalidOperationException(
                    String.Format("No es posible convertir el texto '{0}' a 'Roundness'.", source), ex);
            }
        }

        public EdaRatio TopLeft => _topLeft;
        public EdaRatio TopRight => _topRight;
        public EdaRatio BottomLeft => _bottomLeft;
        public EdaRatio BottomRight => _bottomRight;
    }
}
