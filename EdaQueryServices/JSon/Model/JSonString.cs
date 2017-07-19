namespace MikroPic.EdaTools.v1.JSon.Model {

    public sealed class JSonString: JSonValue {

        private readonly string value;

        public JSonString(string value) {

            this.value = value;
        }

        public override void AcceptVisitor(IJSonVisitor visitor) {

            visitor.Visit(this);
        }

        public override string ToString() {

            return value;
        }

        public string Value {
            get {
                return value;
            }
        }
    }
}
