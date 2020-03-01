
namespace EdaBoardViewer.Views.Controls {

    using System;
    using System.Globalization;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;

    public sealed partial class DesignControl : Control {

        private const string xTagFormat = " X: {0:0.00} ";
        private const string yTagFormat = " Y: {0:0.00} ";
        private const string wTagFormat = " W: {0:0.00} ";
        private const string hTagFormat = " H: {0:0.00} ";

        private Point ptPointer;
        private Point ptRegionStart;
        private Point ptRegionEnd;

        /// <summary>
        /// Renderitza el control.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// 
        public override void Render(DrawingContext context) {

            Point Transform(Point p) {

                Matrix matrix = ValueMatrix;
                return new Point(
                    (p.X * matrix.M11) + (p.Y * matrix.M21) + matrix.M31,
                    (p.X * matrix.M12) + (p.Y * matrix.M22) + matrix.M32);
            }

            ptPointer = Transform(PointerPosition);

            Point p1 = Transform(RegionPosition);
            Point p2 = Transform(new Point(RegionPosition.X + RegionSize.Width, RegionPosition.Y + RegionSize.Height));
            ptRegionStart = new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            ptRegionEnd = new Point(ptRegionStart.X + Math.Abs(p2.X - p1.X), ptRegionStart.Y + Math.Abs(p2.Y - p1.Y));

            if (ShowPointer) {
                DrawPointer(context);
                if (ShowPointerTags)
                    DrawPointerTags(context);
            }

            if (ShowRegion) {
                DrawRegion(context);
                if (ShowRegionTags)
                    DrawRegionTags(context);
                if (ShowRegionHandles)
                    DrawRegionHandles(context);
            }
        }

        /// <summary>
        /// Dibuixa el indicador de posicio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawPointer(DrawingContext context) {

            var pen = new Pen(new SolidColorBrush(PointerColor), 0.5);
            context.DrawPixLine(pen, new Point(0, ptPointer.Y), new Point(Bounds.Width, ptPointer.Y));
            context.DrawPixLine(pen, new Point(ptPointer.X, 0), new Point(ptPointer.X, Bounds.Height));
        }

        /// <summary>
        /// Dibuixa les etiquetes del indicador de posicio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawPointerTags(DrawingContext context) {

            FormattedText ft = new FormattedText();
            ft.TextAlignment = TextAlignment.Left;
            ft.Typeface = new Typeface(FontFamily, FontSize, FontStyle);

            double penWidth = 0.5;

            var borderPen = new Pen(new SolidColorBrush(PointerTagBorderColor), penWidth);
            var backgroundBrush = PointerTagBackground;
            var textBrush = new SolidColorBrush(PointerTagTextColor);

            ft.Text = String.Format(CultureInfo.InvariantCulture, xTagFormat, PointerPosition.X / ValueDivisor);
            var r1 = new Rect(
                ptPointer.X - ft.Bounds.Width - ft.Bounds.Height,
                ptPointer.Y - (ft.Bounds.Height / 2.0),
                ft.Bounds.Size.Width,
                ft.Bounds.Size.Height);

            context.DrawPixBox(backgroundBrush, borderPen, r1);
            context.DrawText(textBrush, r1.TopLeft, ft);

            ft.Text = String.Format(CultureInfo.InvariantCulture, yTagFormat, PointerPosition.Y / ValueDivisor);
            var r2 = new Rect(
                ptPointer.X - ft.Bounds.Width - ft.Bounds.Height,
                ptPointer.Y - (ft.Bounds.Height / 2.0),
                ft.Bounds.Size.Width,
                ft.Bounds.Size.Height);

