using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.CoreExtensions.LogicalNet {

    public sealed class EdaLogicalNetGenerator {

        private readonly EdaBoard _board;

        public EdaLogicalNetGenerator(EdaBoard board) {

            _board = board;
        }

        public IEnumerable<EdaLogicalNetSignal> Generate() {

            var netSignals = new Dictionary<string, EdaLogicalNetSignal>();
            var netParts = new Dictionary<string, EdaLogicalNetPart>();

            foreach (var part in _board.Parts) {
                foreach (var pad in part.Pads) {
                    var signal = _board.GetSignal(pad, part, false);
                    if (signal != null) {
                        var signalName = signal.Name;
                        var padName = pad.Name;
                        var partName = pad.Name;
                    }
                }
            }

            return null;
        }
    }
}
