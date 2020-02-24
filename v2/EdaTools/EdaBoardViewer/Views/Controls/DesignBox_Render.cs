namespace EdaBoardViewer.Views.Controls {

    using System;
    using System.Globalization;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;

    public sealed partial class DesignBox : Control {

        public override void Render(DrawingContext context) {

            if (IsPointerVisible())
                DrawPointer(context);
            if (IsRegionVisible())
                DrawRegion(context);

            if (IsPointerTagsVisible())
                DrawPointerTags(context);
            if (IsRegionTagsVisible())
                DrawRegionTags(context);
        }

        /// <summary>
        /// Dibuixa el indicador de posicio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawPointer(DrawingContext context) {

            double x = (PointerPosition.X + Origin.X) * Scale;
            double y = (PointerPosition.Y + Origin.Y) * Scale;

            var pen = new Pen(new SolidColorBrush(PointerColor), 0.5);
            context.DrawLine(pen, new Point(0.5, y + 0.5), new Point(Bounds.Width + 0.5, y + 0.5));
            context.DrawLine(pen, new Point(x + 0.5, 0.5), new Point(x + 0.5, Bounds.Height + 0.5));
        }

        /// <summary>
        /// Dibuixa les etiquetes del indicador de posicio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawPointerTags(DrawingContext context) {

            double x = (PointerPosition.X + Origin.X) * Scale;
            double y = (PointerPosition.Y + Origin.Y) * Scale;

            FormattedText ft = new FormattedText();
            ft.TextAlignment = TextAlignment.Left;
            ft.Typeface = new Typeface(FontFamily, FontSize, FontStyle);

            var borderPen = new Pen(new SolidColorBrush(PointerTagBorderColor), 0.5);
            var backgroundBrush = PointerTagBackground;
            var textBrush = new SolidColorBrush(PointerTagTextColor);

            ft.Text = String.Format(CultureInfo.InvariantCulture, " X: {0:0.00} ", PointerPosition.X / ValueDivisor);
            var r1 = new Rect(new Point(x - ft.Bounds.Width - ft.Bounds.Height + 0.5, y - (ft.Bounds.Height / 2) + 0.5), ft.Bounds.Size);

            context.DrawRectangle(borderPen, r1);
            context.FillRectangle(backgroundBrush, r1);
            context.DrawText(textBrush, r1.TopLeft, ft);

            ft.Text = String.Format(CultureInfo.InvariantCulture, " Y: {0:0.00} ", PointerPosition.Y / ValueDivisor);
            var r2 = new Rect(new Point(x - ft.Bounds.Width - ft.Bounds.Height + 0.5, y - (ft.Bounds.Height / 2) + 0.5), ft.Bounds.Size);

            Matrix m = Matrix.Identity;
            m *= Matrix.CreateTranslation(-x, -y);
            m *= Matrix.CreateRotation(90 * Math.PI / 180);
            m *= Matrix.CreateTranslation(x, y);
            using (context.PushPreTransform(m)) {
                context.DrawRectangle(borderPen, r2);
                context.FillRectangle(backgroundBrush, r2);
                context.DrawText(textBrush, r2.TopLeft, ft);
            }
        }

        /// <summary>
        /// Renderitza la regio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawRegion(DrawingContext context) {

            double x = (RegionPosition.X + Origin.X) * Scale;
            double y = (RegionPosition.Y + Origin.Y) * Scale;
            double w = RegionSize.Width * Scale;
            double h = RegionSize.Height * Scale;

            var pen = new Pen(new SolidColorBrush(RegionBorderColor), 0.5);
            var r = new Rect(x + 0.5, y + 0.5, w, h);
            context.FillRectangle(RegionBackground, r);
            context.DrawRectangle(pen, r);
        }

        /// <summary>
        /// Renderitza les marques de la regio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawRegionTags(DrawingContext context) {

            double x = (RegionPosition.X + RegionSize.Width) * Scale;
            double y = (RegionPosition.Y + RegionSize.Height) * Scale;

            FormattedText ft = new FormattedText();
            ft.TextAlignment = TextAlignment.Left;
            ft.Typeface = new Typeface(FontFamily, FontSize, FontStyle);

            var borderPen = new Pen(new SolidColorBrush(RegionTagBorderColor), 0.5);
            var backgroundBrush = RegionTagBackground;
            var textBrush = new SolidColorBrush(RegionTagTextColor);

            ft.Text = String.Format(CultureInfo.InvariantCulture, " W: {0:0.00} ", RegionSize.Width / ValueDivisor);
            var r1 = new Rect(new Point(x - ft.Bounds.Width - ft.Bounds.Height + 0.5, y - (ft.Bounds.Height / 2) + 0.5), ft.Bounds.Size);

            context.DrawRectangle(borderPen, r1);
            context.FillRectangle(backgroundBrush, r1);
            context.DrawText(textBrush, r1.TopLeft, ft);

            ft.Text = String.Format(CultureInfo.InvariantCulture, " H: {0:0.00} ", RegionSize.Height / ValueDivisor);
            var r2 = new Rect(new Point(x - ft.Bounds.Width - ft.Bounds.Height + 0.5, y - (ft.Bounds.Height / 2) + 0.5), ft.Bounds.Size);

            Matrix m = Matrix.Identity;
            m *= Matrix.CreateTranslation(-x, -y);
            m *= Matrix.CreateRotation(90 * Math.PI / 180);
            m *= Matrix.CreateTranslation(x, y);
            using (context.PushPreTransform(m)) {
                context.DrawRectangle(borderPen, r2);
                context.FillRectangle(backgroundBrush, r2);
                context.DrawText(textBrush, r2.TopLeft, ft);
            }

        }

        private bool IsPointerVisible() {

            return ShowPointer;
        }

        private bool IsPointerTagsVisible() {

            return IsPointerVisible() && ShowPointerTags;
        }

        private bool IsRegionVisible() {

            return ShowRegion;
        }

        private bool IsRegionTagsVisible() {

            return IsRegionVisible() && ShowRegionTags;
        }
    }
}
