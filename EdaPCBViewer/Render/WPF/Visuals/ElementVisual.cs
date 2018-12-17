namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Core.Model.Board;
    using System;
    using System.Windows.Media;

    public abstract class ElementVisual: DrawingVisual {

        private readonly Element element;

        public ElementVisual(DrawingVisual parent, Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            this.element = element;

            if (parent != null)
                parent.Children.Add(this);
        }

        public void Draw() {

            using (DrawingContext dc = RenderOpen())
                Draw(new DrawVisualContext(dc));
        }

        protected abstract void Draw(DrawVisualContext dc);

        public Element Element {
            get {
                return element;
            }
        }
    }
}
