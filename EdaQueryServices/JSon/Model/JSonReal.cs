namespace MikroPic.EdaTools.v1.JSon.Model {

    public sealed class JSonReal: JSonValue {

        private readonly double value;

        public JSonReal(double value) {

            this.value = value;
        }

        public override void AcceptVisitor(IJSonVisitor visitor) {

            visitor.Visit(this);
        }

        public override string ToString() {

            return value.ToString();
        }

        public double Value {
            get {
                return value;
            }
        }
    }
}
