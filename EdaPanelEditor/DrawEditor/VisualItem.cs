namespace MikroPic.EdaTools.v1.PanelEditor.DrawEditor {

    using System;
    using System.Windows.Media;

    public delegate void VisualItemRenderEventHandler(object sender, VisualItemRenderEventArgs e);

    public class VisualItemRenderEventArgs: EventArgs {

        private readonly DrawingContext dc;

        public VisualItemRenderEventArgs(DrawingContext dc) {

            this.dc = dc;
        }

        public DrawingContext Dc {
            get {
                return dc;
            }
        }
    }

    public class VisualItem: DrawingVisual {

        public event VisualItemRenderEventHandler Render;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// 
        public VisualItem() {
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="parent">Visual pare.</param>
        /// 
        public VisualItem(DrawingVisual parent) {

            if (parent != null)
                parent.Children.Add(this);
        }

        /// <summary>
        /// Actualitza el item
        /// </summary>
        /// 
        public void Refresh() {

            using (DrawingContext dc = RenderOpen())
                OnRender(dc);
        }

        /// <summary>
        /// Renderitza el item.
        /// </summary>
        /// <param name="dc">Contexte de dibuix.</param>
        /// 
        protected virtual void OnRender(DrawingContext dc) {

            Render?.Invoke(this, new VisualItemRenderEventArgs(dc));
        }
    }
}
