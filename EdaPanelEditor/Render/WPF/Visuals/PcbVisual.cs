namespace MikroPic.EdaTools.v1.PanelEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using System.Windows.Media;

    public sealed class PcbVisual: ItemVisual {

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">Visual pare.</param>
        /// <param name="item">El item PCB.</param>
        /// 
        public PcbVisual(DrawingVisual parent, PcbItem item):
            base(parent, item) {
        }

        /// <summary>
        /// Renderitzat.
        /// </summary>
        /// <param name="dc">Context de renderitzat.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            Brush brush = dc.GetBrush(Color.FromRgb(31, 80, 10));

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
