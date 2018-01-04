namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class HoleVisual: ElementVisual {

        public HoleVisual(HoleElement hole, Part part)
            : base(hole, part) {

            RenderVisual();
        }

        public override void RenderVisual() {

            bool isMirror = Part == null ? false : Part.IsFlipped;
            //Layer layer = isMirror ? Hole.MirrorLayer : Hole.Layer;

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

                // Dibuixa el forat
                //
                //Pen pen = PenCache.Instance.GetPen(layer.Color, 0.05);
                Pen pen = PenCache.Instance.GetPen(Colors.Coral, 0.05);
                dc.DrawPolygon(null, pen, Hole.Polygon);

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

        public HoleElement Hole {
            get {
                return (HoleElement) Element;
            }
        }
    }
}

