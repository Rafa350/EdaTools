namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System.Windows.Media;

    public sealed class LineVisual : ElementVisual {

        public LineVisual(DrawingVisual parent, LineElement line) : 
            base(parent, line) {

        }

        public LineElement Line {
            get {
                return (LineElement)Element;
            }
        }
    }
}
