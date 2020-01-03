namespace MikroPic.EdaTools.v1.JSon.Model {

    public sealed class JSonBoolean: JSonValue {

        private readonly bool value;

        public JSonBoolean(bool value) {

            this.value = value;
        }

        public override void AcceptVisitor(IJSonVisitor visitor) {

            visitor.Visit(this);
        }

        public override string ToString() {

            return value.ToString();
        }

        public bool Value {
            get {
                return value;
            }
        }
    }
}
