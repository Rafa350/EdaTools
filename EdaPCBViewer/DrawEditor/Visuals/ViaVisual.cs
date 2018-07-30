namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System.Windows.Media;

    public sealed class ViaVisual: ElementVisual {

        public ViaVisual(DrawingVisual parent, ViaElement via) : 
            base(parent, via) {

        }

        public ViaElement Via {
            get {
                return (ViaElement)Element;
            }
        }
    }
}
