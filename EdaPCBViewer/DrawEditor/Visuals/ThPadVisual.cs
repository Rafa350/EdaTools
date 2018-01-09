namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class ThPadVisual: ElementVisual {

        public ThPadVisual(ThPadElement pad, Part part)
            : base(pad, part) {

            RenderVisual();
        }

        public override void RenderVisual() {

            using (DrawingContext dc = RenderOpen()) {

                // Push de la transformacio global
                //
                if (Part != null) {
                    TransformGroup transform = new TransformGroup();
                    transform.Children.Add(new TranslateTransform(Part.Position.X, Part.Position.Y));
                    transform.Children.Add(new RotateTransform(Part.Rotation, Part.Position.X, Part.Position.Y));
                    dc.PushTransform(transform);
                }

                // Dibuixa el pad
                //
                Brush padBrush = BrushCache.Instance.GetBrush(IsSelected ? Colors.Goldenrod : Colors.Gold);
                dc.DrawPolygon(padBrush, null, Pad.Polygon);
                /*
                // Push de la transformacio d'escala del text
                //
                dc.PushTransform(new ScaleTransform(1, -1, Pad.Position.X, Pad.Position.Y));

                // Dibuixa el text amb el nom del pad
                //
                Brush textBrush = BrushCache.Instance.GetBrush(Colors.Yellow);
                FormattedText formattedText = new FormattedText(
                    Pad.Name, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                    new Typeface("Arial"), 0.5, textBrush);
                formattedText.TextAlignment = TextAlignment.Center;
                dc.DrawText(formattedText, new Point(Pad.Position.X, Pad.Position.Y - formattedText.Height / 2));

                // Pop de la transformacio d'escala del text
                //
                dc.Pop();
                */
                // Pop de la transformacio global
                //
                if (Part != null)
                    dc.Pop();
            }
        }

        public ThPadElement Pad {
            get {
                return (ThPadElement) Element;
            }
        }
    }
}
