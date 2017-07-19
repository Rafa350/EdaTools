namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Model;
    using MikroPic.EdaTools.v1.Model.Elements;

    public sealed class LineVisual: ElementVisual {

        public LineVisual(LineElement line, Part part)
            : base(line, part) {

            RenderVisual();
        }

        public override void RenderVisual() {

            bool isMirror = Part == null ? false : Part.IsMirror;
            Layer layer = isMirror ? Line.MirrorLayer : Line.Layer;

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

                // Dibuixa la linia
                //
                Pen pen = PenCache.Instance.GetPen(layer.Color, Line.Thickness,
                    Line.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);
                dc.DrawLine(pen, Line.StartPosition, Line.EndPosition);

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
