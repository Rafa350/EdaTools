namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Base.Geometry;

    public static class PartExtensions {

        public static Transformation GetLocalTransformation(this Part part) {

            Transformation t = new Transformation();
            t.Translate(part.Position);
            t.Rotate(part.Position, part.Rotation);
            if (part.Flip)
                t.Scale(part.Position, -1, 1);
            return t;
        }

        public static LayerSet GetLocalLayerSet(this Part part, Element element) {

/*            if (part.Flip) {
                LayerSet layerSet = default;
                foreach (var layerId in element.LayerSet)
                    layerSet += new LayerId(layerId.Name, layerId.ReverseSide);
                return layerSet;
            }
            else*/
                return element.LayerSet;
        }
    }
}
