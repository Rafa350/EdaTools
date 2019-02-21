namespace MikroPic.EdaTools.v1.PanelEditor.Render.Visuals {

    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using MikroPic.EdaTools.v1.PanelEditor.DrawEditor;

    public sealed class PcbItemVisual: PanelItemVisual {

        private readonly Color boardColor = Color.FromRgb(31, 80, 10);

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">Visual pare.</param>
        /// <param name="item">El item PCB.</param>
        /// 
        public PcbItemVisual(VisualItem parent, PcbItem item):
            base(parent, item) {
        }

        protected override void OnRender(DrawingContext dc) {

            Draw(new DrawVisualContext(dc));
            base.OnRender(dc);
        }

        /// <summary>
        /// Renderitzat.
        /// </summary>
        /// <param name="dc">Context de renderitzat.</param>
        /// 
        private void Draw(DrawVisualContext dc) {

            Brush brush = dc.GetBrush(boardColor);

            Size size = Item.Size;
            Point position = Item.Position.Offset(size.Width / 2, size.Height / 2);
            dc.DrawRectangle(brush, null, position, size);
        }

        /// <summary>
        /// Obte el item associat.
        /// </summary>
        /// 
        public new PcbItem Item {
            get {
                return base.Item as PcbItem;
            }
        }
    }
}
