namespace MikroPic.EdaTools.v1.JSon.Model {
    
    public sealed class JSonObject: JSonValue {

        private readonly string className;
        private readonly JSonProperty[] properties;

        public JSonObject(string className, params JSonProperty[] properties) {

            this.className = className;
            this.properties = properties;
        }

        public override void AcceptVisitor(IJSonVisitor visitor) {

            visitor.Visit(this);
        }

        public string ClassName {
            get {
                return className;
            }
        }

        public JSonProperty[] Properties {
            get {
                return properties;
            }
        }
    }
}
