namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System.Windows.Media;

    public sealed class RectangleVisual: ElementVisual {

        public RectangleVisual(DrawingVisual parent, RectangleElement rectangle) : 
            base(parent, rectangle) {

        }

        public RectangleElement Rectangle {
            get {
                return (RectangleElement) Element;
            }
        }
    }
}
