namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class ArcVisual : ElementVisual {

        public ArcVisual(ArcElement arc) :
            base(arc) {

        }

        public ArcElement Arc {
            get {
                return (ArcElement)Element;
            }
        }
    }
}
