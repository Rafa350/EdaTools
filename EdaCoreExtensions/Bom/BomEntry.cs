namespace EdaCoreExtensions.Bom {

    public sealed class BomEntry {

        private readonly string _name;
        private readonly List<string> _references;

        public BomEntry(string name, IEnumerable<string> references) {

            _name = name;
            _references = new List<string>(references);
        }

        public string Name =>
            _name;

        public int ReferenceCount =>
            _references.Count;

        public IEnumerable<string> References => 
            _references;
    }
}
