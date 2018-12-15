namespace MikroPic.EdaTools.v1.PanelEditor.Render {

    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using System;
    using System.Windows.Media;

    public sealed class VisualDrawer {

        public VisualItem Create(CutItem item) {

            VisualItem visual = new CutVisualItem(item);
            return visual;
        }

        public VisualItem Create(PcbItem item) {

            VisualItem visual = new PcbVisualItem(item);

            return visual;
        }

        public void Remove(VisualItem visual) {

            if (visual == null)
                throw new ArgumentNullException("visual");

            if (visual.Parent is DrawingVisual parent)
                parent.Children.Remove(visual);
        }

        public void Draw(VisualItem visual) {

            if (visual == null)
                throw new ArgumentNullException("visual");

            using (DrawingContext context = visual.RenderOpen()) {
                visual.Draw(context);
            }
        }
    }
}
