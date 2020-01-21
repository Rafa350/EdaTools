namespace EdaBoardViewer.Views.Controls {

    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;

    public sealed partial class RulerBox : Control {

        private const double maxLines = 1000;

        public override void Render(DrawingContext context) {

            using (context.PushPreTransform(GetTransformationMatrix())) {

                DrawBackground(context);
                DrawRuler(context);
                DrawTags(context);
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
        private void DrawTags(DrawingContext context) {

            FormattedText ft = new FormattedText {
                Typeface = new Typeface(FontFamily, FontSize, FontStyle),
                TextAlignment = TextAlignment.Left
            };

            double valueDivisor = ValueDivisor;
            double tickInterval = LargeTickInterval;

            int numLines = 0;
            for (double m = MinValue - (MinValue % valueDivisor); m <= MaxValue; m += valueDivisor) {

                var p = (m + Origin) * Scale;

                if (numLines++ >= maxLines)
                    break;

                if ((m % tickInterval) == 0) {
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
        }


        /// <summary>
        /// Dibuixa el fons.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// 
        private void DrawBackground(DrawingContext context) {

            double w = Orientation == RulerOrientation.Horizontal ? Bounds.Width : Bounds.Height;
            double h = Orientation == RulerOrientation.Horizontal ? Bounds.Height : Bounds.Width;

            var brush = Background;
            var r = new Rect(0, 0, w, h);
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

            var brush = new SolidColorBrush(RegionColor);
            var r = new Rect(x + 0.5, y + 0.5, w, h);
            context.FillRectangle(brush, r);
        }

        /// <summary>
        /// Dibuixa el indicador de posicio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawPointer(DrawingContext context) {

            double x = (PointerPosition + Origin) * Scale;
            double y1 = 0;
            double y2 = Orientation == RulerOrientation.Horizontal ? Bounds.Height : Bounds.Width;

            var pen = new Pen(new SolidColorBrush(PointerColor), 0.5);
            var p1 = new Point(x + 0.5, y1 + 0.5);
            var p2 = new Point(x + 0.5, y2 + 0.5);
            context.DrawLine(pen, p1, p2);
        }


        /// <summary>
        /// Dibuixa el regla.
        /// </summary>
        /// <returns>La geometria.</returns>
        /// 
        private void DrawRuler(DrawingContext context) {

            double valueDivisor = ValueDivisor;

            double length = Orientation == RulerOrientation.Horizontal ? Bounds.Height : Bounds.Width;
            double smallLength = SmallTickLength == 0 ? length * 0.33 : SmallTickLength;
            double mediumLength = MediumTickLength == 0 ? length * 0.66 : MediumTickLength;
            double largeLength = LargeTickLength == 0? length * 1.0 : LargeTickLength;

            double smallInterval = SmallTickInterval;
            double mediumInterval = MediumTickInterval;
            double largeInterval = LargeTickInterval;

            Pen smallPen = new Pen(new SolidColorBrush(SmallTickColor), 0.5);
            Pen mediumPen = new Pen(new SolidColorBrush(SmallTickColor), 0.5);
            Pen largePen = new Pen(new SolidColorBrush(SmallTickColor), 0.5);

            int numLines = 0;
            for (double m = MinValue - (MinValue % valueDivisor); m <= MaxValue; m += valueDivisor) {

                var p = (m + Origin) * Scale;

                // Limita el numero de linies a dibuixar
                //
                if (numLines++ >= maxLines)
                    break;

                // Linia d'interval llarg
                //
                if ((m % largeInterval) == 0)
                    context.DrawLine(largePen, new Point(p + 0.5, length - largeLength + 0.5), new Point(p + 0.5, length + 0.5));

                // Linia d'interval mitja
                //
                else if ((m % mediumInterval) == 0)
                    context.DrawLine(mediumPen, new Point(p + 0.5, length - mediumLength + 0.5), new Point(p + 0.5, length + 0.5));

                // Linia d'interval curt
                //
                else if ((m % smallInterval) == 0)
                    context.DrawLine(smallPen, new Point(p + 0.5, length - smallLength + 0.5), new Point(p + 0.5, length + 0.5));
            }
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

            if (Alignment == RulerAlignment.Top) {
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
