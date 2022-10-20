namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Primitives {

    internal class CirclePrimitive: Primitive {

        private readonly int _diameter;

        public CirclePrimitive(int id, string tag, int diameter) :
            base(id, tag) {

            _diameter = diameter;
        }

        public int Diameter =>
            _diameter;
    }
}
