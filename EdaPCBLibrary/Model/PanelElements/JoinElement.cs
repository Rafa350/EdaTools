namespace MikroPic.EdaTools.v1.Pcb.Model.PanelElements {

    using System;
    using MikroPic.EdaTools.v1.Geometry;

    public sealed class JoinElement: PanelElement {

        public JoinElement(Point position, Angle orientation):
            base(position, orientation) {

        }
    }
}
