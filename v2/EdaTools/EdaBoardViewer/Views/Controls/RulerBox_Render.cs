namespace EdaBoardViewer.Views.Controls {

    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;

    public sealed partial class RulerBox: Control {

        private const double maxLines = 1000;

        private Geometry regionGeometryCache;
        private Geometry pointerGeometryCache;

        public RulerBox() {

            ClipToBounds = true;
        }

        public override void Render(DrawingContext context) {

            RenderRuler(context);
            if (ShowRegion)
                RenderRegion(context);
            if (ShowPointer)
                RenderPointer(context);
        }

        /// <summary>
        /// Renderitza el regla.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// 
        private void RenderRuler(DrawingContext context) {

            double valueDivisor = ValueDivisor;

            double width = Bounds.Width;
            double height = Bounds.Height;

            FormattedText ft = new FormattedText {
                Typeface = new Typeface(FontFamily, FontSize, FontStyle),
                TextAlignment = TextAlignment.Left
            };

            double y3 = Orientation == RulerOrientation.Horizontal ? height : width;
            double y2 = y3 * 0.66;
            double y1 = y3 * 0.33;
            double y0 = 0;

            double u1X = valueDivisor;
            double u5X = u1X * 5;
            double u10X = u1X * 10;

            var pen = new Pen(LineBrush, 1);

            context.FillRectangle(Background, new Rect(0, 0, width, height));

            using (context.PushPreTransform(GetTransformationMatrix())) {

                int numLines = 0;
                for (double m = MinValue - (MinValue % valueDivisor); m <= MaxValue; m += valueDivisor) {

                    var p = (m + Origin) * Scale;

                    // Limita el numero de linies a dibuixar
                    //
                    if (numLines++ >= maxLines)
                        break;

                    // Linia de 10 unitats
                    //
                    if ((m % u10X) == 0) {
                        context.DrawLine(pen, new Point(p, y0), new Point(p, y3));

                        ft.Text = (m / valueDivisor).ToString();
                        Matrix t = Matrix.Identity;
                        if (Orientation == RulerOrientation.Vertical) {
                            t *= Matrix.CreateScale(1, -1);
                            t *= Matrix.CreateTranslation(0, ft.Bounds.Height);
                        }
                        using (context.PushPreTransform(t))
                            context.DrawText(TagBrush, new Point(p + 2, y0), ft);
                    }
                    // Linia de 5 d'unitats
                    //
                    else if ((m % u5X) == 0) 
                        context.DrawLine(pen, new Point(p, y1), new Point(p, y3));

                    // Linia d'unitats
                    //
                    else
                        context.DrawLine(pen, new Point(p, y2), new Point(p, y3));
                }
            }
        }

        /// <summary>
        /// Renderitza el indicador de regio.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// 
        private void RenderRegion(DrawingContext context) {

            using (context.PushPreTransform(GetTransformationMatrix()))
                context.DrawGeometry(RegionBrush, null, GetRegionGeometry());
        }

        /// <summary>
        /// Renderitza el indicador de posicio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void RenderPointer(DrawingContext context) {

            var size = Orientation == RulerOrientation.Horizontal ? Bounds.Height : Bounds.Width;
            var p1 = new Point(PointerValue, 0);
            var p2 = new Point(PointerValue, size);

            var pen = new Pen(PointerBrush, 1);

            using (context.PushPreTransform(GetTransformationMatrix()))
                context.DrawLine(pen, p1, p2);
        }

        /// <summary>
        /// Obte la geometria de la regio. La recalcula o la obte de la cache.
        /// </summary>
        /// <returns>La geometria.</returns>
        /// 
        private Geometry GetRegionGeometry() {

            if (regionGeometryCache == null) {

                double x1 = ((Math.Min(RegionStartValue, RegionEndValue) / ValueDivisor) + Origin) * Scale;
                double y1 = 0;
                double x2 = ((Math.Max(RegionStartValue, RegionEndValue) / ValueDivisor) + Origin) * Scale;
                double y2 = Orientation == RulerOrientation.Horizontal ? Bounds.Height : Bounds.Width;

                StreamGeometry g = new StreamGeometry();
                using (StreamGeometryContext gc = g.Open()) {
                    gc.BeginFigure(new Point(x1, y1), true);
                    gc.LineTo(new Point(x2, y1));
                    gc.LineTo(new Point(x2, y2));
                    gc.LineTo(new Point(x1, y2));
                    gc.LineTo(new Point(x1, y1));
                }

                regionGeometryCache = g;
            }

            return regionGeometryCache;
        }

        /// <summary>
        /// Obte la geometria del punter. La recalcula o la obte del cache.
        /// </summary>
        /// <returns>La geometria.</returns>
        /// 
        private Geometry GetPointerGeometry() {

            if (pointerGeometryCache == null) {

            }

            return pointerGeometryCache;
        }

        /// <summary>
        /// Obte la matriu de transformacio.
        /// </summary>
        /// <returns>La matriu.</returns>
        /// 
        private Matrix GetTransformationMatrix() {

            Matrix m = Matrix.Identity;

            if (Orientation == RulerOrientation.Vertical) {
                m *= Matrix.CreateRotation((90.0 * Math.PI) / 180.0);
                m *= Matrix.CreateTranslation(Bounds.Width, 0);
            }

            if (Alignment == RulerAlignment.RightOrTop) {
                if (Orientation == RulerOrientation.Horizontal) {
                    m *= Matrix.CreateScale(1, -1);
                    m *= Matrix.CreateTranslation(0, Bounds.Height);
                }
                else {
                    m *= Matrix.CreateScale(-1, 1);
                    m *= Matrix.CreateTranslation(Bounds.Width, 0);
                }
            }

            return m;
        }
    }
}
