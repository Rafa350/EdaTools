namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System.Windows.Media;

    public sealed class CircleVisual: ElementVisual {

        public CircleVisual(DrawingVisual parent, CircleElement circle) : 
            base(parent, circle) {

        }

        public CircleElement Circle {
            get {
                return (CircleElement) Element;
            }
        }
    }
}
