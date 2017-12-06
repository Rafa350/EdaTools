namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    internal static class ApertureKeyGenerator {

        public static string GenerateKey(LineElement line) {

            return string.Format("line${0}", line.Thickness);
        }

        public static string GenerateKey(ArcElement arc) {

            return string.Format("line${0}", arc.Thickness);
        }

        public static string GenerateKey(RectangleElement rectangle) {

            return string.Format("rectangle${0}${1}", rectangle.Size.Width, rectangle.Size.Height);
        }

        public static string GenerateKey(ViaElement via, bool drillMode) {

            if (drillMode)
                return string.Format("drill${0}", via.Drill);
            else
                return string.Format("via${0}${1}${2}", via.Shape, via.Size, via.Drill);
        }

        public static string GenerateKey(ThPadElement pad, bool drillMode) {

            if (drillMode)
                return string.Format("drill${0}", pad.Drill);
            else
                return string.Format("th{0}${1}${2}${3}", pad.Shape, pad.Size, pad.Drill, pad.Rotate);
        }

        public static string GenerateKey(SmdPadElement pad) {

            return string.Format("smd{0}${1}${2}${3}", pad.Size, pad.Roundnes, pad.Rotate, pad.Rotate);
        }
    }
}
