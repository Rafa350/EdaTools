namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System.Windows.Media;

    public sealed class SmdPadVisual: ElementVisual {

        public SmdPadVisual(DrawingVisual parent, SmdPadElement pad) : 
            base(parent, pad) {

        }

        public SmdPadElement Pad {
            get {
                return (SmdPadElement)Element;
            }
        }
    }
}
