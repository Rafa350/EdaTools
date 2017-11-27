namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    public abstract class ElementBase: IVisitable {

        public abstract void AcceptVisitor(IVisitor visitor);

        public Layer Layer {
            get {
                return Layer.LayerOf(this);
            }
            set {
                Layer layer = Layer.LayerOf(this);
                if (layer != null)
                    layer.RemoveElement(this);
                if (value != null)
                    value.AddElement(this);
            }
        }

        public Layer MirrorLayer {
            get {
                Layer layer = Layer.LayerOf(this);
                if (layer == null)
                    return null;
                else if (layer.Mirror == null)
                    return null;
                else
                    return layer.Mirror;
            }
        }
    }
}
