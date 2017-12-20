namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class TextVisual: ElementVisual {

        public TextVisual(TextElement text, Part part) :
            base(text, part) {

            RenderVisual();
        }

        public override void RenderVisual() {

            bool isMirror = Part == null ? false : Part.IsMirror;
            Layer layer = isMirror ? Text.MirrorLayer : Text.Layer;

            using (DrawingContext dc = RenderOpen()) {

                // Push de la transformacio global
                //
                if (Part != null) {
                    TransformGroup transform = new TransformGroup();
                    transform.Children.Add(new TranslateTransform(Part.Position.X, Part.Position.Y));
                    transform.Children.Add(new RotateTransform(Part.Rotation, Part.Position.X, Part.Position.Y));
                    dc.PushTransform(transform);
                }

                string value;
                double x, y, rotate;

                Parameter parameter = Part.GetParameter(Text.Name);
                if (parameter == null) {
                    value = Text.Value;
                    x = Text.Position.X;
                    y = Text.Position.Y;
                    rotate = Text.Rotation;
                }
                else {
                    value = parameter.Value;
                    x = parameter.Position.X;
                    y = parameter.Position.Y;
                    rotate = parameter.Rotate;
                }

                if (!String.IsNullOrEmpty(value)) {

                    Brush brush = BrushCache.Instance.GetBrush(Text.Layer.Color);
                    FormattedText formattedText = new FormattedText(
                        value,
                        CultureInfo.CurrentUICulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Courier New"),
                        Text.Height,
                        brush);
                    formattedText.TextAlignment = TextAlignment.Left;

                    if (isMirror)
                        dc.PushTransform(new ScaleTransform(-1, 1));
                    dc.PushTransform(new ScaleTransform(1, -1, x, y));

                    y = y - formattedText.Height;
                    dc.DrawRectangle(null, new Pen(Brushes.Fuchsia, 0.05), new Rect(x, y, formattedText.Width, formattedText.Height));
                    dc.DrawText(formattedText, new Point(x, y));

                    dc.Pop();
                    if (isMirror)
                        dc.Pop();
                }

                // Pop de la transformacio global
                //
                if (Part != null)
                    dc.Pop();
            }
        }

        public TextElement Text {
            get {
                return (TextElement) Element;
            }
        }
    }
}