            Matrix m = Matrix.Identity;
            m *= Matrix.CreateTranslation(-ptPointer.X, -ptPointer.Y);
            m *= Matrix.CreateRotation(Matrix.ToRadians(90));
            m *= Matrix.CreateTranslation(ptPointer.X, ptPointer.Y);
            using (context.PushPreTransform(m)) {
                context.DrawPixBox(backgroundBrush, borderPen, r2);
                context.DrawText(textBrush, r2.TopLeft, ft);
            }
        }

        /// <summary>
        /// Renderitza la regio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawRegion(DrawingContext context) {

            context.DrawPixBox(
                RegionBackground,
                new Pen(new SolidColorBrush(RegionBorderColor), 0.5),
                new Rect(ptRegionStart, ptRegionEnd));
        }

        /// <summary>
        /// Renderitza les marques de la regio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawRegionTags(DrawingContext context) {

            FormattedText ft = new FormattedText();
            ft.TextAlignment = TextAlignment.Left;
            ft.Typeface = new Typeface(FontFamily, FontSize, FontStyle);

            var borderPen = new Pen(new SolidColorBrush(RegionTagBorderColor), 0.5);
            var backgroundBrush = RegionTagBackground;
            var textBrush = new SolidColorBrush(RegionTagTextColor);

            ft.Text = String.Format(CultureInfo.InvariantCulture, wTagFormat, RegionSize.Width / ValueDivisor);
            var r1 = new Rect(
                new Point(ptRegionEnd.X - ft.Bounds.Width - ft.Bounds.Height, ptRegionEnd.Y - (ft.Bounds.Height / 2)),
                ft.Bounds.Size);

            context.DrawPixBox(backgroundBrush, borderPen, r1);
            context.DrawText(textBrush, r1.TopLeft, ft);

            ft.Text = String.Format(CultureInfo.InvariantCulture, hTagFormat, RegionSize.Height / ValueDivisor);
            var r2 = new Rect(
                new Point(ptRegionEnd.X - ft.Bounds.Width - ft.Bounds.Height, ptRegionEnd.Y - (ft.Bounds.Height / 2)),
                ft.Bounds.Size);

            Matrix m = Matrix.Identity;
            m *= Matrix.CreateTranslation(-ptRegionEnd.X, -ptRegionEnd.Y);
            m *= Matrix.CreateRotation(Matrix.ToRadians(90));
            m *= Matrix.CreateTranslation(ptRegionEnd.X, ptRegionEnd.Y);
            using (context.PushPreTransform(m)) {
                context.DrawPixBox(backgroundBrush, borderPen, r2);
                context.DrawText(textBrush, r2.TopLeft, ft);
            }
        }

        /// <summary>
        /// Dibuixa els manipuladors de la regio.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// 
        private void DrawRegionHandles(DrawingContext context) {

            static Rect GetRect(double x, double y, double s) {

                return new Rect(x - s / 2, y - s / 2, s, s);
            }

            double margin = 10;
            double size = 8;

            double x1 = ptRegionStart.X - margin;
            double x2 = (ptRegionStart.X + ptRegionEnd.X) / 2;
            double x3 = ptRegionEnd.X + margin;

            double y1 = ptRegionStart.Y - margin;
            double y2 = (ptRegionStart.Y + ptRegionEnd.Y) / 2;
            double y3 = ptRegionEnd.Y + margin;

            var borderPen = new Pen(new SolidColorBrush(RegionTagBorderColor), 0.5);
            var backgroundBrush = RegionTagBackground;

            context.DrawPixBox(backgroundBrush, borderPen, GetRect(x1, y1, size));
            context.DrawPixBox(backgroundBrush, borderPen, GetRect(x2, y1, size));
            context.DrawPixBox(backgroundBrush, borderPen, GetRect(x3, y1, size));
            context.DrawPixBox(backgroundBrush, borderPen, GetRect(x1, y2, size));
            context.DrawPixBox(backgroundBrush, borderPen, GetRect(x3, y2, size));
            context.DrawPixBox(backgroundBrush, borderPen, GetRect(x1, y3, size));
            context.DrawPixBox(backgroundBrush, borderPen, GetRect(x2, y3, size));
            context.DrawPixBox(backgroundBrush, borderPen, GetRect(x3, y3, size));
        }
    }
}
