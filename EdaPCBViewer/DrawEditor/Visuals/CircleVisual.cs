namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class CircleVisual: ElementVisual {

        public CircleVisual(CircleElement circle) : 
            base(circle) {

        }

        public CircleElement Circle {
            get {
                return (CircleElement) Element;
            }
        }
    }
}
