namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using MikroPic.EdaTools.v1.Model.Elements;

    internal static class ApertureKeyGenerator {

        public static string GenerateKey(ViaElement via) {

            return string.Format("via${0}${1}${2}", via.Shape, via.Size, via.Drill);
        }

        public static string GenerateKey(ThPadElement pad) {

            return string.Format("th{0}${1}${2}", pad.Shape, pad.Size, pad.Drill);
        }

        public static string GenerateKey(SmdPadElement pad) {

            return string.Format("smd{0}${1}", pad.Size, pad.Roundnes);
        }
    }
}
