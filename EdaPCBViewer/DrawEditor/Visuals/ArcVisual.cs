namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System.Windows.Media;

    public sealed class ArcVisual : ElementVisual {

        public ArcVisual(DrawingVisual parent, ArcElement arc) :
            base(parent, arc) {

        }

        public ArcElement Arc {
            get {
                return (ArcElement)Element;
            }
        }
    }
}
