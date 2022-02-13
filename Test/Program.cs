using System;
using System.Collections.Generic;
using System.Linq;
using MikroPic.EdaTools.v1.Base.Geometry;


namespace MyApp // Note: actual namespace depends on the project name.
{
    public class Program {

        public static void Main(string[] args) {

            EdaPoints p1 = EdaPoints.CreateLineTrace(new EdaPoint(0, 0), new EdaPoint(100, 100), 5, true);

            EdaPoints p2 = EdaPoints.CreateLineTrace(new EdaPoint(0, 0), new EdaPoint(-100, 100), 5, true);

            EdaPoints p3 = EdaPoints.CreateLineTrace(new EdaPoint(0, 0), new EdaPoint(-100, -100), 5, true);

            EdaPoints p4 = EdaPoints.CreateLineTrace(new EdaPoint(0, 0), new EdaPoint(100, -100), 5, true);
        }
    }
}