namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class RectangleVisual: ElementVisual {

        public RectangleVisual(RectangleElement rectangle, Part part)
            : base(rectangle, part) {

            RenderVisual();
        }

        public override void RenderVisual() {

            bool isMirror = Part == null ? false : Part.IsMirror;
            Layer layer = isMirror ? Rectangle.MirrorLayer : Rectangle.Layer;

            using (DrawingContext dc = RenderOpen()) {

                // Push de la transformacio global
                //
                if (Part != null) {
                    TransformGroup transform = new TransformGroup();
                    transform.Children.Add(new TranslateTransform(Part.Position.X, Part.Position.Y));
                    transform.Children.Add(new RotateTransform(Part.Rotate, Part.Position.X, Part.Position.Y));
                    dc.PushTransform(transform);
                }

                // Push de la transformacio d'escala pel canvi de cara
                //
                if (isMirror)
                    dc.PushTransform(new ScaleTransform(-1, 1));

                // Dibuixa el rectangle
                //
                Pen pen = null;
                Brush brush = null;

                if (Rectangle.Thickness > 0)
                    pen = PenCache.Instance.GetPen(layer.Color, Rectangle.Thickness);
                else
                    brush = BrushCache.Instance.GetBrush(layer.Color);
                Point position = new Point(
                    Rectangle.Position.X - (Rectangle.Size.Width / 2),
                    Rectangle.Position.Y - (Rectangle.Size.Height / 2));
                dc.DrawRectangle(brush, pen, new Rect(position, Rectangle.Size));

                // Pop de la transformacio d'escala pel canvi de cara
                //
                if (isMirror)
                    dc.Pop();

                // Pop de la transformacio global
                //
                if (Part != null)
                    dc.Pop();
            }
        }

        public RectangleElement Rectangle {
            get {
                return (RectangleElement) Element;
            }
        }
    }
}
