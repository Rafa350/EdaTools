namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class CircleVisual: ElementVisual {

        public CircleVisual(CircleElement circle, Part part)
            : base(circle, part) {

            RenderVisual();
        }

        public override void RenderVisual() {

            bool isMirror = Part == null ? false : Part.IsMirror;
            Layer layer = isMirror ? Circle.MirrorLayer : Circle.Layer;

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

                // Dibuixa el cercle
                //
                Pen pen = null;
                Brush brush = null;
                if (Circle.Thickness > 0)
                    pen = PenCache.Instance.GetPen(layer.Color, Circle.Thickness);
                else
                    brush = BrushCache.Instance.GetBrush(layer.Color);
                dc.DrawEllipse(brush, pen, Circle.Position, Circle.Radius, Circle.Radius);

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
     
        public CircleElement Circle {
            get {
                return (CircleElement) Element;
            }
        }
    }
}