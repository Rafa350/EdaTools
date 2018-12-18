namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using System.Windows.Media;

    public abstract class VisualBase: DrawingVisual {

        public VisualBase(DrawingVisual parent) {

            if (parent != null)
                parent.Children.Add(this);
        }

        public void Draw() {

            using (DrawingContext dc = RenderOpen())
                Draw(new DrawVisualContext(dc));
        }

        protected abstract void Draw(DrawVisualContext dc);
    }
}
