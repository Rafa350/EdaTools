namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    public abstract class ElementBase: IVisitable {

        public abstract void AcceptVisitor(IVisitor visitor);

        public abstract bool InLayer(Layer layer);
    }
}
