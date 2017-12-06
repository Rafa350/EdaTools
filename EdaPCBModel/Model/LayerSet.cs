namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class LayerSet: IEnumerable<Layer> {

        private readonly List<Layer> items = new List<Layer>();

        public void Add(IEnumerable<Layer> layerSet) {

            foreach (Layer layer in layerSet)
                Add(layer);
        }

        public void Add(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (items.Contains(layer))
                throw new InvalidOperationException("La capa ya pertenece al conjunto.");

            items.Add(layer);
        }

        public void Remove(Layer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

            if (!items.Contains(layer))
                throw new InvalidOperationException("La capa no pertenece al conjunto.");

            items.Remove(layer);
        }

        public IEnumerator<Layer> GetEnumerator() {

            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {

            return items.GetEnumerator();
        }

        public int Count {
            get {
                return items.Count;
            }
        }

        public IEnumerable<Layer> Layers {
            get {
                return items;
            }
        }
    }
}
