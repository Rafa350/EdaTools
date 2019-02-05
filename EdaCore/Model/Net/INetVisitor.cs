namespace MikroPic.EdaTools.v1.Core.Model.Net {

    public interface INetVisitor {

        void Visit(Net net);
        void Visit(NetSignal signal);
        void Visit(NetConnection pin);
    }
}
