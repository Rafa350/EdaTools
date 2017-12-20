namespace MikroPic.EdaTools.v1.Pcb.Model {

    public interface ISingleLayer {

        void AddToLayer(Layer layer);

        void RemoveFromLayer(Layer layer);

        Layer Layer { get; }
    }
}
