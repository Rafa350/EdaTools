namespace MikroPic.EdaTools.v1.Base.Geometry {

    public readonly struct Segment {

        private readonly EdaPoint _start;
        private readonly EdaPoint _end;
        private readonly EdaAngle _angle;

        public Segment(EdaPoint start, EdaPoint end) {

            _start = start;
            _end = end;
            _angle = EdaAngle.Zero;
        }

        public Segment(EdaPoint start, int length, EdaAngle angle) {

            _start = start;
            _end = start;
            _angle = angle;
        }

        public EdaPoint Start =>
            _start;

        public EdaPoint End =>
            _end;

        public EdaAngle Angle =>
            _angle;
    }
}
