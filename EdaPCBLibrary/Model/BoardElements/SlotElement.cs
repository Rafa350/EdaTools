namespace MikroPic.EdaTools.v1.Pcb.Model.BoardElements {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;

    public sealed class SlotElement: PadElement {

        private int drill;
        private int length;
        private int topSize;
        private int innerSize;
        private int bottomSize;

        SlotElement(string name, Point position, Angle rotation):
            base(name, position, rotation) {

        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public override Rect GetBoundingBox(BoardSide side) {
            throw new System.NotImplementedException();
        }

        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {
            throw new System.NotImplementedException();
        }

        public override Polygon GetPolygon(BoardSide side) {
            throw new System.NotImplementedException();
        }

        public override Polygon GetThermalPolygon(BoardSide side, int spacing, int width) {
            throw new System.NotImplementedException();
        }
    }
}
