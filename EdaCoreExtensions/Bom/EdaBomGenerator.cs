using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.CoreExtensions.Bom {

    public sealed class EdaBomGenerator {

        private readonly EdaBoard _board;
        private readonly string _mpnAttributeName = "MPN";

        public EdaBomGenerator(EdaBoard board) {

            _board = board ?? throw new ArgumentNullException(nameof(board));
        }

        public IEnumerable<EdaBomEntry> Generate() {

            // Obte els components unics de la placa en un diccionari
            //
            var dictionary = new Dictionary<string, List<EdaPart>>();
            foreach (var part in _board.Parts) {
                var partMPN = GetMPN(part);
                if (partMPN != null) {
                    if (!dictionary.TryGetValue(partMPN, out var value)) {
                        value = new List<EdaPart>();
                        dictionary.Add(partMPN, value);
                    }
                    value.Add(part);
                }
            }

            // Obte les referencies de cada component
            //
            var bomEntries = new List<EdaBomEntry>();
            foreach (var kv in dictionary) {
                
                var name = kv.Key;
                var references = kv.Value.Select(part => part.Name);
                var bomEntry = new EdaBomEntry(name, references);

                bomEntries.Add(bomEntry);
            }

            return bomEntries;
        }

        private string? GetMPN(EdaPart part) =>
            part.GetAttribute(_mpnAttributeName)?.Value;
    }
}
