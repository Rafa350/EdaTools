namespace MikroPic.EdaTools.v1.Core.Model.Net {

    public interface INetVisitable {

        void AcceptVisitor(INetVisitor visitor);
    }
}
