namespace MikroPic.EdaTools.v1.PanelEditor.Render {

    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using System.Windows.Media;

    using WinPoint = System.Windows.Point;
    using WinRect = System.Windows.Rect;
    using WinSize = System.Windows.Size;

    public sealed class PcbVisualItem: VisualItem {

        public PcbVisualItem(PcbItem item):
            base(item) {
        }

        protected override void DrawItem(DrawingContext context) {

            WinPoint position = new WinPoint(Item.Position.X, Item.Position.Y);
            WinSize size = new WinSize(Item.Board.Size.Width, Item.Board.Size.Height);

            Brush brush = Brushes.OliveDrab;
            context.DrawRectangle(brush, null, new WinRect(position, size));
        }

        public new PcbItem Item {
            get {
                return base.Item as PcbItem;
            }
        }
    }
}
