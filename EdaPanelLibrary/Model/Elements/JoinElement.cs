namespace MikroPic.EdaTools.v1.Panel.Model.Elements {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Panel.Model;

    public sealed class JoinElement: PanelElement {

        public JoinElement(Point position, Angle rotation):
            base(position, rotation) {

        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }
    }
}
