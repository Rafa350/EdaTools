namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model;

    internal static class PartHelper {

        public static Point Transform(this Part part, Point point) {

            Matrix m = new Matrix();
            m.Translate(part.Position.X, part.Position.Y);
            m.RotateAt(part.Rotation.Degrees, part.Position.X, part.Position.Y);

            return m.Transform(point);
        }
    }
}
