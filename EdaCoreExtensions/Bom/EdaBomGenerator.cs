using MikroPic.EdaTools.v1.Core.Model.Board;

namespace EdaCoreExtensions.Bom {

    public sealed class EdaBomGenerator {

        private readonly EdaBoard _board;
        private readonly string _mpnAttributeName = "MPN";

        public EdaBomGenerator(EdaBoard board) {

            _board = board ?? throw new ArgumentNullException(nameof(board));
        }

        public EdaBom Generate() {

            var bom = new Dictionary<string, List<EdaPart>>();
            foreach (var part in _board.Parts) {
                var attribute = part.GetAttribute(_mpnAttributeName);
                if (attribute != null) {
                    if (!bom.TryGetValue(attribute.Value, out var value)) {
                        value = new List<EdaPart>();
                        bom.Add(attribute.Value, value);
                    }
                    value.Add(part);
                }
            }

            var bomEntries = new List<EdaBomEntry>();
            foreach (var bomItem in bom) {
                var name = bomItem.Key;
                var references = bomItem.Value.Select(part => part.Name);
                var bomEntry = new EdaBomEntry(bomItem.Key, references);
                bomEntries.Add(bomEntry);
            }

            return new EdaBom(bomEntries);
        }
    }
}
