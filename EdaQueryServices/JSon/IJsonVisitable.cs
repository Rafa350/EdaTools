namespace MikroPic.EdaTools.v1.JSon {

    public interface IJSonVisitable {

        void AcceptVisitor(IJSonVisitor visitor);
    }
}
