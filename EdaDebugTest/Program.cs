namespace EdaDebugTest {

    using System;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons.Infrastructure;

    class Program {

        static void Main(string[] args) {

            Point v1 = new Point( 0,  0);
            Point v2 = new Point( 0, 20);
            Point v3 = new Point(20, 20);
            Point v4 = new Point(20,  0);

            Segment[] segments = new Segment[] {
                new Segment(v1, v2),
                new Segment(v3, v2),
                new Segment(v3, v4),
                new Segment(v4, v1),
            };

            Polygonizer.Poligonize(segments);
        }
    }
}
