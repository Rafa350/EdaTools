using MikroPic.EdaTools.v1.Core.Model.Common;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed class EdaDevice: IEdaVisitable<IEdaBoardVisitor> {

        private string _name;

        public void AcceptVisitor(IEdaBoardVisitor visitor) {

        }

        public string Name {
            get => _name;
            set => _name = value;
        }
    }
}
