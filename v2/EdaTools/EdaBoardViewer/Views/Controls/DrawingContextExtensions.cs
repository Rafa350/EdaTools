namespace EdaBoardViewer.Views.Controls {

    using System;
    using Avalonia;
    using Avalonia.Media;

    internal static class DrawingContextExtensions {

        /// <summary>
        /// Dibuixa una linia ajustada a pixel.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// <param name="pen">El pen.</param>
        /// <param name="p1">Punt inicial.</param>
        /// <param name="p2">Punt final.</param>
        /// 
        public static void DrawPixLine(this DrawingContext context, IPen pen, Point p1, Point p2) {

            double thickness = pen.Thickness;

            double x1 = Math.Floor(p1.X) + thickness;
            double y1 = Math.Floor(p1.Y) + thickness;
            double x2 = Math.Floor(p2.X) + thickness;
            double y2 = Math.Floor(p2.Y) + thickness;

            Point pp1 = new Point(x1, y1);
            Point pp2 = new Point(x2, y2);

            context.DrawLine(pen, pp1, pp2);
        }

        /// <summary>
        /// Dibuixa una caixa ajutada a pixel.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// <param name="brush">La brotxa.</param>
        /// <param name="pen">El pen.</param>
        /// <param name="r">Rectangle de la caixa.</param>
        /// 
        public static void DrawPixBox(this DrawingContext context, IBrush brush, IPen pen, Rect r) {

            double thickness = pen.Thickness;

            double x = Math.Floor(r.X) + thickness;
            double y = Math.Floor(r.Y) + thickness;
            double w = Math.Ceiling(r.Width);
            double h = Math.Ceiling(r.Height);

            Rect rr = new Rect(x, y, w, h);

            context.FillRectangle(brush, rr);
            context.DrawRectangle(pen, rr);
        }
    }
}
