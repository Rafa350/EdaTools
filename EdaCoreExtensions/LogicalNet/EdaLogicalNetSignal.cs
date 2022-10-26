namespace MikroPic.EdaTools.v1.CoreExtensions.LogicalNet {

    public sealed class EdaLogicalNetSignal {

        private readonly string _name;
        private readonly List<EdaLogicalNetPart> _parts;

        public EdaLogicalNetSignal(string name, IEnumerable<EdaLogicalNetPart> parts) {

            _name = name;
            _parts = new List<EdaLogicalNetPart>(parts);
        }

        public string Name =>
            _name;

        public IEnumerable<EdaLogicalNetPart> Parts =>
            _parts;
    }
}
