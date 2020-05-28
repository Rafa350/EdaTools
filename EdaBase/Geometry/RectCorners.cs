namespace MikroPic.EdaTools.v1.Base.Geometry {

    public readonly struct RectCorners {

        private readonly int topLeft;
        private readonly int topRight;
        private readonly int bottomLeft;
        private readonly int bottomRight;

        public RectCorners(int value) {

            topLeft = value;
            topRight = value;
            bottomLeft = value;
            bottomRight = value;
        }
    }
}
