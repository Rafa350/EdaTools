namespace EdaBoardViewer.Views.Controls {

    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;

    public sealed partial class RulerBox : Control {

        private const double maxLines = 1000;

        private Geometry rulerGeometryCache;

        public override void Render(DrawingContext context) {

            using (context.PushPreTransform(GetTransformationMatrix())) {

                DrawBackground(context);
                DrawRuler(context);
                if (IsRegionVisible())
                    DrawRegion(context);
                if (IsPointerVisible())
                    DrawPointer(context);
            }
        }

        /// <summary>
        /// Dibuixa el regla.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// 
        private void DrawRuler(DrawingContext context) {

            double valueDivisor = ValueDivisor;

            double width = Bounds.Width;
            double height = Bounds.Height;

            FormattedText ft = new FormattedText {
                Typeface = new Typeface(FontFamily, FontSize, FontStyle),
                TextAlignment = TextAlignment.Left
            };

            double u1X = valueDivisor;
            double u10X = u1X * 10;

            var pen = new Pen(LineBrush, 1);

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
                    ft.Text = (m / valueDivisor).ToString();
                    Matrix t = Matrix.Identity;
                    if (Orientation == RulerOrientation.Vertical) {
                        t *= Matrix.CreateScale(1, -1);
                        t *= Matrix.CreateTranslation(0, ft.Bounds.Height);
                    }
                    using (context.PushPreTransform(t))
                        context.DrawText(TagBrush, new Point(p + 2, 0), ft);
                }
            }

            context.DrawGeometry(null, pen, RulerGeometry());
        }


        /// <summary>
        /// Dibuixa el fons.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// 
        private void DrawBackground(DrawingContext context) {

            var brush = Background;
            var r = new Rect(0, 0, Bounds.Width, Bounds.Height);
            context.FillRectangle(brush, r);
        }

        /// <summary>
        /// Dibuixa la regio.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// 
        private void DrawRegion(DrawingContext context) {

            double x = (RegionPosition + Origin) * Scale;
            double y = 0;
            double w = RegionSize * Scale;
            double h = Orientation == RulerOrientation.Horizontal ? Bounds.Height : Bounds.Width;

            var brush = RegionBrush;
            var r = new Rect(x, y, w, h);
            context.FillRectangle(brush, r);
        }

        /// <summary>
        /// Renderitza el indicador de posicio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawPointer(DrawingContext context) {

            double x = (PointerPosition + Origin) * Scale;
            double y1 = 0;
            double y2 = Orientation == RulerOrientation.Horizontal ? Bounds.Height : Bounds.Width;

            var pen = new Pen(PointerBrush, 1);
            var p1 = new Point(x, y1);
            var p2 = new Point(x, y2);
            context.DrawLine(pen, p1, p2);
        }


        /// <summary>
        /// Obte la geometria del regla. La recalcula o la obte de la cache.
        /// </summary>
        /// <returns>La geometria.</returns>
        /// 
        private Geometry RulerGeometry() {

            if (rulerGeometryCache == null) {

                StreamGeometry g = new StreamGeometry();
                using (StreamGeometryContext gc = g.Open()) {

                    double valueDivisor = ValueDivisor;

                    double width = Bounds.Width;
                    double height = Bounds.Height;

                    double y3 = Orientation == RulerOrientation.Horizontal ? height : width;
                    double y2 = y3 * 0.66;
                    double y1 = y3 * 0.33;
                    double y0 = 0;

                    double u1X = valueDivisor;
                    double u5X = u1X * 5;
                    double u10X = u1X * 10;

                    int numLines = 0;
                    for (double m = MinValue - (MinValue % valueDivisor); m <= MaxValue; m += valueDivisor) {

                        var p = (m + Origin) * Scale;

                        // Limita el numero de linies a dibuixar
                        //
                        if (numLines++ >= maxLines)
                            break;

                        // Linia de 10 unitats
                        //
                        if ((m % u10X) == 0)
                            gc.BeginFigure(new Point(p, y0), false);

                        // Linia de 5 d'unitats
                        //
                        else if ((m % u5X) == 0)
                            gc.BeginFigure(new Point(p, y1), false);

                        // Linia d'unitats
                        //
                        else
                            gc.BeginFigure(new Point(p, y2), false);

                        gc.LineTo(new Point(p, y3));
                    }
                }

                rulerGeometryCache = g;
            }

            return rulerGeometryCache;
        }

        /// <summary>
        /// Comprova si el punter es visible.
        /// </summary>
        /// <returns>True si es visible.</returns>
        /// 
        private bool IsPointerVisible() {

            return ShowPointer && (PointerPosition >= MinValue) && (PointerPosition <= MaxValue);
        }

        /// <summary>
        /// Comprova si la regio es visinle.
        /// </summary>
        /// <returns>True si es visible.</returns>
        /// 
        private bool IsRegionVisible() {

            return ShowRegion;
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
