using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public static class PartExtensions {

        public static Transformation GetLocalTransformation(this Part part) {

            var t = new Transformation();
            t.Translate(part.Position);
            t.Rotate(part.Position, part.Rotation);
            if (part.Flip)
                t.Scale(part.Position, -1, 1);
            return t;
        }

        public static LayerSet GetLocalLayerSet(this Part part, Element element) {

            if (part.IsFlipped) {
                LayerSet layerSet = default;
                foreach (var layerId in element.LayerSet) {
                    var layerIdString = layerId.ToString();
                    if (layerIdString.Contains("Top."))
                        layerIdString = layerIdString.Replace("Top.", "Bottom.");
                    else if (layerIdString.Contains("Bottom."))
                        layerIdString = layerIdString.Replace("Bottom.", "Top.");
                    layerSet += LayerId.Get(layerIdString);
                }
                return layerSet;
            }
            else
                return element.LayerSet;
        }
    }
}
