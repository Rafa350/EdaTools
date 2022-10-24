namespace MikroPic.EdaTools.v1.CoreExtensions.LogicalNet {
    
    public sealed class EdaLogicalNet {

        private readonly List<EdaLogicalNetEntry> _nets;

        public EdaLogicalNet(IList<EdaLogicalNetEntry> nets) {

            _nets = new List<EdaLogicalNetEntry>(nets);
        }

        public IEnumerable<EdaLogicalNetEntry> Nets =>
            _nets;
    }
}
