using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Primitives {

    internal class RectRoundDefinition: Definition {

        private readonly EdaSize _size;
        private readonly EdaRatio _ratio;

        public RectRoundDefinition(int id, string tag, EdaSize size, EdaRatio ratio) :
            base(id, tag) {

            _size = size;
            _ratio = ratio;
        }

        public EdaSize Size =>
            _size;

        public EdaRatio Ratio =>
            _ratio;
    }
}
