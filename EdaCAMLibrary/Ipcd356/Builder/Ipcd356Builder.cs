namespace MikroPic.EdaTools.v1.Cam.Ipcd356.Builder {

    using System;
    using System.IO;
    using MikroPic.EdaTools.v1.Pcb.Geometry;

    public enum TestAccess {
        None,
        Top,
        Bottom,
        Both
    }

    public enum Units {
        Inches,
        Millimeters
    }

    /// <summary>
    /// Generador de codi IPCD356
    /// </summary>
    public sealed class Ipcd356Builder {

        private readonly TextWriter writer;

        /// <summary>
        /// Constructor del objecte
        /// </summary>
        /// <param name="writer">Escriptor de sortida.</param>
        /// 
        public Ipcd356Builder(TextWriter writer) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        /// <summary>
        /// Escriu un comentari
        /// </summary>
        /// <param name="text">El text del comentari.</param>
        /// 
        public void Comment(string text) {

            writer.Write(
                "C  {0}", 
                text);
            writer.WriteLine();
        }

        public void SetVersion() {

            writer.WriteLine("P  VER   IPC-D-356");
        }

        public void SetImage() {

            writer.WriteLine("P  IMAGE PRIMARY");
        }

        /// <summary>
        /// Selecciona les unitats
        /// </summary>
        /// 
        public void SetUnits(Units units) {

            switch (units) {
                case Units.Inches:
                    writer.WriteLine("P  UNITS CUST 0");
                    break;

                case Units.Millimeters:
                    writer.WriteLine("P  UNITS CUST 1");
                    break;
            }
        }

        public void Conductor(PointInt[] points, int layerNum, int thickness, string netName) {

            if (points == null)
                throw new ArgumentNullException("points");

            if (points.Length < 2)
                throw new ArgumentOutOfRangeException("points");

            if (layerNum <= 0)
                throw new ArgumentOutOfRangeException("lauerNum");

            if (thickness <= 0)
                throw new ArgumentOutOfRangeException("thickness");

            if (String.IsNullOrEmpty(netName))
                throw new ArgumentNullException("netName");

            writer.Write(
                "378{0,-14} L{1} X{2}Y0000 X{3}Y{4} X{5}Y{6}{7}",
                netName,
                FormatLayerNum(layerNum),
                FormatDiameter(thickness),
                FormatCoordinate(points[0].X),
                FormatCoordinate(points[0].Y),
                FormatCoordinate(points[1].X),
                FormatCoordinate(points[1].Y),
                points.Length > 2 ? ' ' : '*');

            if (points.Length > 2) {
                writer.WriteLine();
                writer.Write("078 ");
                for (int i = 2; i < points.Length; i++) {
                    writer.Write(
                        " X{0}Y{1}",
                        FormatCoordinate(points[i].X),
                        FormatCoordinate(points[i].Y));
                }
                writer.Write("*");
            }

            writer.WriteLine();
        }

        /// <summary>
        /// Escriu una definicio de pad throug hole
        /// </summary>
        /// 
        public void ThPad(PointInt position, int drill, string partId, string padId, string netName) {

            if (drill <= 0)
                throw new ArgumentOutOfRangeException("drill");

            if (String.IsNullOrEmpty(partId))
                throw new ArgumentNullException("partId");

            if (String.IsNullOrEmpty(padId))
                throw new ArgumentNullException("padId");

            if (String.IsNullOrEmpty(netName))
                throw new ArgumentNullException("netName");

            writer.Write(
                "317{0,-14}   {3,-6}-{4,-4} D{5}PA00X{1}Y{2}",
                netName,
                FormatCoordinate(position.X),
                FormatCoordinate(position.Y),
                partId,
                padId,
                FormatDiameter(drill));
            writer.WriteLine();
        }

        /// <summary>
        /// Escriu una definicio de pad smd
        /// </summary>
        /// <param name="position"></param>
        /// <param name="access"></param>
        /// <param name="partId"></param>
        /// <param name="padId"></param>
        /// <param name="netName"></param>
        /// 
        public void SmdPad(PointInt position, TestAccess access, string partId, string padId, string netName) {

            if (String.IsNullOrEmpty(partId))
                throw new ArgumentNullException("partId");

            if (String.IsNullOrEmpty(padId))
                throw new ArgumentNullException("padId");

            if (String.IsNullOrEmpty(netName))
                throw new ArgumentNullException("netName");

            writer.Write(
                "327{0,-14}   {3,-6}-{4,-4}       A{5}X{1}Y{2}",
                netName,
                FormatCoordinate(position.X),
                FormatCoordinate(position.Y),
                partId,
                padId,
                FormatAccess(access));
            writer.WriteLine();
        }

        /// <summary>
        /// Escriu una definicio de via
        /// </summary>
        /// <param name="position"></param>
        /// <param name="drill"></param>
        /// <param name="netName"></param>
        /// 
        public void Via(PointInt position, int drill, string netName) {

            if (drill <= 0)
                throw new ArgumentOutOfRangeException("drill");

            if (String.IsNullOrEmpty(netName))
                throw new ArgumentNullException("netName");

            writer.Write(
                "317{0,-14}   VIA   -     D{3}PA00X{1}Y{2}",
                netName,
                FormatCoordinate(position.X),
                FormatCoordinate(position.Y),
                FormatDiameter(drill));
            writer.WriteLine();
        }

        /// <summary>
        /// Escriu el indicador de final de fitxer
        /// </summary>
        /// 
        public void EndFile() {

            writer.WriteLine("999");
        }

        /// <summary>
        /// Formateja una mesura del diametre
        /// </summary>
        /// <param name="number">Valor a formatejar</param>
        /// <returns>La cadena formatejada</returns>
        /// 
        private static string FormatDiameter(int number) {

            return String.Format("{0:0000}", number / 1000);
        }

        /// <summary>
        /// Formateja una mesura de posicio
        /// </summary>
        /// <param name="number">El valor a formatejat</param>
        /// <returns>La cadena formatejada</returns>
        /// 
        private static string FormatCoordinate(int number) {

            bool sign = number >= 0;
            int value = Math.Abs(number / 1000);

            return String.Format("{0}{1:000000}", sign ? '+' : '-', value);
        }

        private string FormatLayerNum(int layerNum) {

            return String.Format("{0:00}", layerNum);
        }

        private string FormatAccess(TestAccess access) {

            switch (access) {
                default:
                case TestAccess.Both:
                    return "00";

                case TestAccess.Top:
                    return "01";

                case TestAccess.Bottom:
                    return "02";
            }
        }
    }
}
