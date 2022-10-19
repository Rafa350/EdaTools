namespace EdaCoreExtensions.Bom {

    public sealed class EdaBom {

        private readonly List<EdaBomEntry> _entries;

        public EdaBom(IEnumerable<EdaBomEntry> entries) {

            _entries = new List<EdaBomEntry>(entries);
        }

        public int EntryCount =>
            _entries.Count;

        public IEnumerable<EdaBomEntry> Entries =>
            _entries;
    }
}
