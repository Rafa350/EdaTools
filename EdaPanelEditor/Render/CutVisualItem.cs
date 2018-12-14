namespace MikroPic.EdaTools.v1.PanelEditor.Render {

    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using System.Windows.Media;

    using WinPoint = System.Windows.Point;

    public sealed class CutVisualItem: VisualItem {

        public CutVisualItem(CutItem item):
            base(item) {
        }

        protected override void DrawItem(DrawingContext context) {

            WinPoint start = new WinPoint(Item.StartPosition.X, Item.StartPosition.Y);
            WinPoint end = new WinPoint(Item.EndPosition.X, Item.EndPosition.Y);

            Pen pen = new Pen(new SolidColorBrush(Colors.Cyan), Item.Tickness);
            pen.StartLineCap = PenLineCap.Round;
            pen.EndLineCap = PenLineCap.Round;

            context.DrawLine(pen, start, end);
        }

        public new CutItem Item {
            get {
                return base.Item as CutItem;
            }
        }
    }
}
