namespace MikroPic.EdaTools.v1.PanelEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using System.Windows.Media;

    public sealed class PcbVisual: ItemVisual {

        public PcbVisual(DrawingVisual parent, PcbItem item):
            base(parent, item) {
        }

        protected override void Draw(DrawVisualContext dc) {

            Brush brush = Brushes.OliveDrab;
            dc.DrawRectangle(brush, null, Item.Position, new Base.Geometry.Size(100000000, 100000000));
        }

        public new PcbItem Item {
            get {
                return base.Item as PcbItem;
            }
        }
    }
}
