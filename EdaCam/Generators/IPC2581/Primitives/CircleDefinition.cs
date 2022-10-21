namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Primitives {

    internal class CircleDefinition: Definition {

        private readonly int _diameter;

        public CircleDefinition(int id, string tag, int diameter) :
            base(id, tag) {

            _diameter = diameter;
        }

        public int Diameter =>
            _diameter;
    }
}
