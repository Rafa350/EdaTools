namespace MikroPic.EdaTools.v1.Base.Geometry {

    public readonly struct EdaArcPoint {

        private readonly EdaPoint _position;
        private readonly EdaAngle _arc;

        public EdaArcPoint(EdaPoint position) {

            _position = position;
            _arc = EdaAngle.Zero;
        }

        public EdaArcPoint(EdaPoint position, EdaAngle arc) {

            _position = position;
            _arc = arc;
        }

        public EdaPoint Position =>
            _position;

        public EdaAngle Arc =>
            _arc;
    }
}
