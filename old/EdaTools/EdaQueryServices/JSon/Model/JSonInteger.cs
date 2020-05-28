namespace MikroPic.EdaTools.v1.JSon.Model {

    public sealed class JSonInteger: JSonValue {

        private readonly int value;

        public JSonInteger(int value) {

            this.value = value;
        }

        public override void AcceptVisitor(IJSonVisitor visitor) {

            visitor.Visit(this);
        }

        public override string ToString() {

            return value.ToString();
        }

        public int Value {
            get {
                return value;
            }
        }
    }
}
