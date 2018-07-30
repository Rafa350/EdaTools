namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class RectangleVisual: ElementVisual {

        public RectangleVisual(RectangleElement rectangle) : 
            base(rectangle) {

        }

        public RectangleElement Rectangle {
            get {
                return (RectangleElement) Element;
            }
        }
    }
}
