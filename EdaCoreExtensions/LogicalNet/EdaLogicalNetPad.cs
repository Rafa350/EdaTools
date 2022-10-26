namespace MikroPic.EdaTools.v1.CoreExtensions.LogicalNet {

    public sealed class EdaLogicalNetPad {

        private readonly string _name;

        public EdaLogicalNetPad(string name) {

            _name = name;
        }

        public string Name =>
            _name;
    }
}