namespace MikroPic.EdaTools.v1.Cam.Ipcd365.Builder {

    using System;
    using System.IO;
    using MikroPic.EdaTools.v1.Pcb.Geometry;

    public enum TestAccess{
        Top,
        Bottom,
        Both
    }

    public sealed class Ipcd365Builder {

        private readonly TextWriter writer;

        public Ipcd365Builder(TextWriter writer) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        public void Comment(string text) {

            writer.Write(
                "C  {0}", 
                text);
            writer.WriteLine();
        }

        public void SetUnits() {

            writer.WriteLine("P UNITS SI");
        }

        public void ThPad() {

        }

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

        public void Via(PointInt position, int drill, string netName) {

            writer.Write(
                "317{0,-14}   VIA   -     D{3}PA00X{1}Y{2}",
                netName,
                FormatCoordinate(position.X),
                FormatCoordinate(position.Y),
                FormatDiameter(drill));
            writer.WriteLine();
        }

        public void EndFile() {

            writer.WriteLine("999");
        }

        private static string FormatDiameter(int number) {

            return String.Format("{0:0000}", number / 1000);
        }

        private static string FormatCoordinate(int number) {

            bool sign = number >= 0;
            int value = Math.Abs(number / 1000);

            return String.Format("{0}{1:000000}", sign ? '+' : '-', value);        }
    }
}
