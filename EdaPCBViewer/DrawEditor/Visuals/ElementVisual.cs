namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using System.Windows.Media;

    public abstract class ElementVisual: DrawingVisual {

        private readonly Element element;

        public ElementVisual(Element element) {

            this.element = element;
        }

        protected Element Element {
            get {
                return element;
            }
        }
    }
}
