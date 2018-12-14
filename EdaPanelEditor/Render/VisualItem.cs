namespace MikroPic.EdaTools.v1.PanelEditor.Render {

    using MikroPic.EdaTools.v1.Panel.Model;
    using System;
    using System.Windows.Media;

    public abstract class VisualItem: DrawingVisual {

        private readonly ProjectItem item;

        public VisualItem(ProjectItem item) {

            if (item == null)
                throw new ArgumentNullException("item");

            this.item = item;
        }

        protected abstract void DrawItem(DrawingContext context);

        public void Draw(DrawingContext context) {

            if (context == null)
                throw new ArgumentNullException("context");

            DrawItem(context);
        }

        public ProjectItem Item {
            get {
                return item;
            }
        }
    }
}
