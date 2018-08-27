namespace MikroPic.EdaTools.v1.Cam.Model {

    using MikroPic.EdaTools.v1.Geometry;

    public abstract class PanelElement {

        private Point position;
        private Angle orientation;

        public PanelElement(Point position, Angle orientation) {

            this.position = position;
            this.orientation = orientation;
        }
    }
}
