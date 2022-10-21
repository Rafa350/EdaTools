namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Primitives {

    internal sealed class LineDefinition: Definition {

        private int _width;

        public LineDefinition(int id, string tag, int width):
            base(id, tag) {

            _width = width;
        }

        public int LineWidth =>
            _width;
    }
}
