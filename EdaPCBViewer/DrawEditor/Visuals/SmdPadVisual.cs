namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class SmdPadVisual: ElementVisual {

        public SmdPadVisual(SmdPadElement pad, Part part)
            : base(pad, part) {

            RenderVisual();
        }

        public override void RenderVisual() {

            bool isMirror = Part == null ? false : Part.IsMirror;
            Layer layer = isMirror ? Pad.MirrorLayer : Pad.Layer;

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

                // Push de la transformacio de rotacio
                //
                if (Pad.Rotation != 0)
                    dc.PushTransform(new RotateTransform(Pad.Rotation, Pad.Position.X, Pad.Position.Y));

                // Dibuixa el pad
                //
                double radius = Pad.Roundnes * (Math.Min(Pad.Size.Width, Pad.Size.Height) / 2);
                Rect rect = new Rect(
                    Pad.Position.X - (Pad.Size.Width / 2),
                    Pad.Position.Y - (Pad.Size.Height / 2),
                    Pad.Size.Width,
                    Pad.Size.Height);
                Brush padBrush = BrushCache.Instance.GetBrush(IsSelected ? Colors.GreenYellow : layer.Color);
                dc.DrawRoundedRectangle(padBrush, null, rect, radius, radius);

                // Push de la transformacio d'escala del text
                //
                dc.PushTransform(new ScaleTransform(1, -1, Pad.Position.X, Pad.Position.Y));

                // Dibuixa el text
                //
                Brush textBrush = BrushCache.Instance.GetBrush(Colors.Yellow);
                FormattedText formattedText = new FormattedText(
                    Pad.Name,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    0.5,
                    textBrush);
                formattedText.TextAlignment = TextAlignment.Center;
                dc.DrawText(formattedText, new Point(Pad.Position.X, Pad.Position.Y - formattedText.Height / 2));

                // Pop de la transformacio d'escala del text
                //
                dc.Pop();

                // Pop de la transformacio de rotacio
                //
                if (Pad.Rotation != 0)
                    dc.Pop();

                // Pop de la transformacio d'escala pel canvi de cara
                //
                if (isMirror)
                    dc.Pop();

                // Pop de la transformacio global
                //
                //if (transform != null)
                if (Part != null)
                    dc.Pop();
            }
        }

        public SmdPadElement Pad {
            get {
                return (SmdPadElement) Element;
            }
        }
    }
}