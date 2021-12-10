using System;

namespace MikroPic.EdaTools.v1.Base.Geometry {
    
    public readonly struct Roundness {

        private readonly Ratio _topLeft;
        private readonly Ratio _topRight;
        private readonly Ratio _bottomLeft;
        private readonly Ratio _bottomRight;

        public Roundness(Ratio roundness) {

            _topLeft = roundness;
            _topRight = roundness;  
            _bottomLeft = roundness;    
            _bottomRight = roundness;  
        }

        public Roundness(Ratio topLeft, Ratio topRight, Ratio bottomLeft, Ratio bottomRight) {
         
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
                var topLeft = Ratio.Parse(s[0]);
                var topRight = Ratio.Parse(s[1]);
                var bottomLeft = Ratio.Parse(s[2]);
                var bottomRight = Ratio.Parse(s[3]);

                return new Roundness(topLeft, topRight, bottomLeft, bottomRight);
            }
            catch (Exception ex) {
                throw new InvalidOperationException(
                    String.Format("No es posible convertir el texto '{0}' a 'Roundness'.", source), ex);
            }
        }

        public Ratio TopLeft => _topLeft;
        public Ratio TopRight => _topRight;
        public Ratio BottomLeft => _bottomLeft;
        public Ratio BottomRight => _bottomRight;
    }
}
