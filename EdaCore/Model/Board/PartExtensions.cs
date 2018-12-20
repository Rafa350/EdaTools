namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Base.Geometry;

    public static class PartExtensions {

        public static Transformation GetLocalTransformation(this Part part) {

            Transformation t = new Transformation();
            t.Translate(part.Position);
            t.Rotate(part.Position, part.Rotation);
            return t;
        }
    }
}
