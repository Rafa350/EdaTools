namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed class EdaSignalNode {

        private readonly IEdaConectable _conectable;
        private readonly EdaPart _part;

        public EdaSignalNode(IEdaConectable conectable, EdaPart part = null) {

            _conectable = conectable;
            _part = part;
        }

        public IEdaConectable Conectable =>
            _conectable;

        public EdaPart Part =>
            _part;
    }
}
