namespace MikroPic.EdaTools.v1.PanelEditor.DrawEditor {

    using System;
    using System.Windows.Media;

    public delegate void VisualItemRenderEventHandler(object sender, VisualItemRenderEventArgs e);

    public class VisualItemRenderEventArgs: EventArgs {

        private readonly VisualItem visual;
        private readonly DrawingContext dc;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="visual">El visual.</param>
        /// <param name="dc">El context de dibuix.</param>
        /// 
        public VisualItemRenderEventArgs(VisualItem visual, DrawingContext dc) {

            this.visual = visual;
            this.dc = dc;
        }

        /// <summary>
        /// Obte el visual.
        /// </summary>
        /// 
        public VisualItem Visual {
            get {
                return visual;
            }
        }

        /// <summary>
        /// Obte el context de dibuix.
        /// </summary>
        /// 
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
        public VisualItem(VisualItem parent) {

            if (parent != null)
                parent.Children.Add(this);
        }

        /// <summary>
        /// Renderitza el item
        /// </summary>
        /// 
        public void Renderize() {

            using (DrawingContext dc = RenderOpen())
                OnRender(dc);
        }

        /// <summary>
        /// Renderitza el item.
        /// </summary>
        /// <param name="dc">Context de dibuix.</param>
        /// 
        protected virtual void OnRender(DrawingContext dc) {

            Render?.Invoke(this, new VisualItemRenderEventArgs(this, dc));
        }
    }
}
