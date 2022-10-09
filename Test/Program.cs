using MikroPic.EdaTools.v1.Base.Geometry;


namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Program {

        public static void Main(string[] args) {

            var p1 = EdaPointFactory.CreateLineTrace(new EdaPoint(0, 0), new EdaPoint(100, 100), 5, true);

            var p2 = EdaPointFactory.CreateLineTrace(new EdaPoint(0, 0), new EdaPoint(-100, 100), 5, true);

            var p3 = EdaPointFactory.CreateLineTrace(new EdaPoint(0, 0), new EdaPoint(-100, -100), 5, true);

            var p4 = EdaPointFactory.CreateLineTrace(new EdaPoint(0, 0), new EdaPoint(100, -100), 5, true);
        }
    }
}