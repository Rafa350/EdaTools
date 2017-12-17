namespace MikroPic.EdaTools.v1.Pcb.Model {

    public sealed class Terminal: IVisitable {

        private readonly Part part;
        private readonly string padName;

        public Terminal(Part part, string padName) {

            this.part = part;
            this.padName = padName;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public Part Part {
            get {
                return part;
            }
        }

        public string PadName {
            get {
                return padName;
            }
        }
    }
}
