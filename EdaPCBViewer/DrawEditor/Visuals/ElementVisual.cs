namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Visuals {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using System;
    using System.Windows.Media;

    public abstract class ElementVisual: VisualItem {

        private readonly Element element;

        public ElementVisual(DrawingVisual parent, Element element) {

            if (element == null)
                throw new ArgumentNullException("parent");

            this.element = element;

            if (parent != null)
                parent.Children.Add(this);
        }

        protected Element Element {
            get {
                return element;
            }
        }
    }
}
