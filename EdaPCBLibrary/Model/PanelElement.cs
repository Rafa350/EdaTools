namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Geometry;

    public abstract class PanelElement {

        private Point position;
        private Angle rotation;

        public PanelElement(Point position, Angle rotation) {

            this.position = position;
            this.rotation = rotation;
        }
    }
}
