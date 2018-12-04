namespace MikroPic.EdaTools.v1.Panelizer.Infrastructure {

    using MikroPic.EdaTools.v1.Base.Geometry;

    public static class SegmentExtensions {

        public static Segment[] Split(this Segment segment, int cuts) {

            return new Segment[] { segment };
        }
    }
}
