namespace MikroPic.EdaTools.v1.JSon.Model {

    using System;

    public sealed class JSonProperty: IJSonVisitable {

        private readonly string name;
        private readonly JSonValue value;

        public JSonProperty(string name, JSonValue value) {

            this.name = name;
            this.value = value;
        }

        public void AcceptVisitor(IJSonVisitor visitor) {

            visitor.Visit(this);
        }

        public override string ToString() {

            return String.Format("\"{0}\": {1}", name, value);
        }

        public string Name {
            get {
                return name;
            }
        }

        public JSonValue Value {
            get {
                return value;
            }
        }        
    }
}
