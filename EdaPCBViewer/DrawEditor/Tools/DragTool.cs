namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Tools {

    using System.Windows;
    using System.Windows.Media;

    public sealed class DragTool: DesignTool {

        public DragTool(DesignSurface surface)
            : base(surface) {
        }

        protected override void RenderBox(DrawingContext dc, Point startPosition, Point endPosition) {
        }
    }
}