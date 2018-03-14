namespace MikroPic.EdaTools.v1.Cam.Ipcd365.Builder {

    using System;
    using System.IO;
    using MikroPic.EdaTools.v1.Pcb.Geometry;

    public enum TestAccess{
        Top,
        Bottom,
        Both
    }

    /// <summary>
    /// Generador de codi IPCD365
    /// </summary>
    public sealed class Ipcd365Builder {

        private readonly TextWriter writer;

        /// <summary>
        /// Constructor del objecte
        /// </summary>
        /// <param name="writer">Escriptor de sortida.</param>
        /// 
        public Ipcd365Builder(TextWriter writer) {

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

        /// <summary>
        /// Selecciona les unitats
        /// </summary>
        /// 
        public void SetUnits() {

            writer.WriteLine("P  UNITS SI");
        }

        /// <summary>
        /// Escriu una definicio de pad throug hole
        /// </summary>
        /// 
        public void ThPad(PointInt position, int drill, string partId, string padId, string netName) {

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

            int accessNum;
            switch (access) {
                default:
                case TestAccess.Both:
                    accessNum = 0;
                    break;

                case TestAccess.Top:
                    accessNum = 1;
                    break;

                case TestAccess.Bottom:
                    accessNum = 2;
                    break;
            }

            writer.Write(
                "327{0,-14}   {3,-6}-{4,-4}       A{5:00}X{1}Y{2}",
                netName,
                FormatCoordinate(position.X),
                FormatCoordinate(position.Y),
                partId,
                padId,
                accessNum);
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

            return String.Format("{0}{1:000000}", sign ? '+' : '-', value);        }
    }
}
