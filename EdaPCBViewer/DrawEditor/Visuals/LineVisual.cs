namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class LineVisual: ElementVisual {

        public LineVisual(LineElement line, Part part)
            : base(line, part) {

            RenderVisual();
        }

        public override void RenderVisual() {

            bool isMirror = Part == null ? false : Part.IsFlipped;
            Layer layer = Line.Layer;

            using (DrawingContext dc = RenderOpen()) {

                // Push de la transformacio global
                //
                if (Part != null) {
                    TransformGroup transform = new TransformGroup();
                    transform.Children.Add(new TranslateTransform(Part.Position.X, Part.Position.Y));
                    transform.Children.Add(new RotateTransform(Part.Rotation, Part.Position.X, Part.Position.Y));
                    dc.PushTransform(transform);
                }

                // Push de la transformacio d'escala pel canvi de cara
                //
                if (isMirror)
                    dc.PushTransform(new ScaleTransform(-1, 1));

                // Dibuixa la linia
                //
                Brush brush = BrushCache.Instance.GetBrush(layer.Color);
                dc.DrawPolygon(brush, null, Line.Polygon);

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

        public LineElement Line {
            get {
                return (LineElement) Element;
            }
        }
    }
}
