namespace MikroPic.EdaTools.v1.JSon {

    using MikroPic.EdaTools.v1.JSon.Model;

    public abstract class JSonDefaultVisitor: IJSonVisitor {

        public virtual void Visit(JSonArray jsonArray) {
        }

        public virtual void Visit(JSonObject jsonObject) {
        }

        public virtual void Visit(JSonProperty jsonProperty) {
        }

        public virtual void Visit(JSonBoolean jsonBooloan) {
        }

        public virtual void Visit(JSonInteger jsonInteger) {
        }

        public virtual void Visit(JSonReal jsonReal) {
        }

        public virtual void Visit(JSonString jsonString) {
        }
    }
}
