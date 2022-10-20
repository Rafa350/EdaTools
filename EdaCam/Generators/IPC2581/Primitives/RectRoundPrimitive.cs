namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Primitives {

    internal class RectRoundPrimitive: Primitive {

        private readonly int _width;
        private readonly int _height;
        private readonly int _radius;

        public RectRoundPrimitive(int id, string tag, int width, int height, int radius) :
            base(id, tag) {

            _width = width;
            _height = height;
            _radius = radius;
        }

        public int Width =>
            _width;

        public int Height =>
            _height;

        public int Radius =>
            _radius;
    }
}
