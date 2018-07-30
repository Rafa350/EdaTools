namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class LineVisual : ElementVisual {

        public LineVisual(LineElement line) : 
            base(line) {

        }

        public LineElement Line {
            get {
                return (LineElement)Element;
            }
        }
    }
}
