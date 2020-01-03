namespace MikroPic.EdaTools.v1.JSon.Model {

    public sealed class JSonArray: JSonValue {

        private JSonValue[] values;

        public JSonArray(params JSonValue[] values) {

            this.values = values;
        }

        public override void AcceptVisitor(IJSonVisitor visitor) {

            visitor.Visit(this);
        }

        public JSonValue[] Values {
            get {
                return values;
            }
        }
    }
}
