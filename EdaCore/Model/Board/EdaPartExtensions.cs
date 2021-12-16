using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public static class EdaPartExtensions {

        public static Transformation GetLocalTransformation(this EdaPart part) {

            var t = new Transformation();
            t.Translate(part.Position);
            t.Rotate(part.Position, part.Rotation);
            if (part.IsFlipped)
                t.Scale(part.Position, -1, 1);
            return t;
        }
    }
}
