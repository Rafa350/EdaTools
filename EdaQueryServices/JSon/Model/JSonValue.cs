namespace MikroPic.EdaTools.v1.JSon.Model {
   
    public abstract class JSonValue: IJSonVisitable {

        public abstract void AcceptVisitor(IJSonVisitor visitor);
    }
}
