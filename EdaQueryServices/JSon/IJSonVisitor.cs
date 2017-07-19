namespace MikroPic.EdaTools.v1.JSon {

    using MikroPic.EdaTools.v1.JSon.Model;

    public interface IJSonVisitor {

        void Visit(JSonArray jsonArray);
        void Visit(JSonObject jsonObject);
        void Visit(JSonProperty jsonProperty);
        void Visit(JSonBoolean jsonBoolean);
        void Visit(JSonInteger jsonInteger);
        void Visit(JSonReal jsonReal);
        void Visit(JSonString jsonString);
    }
}
