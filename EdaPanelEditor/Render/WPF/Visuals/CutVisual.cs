namespace MikroPic.EdaTools.v1.PanelEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Panel.Model.Items;

    public sealed class CutVisual: ItemVisual {

        public CutVisual(System.Windows.Media.DrawingVisual parent, CutItem item):
            base(parent, item) {
        }

        protected override void Draw(DrawVisualContext dc) {

            System.Windows.Media.Pen pen = dc.GetPen(new Color(255, 128, 128, 128), Item.Tickness, System.Windows.Media.PenLineCap.Round);
            dc.DrawLine(pen, Item.StartPosition, Item.EndPosition);
        }

        public new CutItem Item {
            get {
                return base.Item as CutItem;
            }
        }
    }
}
