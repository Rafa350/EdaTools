namespace MikroPic.EdaTools.v1.CoreExtensions.LogicalNet {

    public sealed class EdaLogicalNetPart {

        private readonly string _name;
        private readonly IEnumerable<EdaLogicalNetPad> _pads;

        public EdaLogicalNetPart(string name, IEnumerable<EdaLogicalNetPad> pads) {

            _name = name;
            _pads = new List<EdaLogicalNetPad>(pads);
        }

        public string Name =>
            _name;

        public IEnumerable<EdaLogicalNetPad> Pads =>
            _pads;
    }
}
