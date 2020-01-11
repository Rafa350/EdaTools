namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
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

            if (part.IsFlipped) {
                LayerSet layerSet = default;
                foreach (var layerName in element.LayerSet) {
                    if (layerName.Contains(".")) {
                        string[] s = layerName.Split('.');
                        if (s[0] == "Top")
                            s[0] = "Bottom";
                        else if (s[0] == "Bottom")
                            s[0] = "Top";
                        layerSet += Layer.GetName((BoardSide)Enum.Parse(typeof(BoardSide), s[0]), s[1]);
                    }
                    else
                        layerSet += layerName;
                }
                return layerSet;
            }
            else
                return element.LayerSet;
        }
    }
}
