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

        public EdaRatio TopLeft =>
            _topLeft;

        public EdaRatio TopRight =>
            _topRight;

        public EdaRatio BottomLeft =>
            _bottomLeft;

        public EdaRatio BottomRight =>
            _bottomRight;
    }
}
