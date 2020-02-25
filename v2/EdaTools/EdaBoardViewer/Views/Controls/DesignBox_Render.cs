namespace EdaBoardViewer.Views.Controls {

    using System;
    using System.Globalization;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;

    public sealed partial class DesignBox : Control {

        private const string xTagFormat = " X: {0:0.00} ";
        private const string yTagFormat = " Y: {0:0.00} ";
        private const string wTagFormat = " W: {0:0.00} ";
        private const string hTagFormat = " H: {0:0.00} ";

        /// <summary>
        /// Renderitza el control.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// 
        public override void Render(DrawingContext context) {

            if (IsPointerVisible)
                DrawPointer(context);
            if (IsRegionVisible)
                DrawRegion(context);

            if (IsPointerTagsVisible)
                DrawPointerTags(context);
            if (IsRegionTagsVisible)
                DrawRegionTags(context);
            if (IsRegionHandlesVisible)
                DrawRegionHandles(context);
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
            DrawLine(context, pen, new Point(0, y), new Point(Bounds.Width, y));
            DrawLine(context, pen, new Point(x, 0), new Point(x, Bounds.Height));
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

            double penWidth = 0.5;

            var borderPen = new Pen(new SolidColorBrush(PointerTagBorderColor), penWidth);
            var backgroundBrush = PointerTagBackground;
            var textBrush = new SolidColorBrush(PointerTagTextColor);

            ft.Text = String.Format(CultureInfo.InvariantCulture, xTagFormat, PointerPosition.X / ValueDivisor);
            var r1 = new Rect(
                x - ft.Bounds.Width - ft.Bounds.Height, 
                y - (ft.Bounds.Height / 2.0) , 
                ft.Bounds.Size.Width,
                ft.Bounds.Size.Height);

            DrawBox(context, backgroundBrush, borderPen, r1);
            context.DrawText(textBrush, r1.TopLeft, ft);

            ft.Text = String.Format(CultureInfo.InvariantCulture, yTagFormat, PointerPosition.Y / ValueDivisor);
            var r2 = new Rect(
                x - ft.Bounds.Width - ft.Bounds.Height, 
                y - (ft.Bounds.Height / 2.0), 
                ft.Bounds.Size.Width,
                ft.Bounds.Size.Height);

            Matrix m = Matrix.Identity;
            m *= Matrix.CreateTranslation(-x, -y);
            m *= Matrix.CreateRotation(90 * Math.PI / 180);
            m *= Matrix.CreateTranslation(x, y);
            using (context.PushPreTransform(m)) {
                DrawBox(context, backgroundBrush, borderPen, r2);
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
            DrawBox(context, RegionBackground, pen, new Rect(x, y, w, h));
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

            ft.Text = String.Format(CultureInfo.InvariantCulture, wTagFormat, RegionSize.Width / ValueDivisor);
            var r1 = new Rect(
                new Point(x - ft.Bounds.Width - ft.Bounds.Height, y - (ft.Bounds.Height / 2)), 
                ft.Bounds.Size);

            DrawBox(context, backgroundBrush, borderPen, r1);
            context.DrawText(textBrush, r1.TopLeft, ft);

            ft.Text = String.Format(CultureInfo.InvariantCulture, hTagFormat, RegionSize.Height / ValueDivisor);
            var r2 = new Rect(
                new Point(x - ft.Bounds.Width - ft.Bounds.Height, y - (ft.Bounds.Height / 2)), 
                ft.Bounds.Size);

            Matrix m = Matrix.Identity;
            m *= Matrix.CreateTranslation(-x, -y);
            m *= Matrix.CreateRotation(90 * Math.PI / 180);
            m *= Matrix.CreateTranslation(x, y);
            using (context.PushPreTransform(m)) {
                DrawBox(context, backgroundBrush, borderPen, r2);
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

            double x = (RegionPosition.X + Origin.X) * Scale;
            double y = (RegionPosition.Y + Origin.Y) * Scale;
            double w = RegionSize.Width * Scale;
            double h = RegionSize.Height * Scale;

            double x1 = x - margin;
            double x2 = x + (w / 2);
            double x3 = x + w + margin;

            double y1 = y - margin;
            double y2 = y + (h / 2);
            double y3 = y + h + margin;

            var borderPen = new Pen(new SolidColorBrush(RegionTagBorderColor), 0.5);
            var backgroundBrush = RegionTagBackground;

            DrawBox(context, backgroundBrush, borderPen, GetRect(x1, y1, size));
            DrawBox(context, backgroundBrush, borderPen, GetRect(x2, y1, size));
            DrawBox(context, backgroundBrush, borderPen, GetRect(x3, y1, size));
            DrawBox(context, backgroundBrush, borderPen, GetRect(x1, y2, size));
            DrawBox(context, backgroundBrush, borderPen, GetRect(x3, y2, size));
            DrawBox(context, backgroundBrush, borderPen, GetRect(x1, y3, size));
            DrawBox(context, backgroundBrush, borderPen, GetRect(x2, y3, size));
            DrawBox(context, backgroundBrush, borderPen, GetRect(x3, y3, size));
        }

        /// <summary>
        /// Dibuixa una linia ajustada a pixel.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// <param name="pen">El pen.</param>
        /// <param name="p1">Punt inicial.</param>
        /// <param name="p2">Punt final.</param>
        /// 
        private static void DrawLine(DrawingContext context, IPen pen, Point p1, Point p2) {

            double x1 = Math.Floor(p1.X) + pen.Thickness;
            double y1 = Math.Floor(p1.Y) + pen.Thickness;
            double x2 = Math.Floor(p2.X) + pen.Thickness;
            double y2 = Math.Floor(p2.Y) + pen.Thickness;

            Point pp1 = new Point(x1, y1);
            Point pp2 = new Point(x2, y2);
            context.DrawLine(pen, pp1, pp2);
        }

        /// <summary>
        /// Dibuixa una caixa ajstada a pixel.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// <param name="brush">La brotxa.</param>
        /// <param name="pen">El pen.</param>
        /// <param name="r">Rectangle de la caixa.</param>
        /// 
        private static void DrawBox(DrawingContext context, IBrush brush, IPen pen, Rect r) {

            double x = Math.Floor(r.X) + pen.Thickness;
            double y = Math.Floor(r.Y) + pen.Thickness;
            double w = Math.Ceiling(r.Width);
            double h = Math.Ceiling(r.Height);

            Rect rr = new Rect(x, y, w, h);
            context.FillRectangle(brush, rr);
            context.DrawRectangle(pen, rr);
        }

        private bool IsPointerVisible => ShowPointer;

        private bool IsPointerTagsVisible => IsPointerVisible && ShowPointerTags;

        private bool IsRegionVisible =>  ShowRegion;

        private bool IsRegionTagsVisible => IsRegionVisible && ShowRegionTags;

        private bool IsRegionHandlesVisible => IsRegionVisible;// && ShowRegionHandles;
    }
}
