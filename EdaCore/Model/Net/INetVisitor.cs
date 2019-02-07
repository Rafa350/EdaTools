namespace MikroPic.EdaTools.v1.Core.Model.Net {

    public interface INetVisitor {

        void Visit(Net net);
        void Visit(NetPart part);
        void Visit(NetConnection connection);
        void Visit(NetSignal signal);
    }
}
