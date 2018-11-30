namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Geometry;

    public static class PartExtensions {

        public static Transformation GetLocalTransformation(this Part part) {

            return new Transformation(part.Position, part.Rotation);
        }
    }
}
