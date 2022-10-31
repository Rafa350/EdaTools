using System;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    public readonly struct EdaSegment {

        private readonly EdaPoint _start;
        private readonly EdaPoint _end;

        public EdaSegment(EdaPoint start, EdaPoint end) {

            _start = start;
            _end = end;
        }

        public override string ToString() =>
            String.Format("X1: {0}; Y1: {1}; X2: {2}; Y2: {3}",
                Math.Round(_start.X / 1000000.0, 3),
                Math.Round(_start.Y / 1000000.0, 3),
                Math.Round(_end.X / 1000000.0, 3),
                Math.Round(_end.Y / 1000000.0, 3));

        public EdaPoint Start =>
            _start;

        public EdaPoint End =>
            _end;
    }
}
