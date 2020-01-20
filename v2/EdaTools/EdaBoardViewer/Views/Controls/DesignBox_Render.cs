namespace EdaBoardViewer.Views.Controls {

    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;

    public sealed partial class DesignBox: Control {

        private Geometry regionTagsGeometryCache;
        //private Geometry handlesGeometryCache;

        public override void Render(DrawingContext context) {

            DrawPointer(context);
            if (IsRegionVisible()) {
                DrawRegion(context);
                if (IsRegionTagsVisible())
                    DrawRegionTags(context);
            }
        }

        /// <summary>
        /// Renderitza el indicador de posicio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawPointer(DrawingContext context) {

            FormattedText ft = new FormattedText();
            ft.TextAlignment = TextAlignment.Left;
            ft.Typeface = new Typeface(FontFamily, FontSize, FontStyle);

            string strPositionX = (PointerPosition.X / ValueDivisor).ToString("0.00");
            string strPositionY = (PointerPosition.Y / ValueDivisor).ToString("0.00");

            ft.Text = strPositionX;
            double tagHWidth = ft.Bounds.Width;
            double tagHHeight = ft.Bounds.Height;

            double tagOffset = 5;

            double x = (PointerPosition.X + Origin.X) * Scale;
            double y = (PointerPosition.Y + Origin.Y) * Scale;

            var pen = new Pen(PointerBrush, 1);
            context.DrawLine(pen, new Point(0, y), new Point(Bounds.Width, y));
            context.DrawLine(pen, new Point(x, 0), new Point(x, Bounds.Height));
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

            var pen = new Pen(RegionBorderBrush, 1);
            var r = new Rect(x, y, w, h);
            context.FillRectangle(RegionBackground, r);
            context.DrawRectangle(pen, r);
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

            double tagOffset = 5;

            // Tamany de la regio
            //
            double regionWidth = RegionSize.Width * Scale;
            double regionHeight = RegionSize.Height * Scale;

            string strWidth = (RegionSize.Width / ValueDivisor).ToString("0.00");
            string strHeight = (RegionSize.Height / ValueDivisor).ToString("0.00");

            // Tamany de l'etiqueta horizontal.
            //
            ft.Text = strWidth;
            double tagHWidth = ft.Bounds.Width * 1.2;
            double tagHHeight = ft.Bounds.Height;

            // Tamany de l'etiqueta vertical.
            //
            ft.Text = strHeight;
            double tagVWidth = ft.Bounds.Width * 1.2;
            double tagVHeight = ft.Bounds.Height;

            // Calcula els punts de control.
            //
            double x1 = (RegionPosition.X + Origin.X) * Scale;
            double x2 = x1 + (regionWidth - tagHWidth) / 2;
            double x3 = x1 + (regionWidth / 2);
            double x4 = x3 + tagHWidth;
            double x5 = x1 + regionWidth;
            double x6 = x5 + tagOffset;
            double x7 = x6 + tagVHeight / 2;
            double x8 = x6 + tagVHeight;

            double y1 = (RegionPosition.Y + Origin.Y) * Scale;
            double y2 = y1 + (regionHeight - tagHWidth) / 2;
            double y3 = y1 + (regionHeight / 2);
            double y4 = y2 + tagVWidth;
            double y5 = y1 + regionHeight;
            double y6 = y5 + tagOffset;
            double y7 = y6 + tagHHeight / 2;
            double y8 = y6 + tagHHeight;

            var pen = new Pen(RegionTagBorderBrush, 1);
            var brush = RegionTagBackground;
            context.DrawLine(pen, new Point(x1, y6), new Point(x1, y8));
            context.DrawLine(pen, new Point(x1, y7), new Point(x2, y7));
            context.DrawLine(pen, new Point(x4, y7), new Point(x5, y7));
            context.DrawLine(pen, new Point(x5, y6), new Point(x5, y8));
            context.FillRectangle(brush, new Rect(x2, y6, x4 - x2, y8 - y6));
            context.DrawRectangle(pen, new Rect(x2, y6, x4 - x2, y8 - y6));
            context.DrawLine(pen, new Point(x6, y1), new Point(x8, y1));
            context.DrawLine(pen, new Point(x7, y1), new Point(x7, y2));
            context.DrawLine(pen, new Point(x7, y4), new Point(x7, y5));
            context.DrawLine(pen, new Point(x6, y5), new Point(x8, y5));
            context.FillRectangle(brush, new Rect(x6, y2, x7 - x5, y4 - y2));
            context.DrawRectangle(pen, new Rect(x6, y2, x7 - x5, y4 - y2));

            ft.Text = strWidth;
            context.DrawText(RegionTagBorderBrush, new Point(x3 - (tagHWidth / 2), y7 - (tagHHeight / 2)), ft);

            ft.Text = strHeight;
            Matrix m = Matrix.Identity;
            m *= Matrix.CreateTranslation(-x7, -y3);
            m *= Matrix.CreateRotation(90 * Math.PI / 180);
            m *= Matrix.CreateTranslation(x7, y3);
            using (context.PushPreTransform(m))
                context.DrawText(RegionTagBorderBrush, new Point(x7 - (tagVWidth / 2), y3 - (tagVHeight / 2)), ft);
        }

        private void RenderPositionTag(DrawingContext context) {

        }

        private bool IsRegionVisible() {

            return ShowRegion;
        }

        private bool IsRegionTagsVisible() {

            return IsRegionVisible();
        }

        private Geometry GetRegionTagsGeometry() {

            if (regionTagsGeometryCache == null) {

                FormattedText ft = new FormattedText();
                ft.TextAlignment = TextAlignment.Center;
                ft.Typeface = new Typeface(FontFamily, FontSize, FontStyle);

                double tagOffset = 5;

                // Tamany de la regio
                //
                var regionWidth = RegionSize.Width * Scale;
                var regionHeight = RegionSize.Height * Scale;

                // Tamany de l'etiqueta horital.
                //
                ft.Text = String.Format(" xx{0}xx ", RegionSize.Width / ValueDivisor);
                var tagHWidth = ft.Bounds.Width;
                var tagHHeight = ft.Bounds.Height;

                // Tamany de l'etiqueta vertical.
                //
                ft.Text = String.Format(" yy{0}yy ", RegionSize.Height / ValueDivisor);
                var tagVWidth = ft.Bounds.Width;
                var tagVHeight = ft.Bounds.Height;

                // Calcula els punts de control.
                //
                var x1 = (RegionPosition.X + Origin.X) * Scale;
                var x2 = x1 + (regionWidth - tagHWidth) / 2;
                var x3 = x2 + tagHWidth;
                var x4 = x1 + regionWidth;
                var x5 = x4 + tagOffset;
                var x6 = x5 + tagVHeight / 2;
                var x7 = x5 + tagVHeight;

                var y1 = (RegionPosition.Y + Origin.Y) * Scale;
                var y2 = y1 + (regionHeight - tagHWidth) / 2;
                var y3 = y2 + tagVWidth;
                var y4 = y1 + regionHeight;
                var y5 = y4 + tagOffset;
                var y6 = y5 + tagHHeight / 2;
                var y7 = y5 + tagHHeight;

                StreamGeometry g = new StreamGeometry();
                using (StreamGeometryContext gc = g.Open()) {
                    gc.BeginFigure(new Point(x1, y6), false);
                    gc.LineTo(new Point(x2, y6));
                    gc.BeginFigure(new Point(x3, y6), false);
                    gc.LineTo(new Point(x4, y6));

                    gc.BeginFigure(new Point(x2, y5), true);
                    gc.LineTo(new Point(x3, y5));
                    gc.LineTo(new Point(x3, y7));
                    gc.LineTo(new Point(x2, y7));
                    gc.EndFigure(true);

                    gc.BeginFigure(new Point(x6, y1), false);
                    gc.LineTo(new Point(x6, y2));
                    gc.BeginFigure(new Point(x6, y3), false);
                    gc.LineTo(new Point(x6, y4));

                    gc.BeginFigure(new Point(x5, y2), true);
                    gc.LineTo(new Point(x7, y2));
                    gc.LineTo(new Point(x7, y3));
                    gc.LineTo(new Point(x5, y3));
                    gc.EndFigure(true);
                }

                regionTagsGeometryCache = g;
            }

            return regionTagsGeometryCache;
        }
    }
}
