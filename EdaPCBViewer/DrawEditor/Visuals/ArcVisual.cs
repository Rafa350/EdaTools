namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class ArcVisual: ElementVisual {

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="arc">El element.</param>
        /// <param name="part">El component que forma el element.</param>
        public ArcVisual(ArcElement arc, Part part)
            : base(arc, part) {

            RenderVisual();
        }

        /// <summary>
        /// Renderitza el element.
        /// </summary>
        public override void RenderVisual() {

            bool isMirror = Part == null ? false : Part.IsFlipped;
            Layer layer = Arc.Layer;

            using (DrawingContext dc = RenderOpen()) {

                // Push de la transformacio global
                //
                if (Part != null) {
                    TransformGroup transform = new TransformGroup();
                    transform.Children.Add(new TranslateTransform(Part.Position.X, Part.Position.Y));
                    transform.Children.Add(new RotateTransform(Part.Rotation.Degrees, Part.Position.X, Part.Position.Y));
                    dc.PushTransform(transform);
                }

                // Push de la transformacio d'escala pel canvi de cara
                //
                if (isMirror)
                    dc.PushTransform(new ScaleTransform(-1, 1));

                // Genera la geometria del arc
                //
                StreamGeometry geometry = new StreamGeometry();
                using (StreamGeometryContext ctx = geometry.Open()) {
                    ctx.BeginFigure(Arc.StartPosition, false, false);
                    ctx.ArcTo(Arc.EndPosition, new Size(Arc.Radius, Arc.Radius),
                        Math.Abs(Arc.Angle.Degrees),
                        false,
                        Arc.Angle.Degrees > 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                        true, true);
                }
                geometry.Freeze();

                // Dibuixa la geometria
                //
                Pen pen = PenCache.Instance.GetPen(layer.Color, Arc.Thickness,
                    Arc.LineCap == LineElement.LineCapStyle.Flat ? PenLineCap.Flat : PenLineCap.Round);
                dc.DrawGeometry(null, pen, geometry);

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

        /// <summary>
        /// Obte el element asignat.
        /// </summary>
        public ArcElement Arc {
            get {
                return (ArcElement) Element;
            }
        }
    }
}

