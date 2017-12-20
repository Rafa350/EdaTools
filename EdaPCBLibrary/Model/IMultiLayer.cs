namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System.Collections.Generic;

    public interface IMultiLayer {

        void AddToLayer(Layer layer);

        void RemoveFromLayer(Layer layer);

        IEnumerable<Layer> Layers { get; }
    }
}
