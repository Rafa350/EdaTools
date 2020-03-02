namespace EdaBoardViewer.Views.Controls {

    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;

    public sealed partial class RulerControl : Control {

        private const double maxLines = 1000;

        /// <summary>
        /// Dibuixa el control.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        public override void Render(DrawingContext context) {

            // Precalcula els punts d'interes en coordinades fisiques.
            //
            Point ptPointer = Transform(PointerPosition);

            Point p1 = Transform(RegionPosition);
            Point p2 = Transform(new Point(RegionPosition.X + RegionSize.Width, RegionPosition.Y + RegionSize.Height));
            Point ptRegionStart = new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            Point ptRegionEnd = new Point(ptRegionStart.X + Math.Abs(p2.X - p1.X), ptRegionStart.Y + Math.Abs(p2.Y - p1.Y));

            // Renderitza la imatge
            //
            using (context.PushPreTransform(GetTransformationMatrix())) {

                DrawBackground(context);
                DrawRuler(context);
                DrawTags(context);

                if (ShowRegion)
                    DrawRegion(context, ptRegionStart, ptRegionEnd);

                if (ShowPointer)
                    DrawPointer(context, ptPointer);
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

            context.FillRectangle(
                Background,
                new Rect(0, 0, w, h));
        }

        /// <summary>
        /// Dibuixa el indicador de posicio.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// <param name="position">Posicio del punter.</param>
        /// 
        private void DrawPointer(DrawingContext context, Point position) {

            double x = Orientation == RulerOrientation.Horizontal ? position.X : position.Y;
            double y1 = 0;
            double y2 = Orientation == RulerOrientation.Horizontal ? Bounds.Height : Bounds.Width;

            context.DrawPixLine(
                new Pen(new SolidColorBrush(PointerColor), 0.5),
                new Point(x, y1),
                new Point(x, y2));
        }
        
        /// <summary>
        /// Dibuixa la regio.
        /// </summary>
        /// <param name="context">Context de renderitzat.</param>
        /// <param name="startPosition">Posicio inicial de la regio.</param>
        /// <param name="endPosition">Posicio final de la regio.</param>
        /// 
        private void DrawRegion(DrawingContext context, Point startPosition, Point endPosition) {

            double x1 = Orientation == RulerOrientation.Horizontal ? startPosition.X : startPosition.Y;
            double y1 = 0;
            double x2 = Orientation == RulerOrientation.Horizontal ? endPosition.X : endPosition.Y;
            double y2 = Orientation == RulerOrientation.Horizontal ? Bounds.Height : Bounds.Width;

            context.FillRectangle(
                new SolidColorBrush(RegionColor),
                new Rect(new Point(x1, y1), new Point(x2, y2)));
        }

        /// <summary>
        /// Dibuixa el regla.
        /// </summary>
        /// <param name="context">El context de renderitzat.</param>
        /// 
        private void DrawRuler(DrawingContext context) {

            double length = Orientation == RulerOrientation.Horizontal ? Bounds.Height : Bounds.Width;
            double smallLength = SmallTickLength == 0 ? length * 0.33 : SmallTickLength;
            double mediumLength = MediumTickLength == 0 ? length * 0.66 : MediumTickLength;
            double largeLength = LargeTickLength == 0 ? length * 1.0 : LargeTickLength;

            double smallInterval = SmallTickInterval;
            double mediumInterval = MediumTickInterval;
            double largeInterval = LargeTickInterval;

            Pen smallPen = new Pen(new SolidColorBrush(SmallTickColor), 0.5);
            Pen mediumPen = new Pen(new SolidColorBrush(SmallTickColor), 0.5);
            Pen largePen = new Pen(new SolidColorBrush(SmallTickColor), 0.5);

            int numLines = 0;
            double valueDivisor = ValueDivisor;
            double minValue = MinValue;
            double maxValue = MaxValue;
            for (double value = minValue - (minValue % valueDivisor); value <= maxValue; value += valueDivisor) {

                double v = value / valueDivisor;

                Point p = Transform(new Point(value, value));
                double x = Orientation == RulerOrientation.Horizontal ? p.X: p.Y;

                // Limita el numero de linies a dibuixar
                //
                if (numLines++ >= maxLines)
                    break;

                // Linia d'interval llarg
                //
                if ((v % largeInterval) == 0)
                    context.DrawPixLine(
                        largePen,
                        new Point(x, length - largeLength),
                        new Point(x, length));

                // Linia d'interval mitja
                //
                else if ((v % mediumInterval) == 0)
                    context.DrawPixLine(
                        mediumPen,
                        new Point(x, length - mediumLength),
                        new Point(x, length));

                // Linia d'interval curt
                //
                else if ((v % smallInterval) == 0)
                    context.DrawPixLine(
                        smallPen,
                        new Point(x, length - smallLength),
                        new Point(x, length));
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
            for (double value = MinValue - (MinValue % valueDivisor); value <= MaxValue; value += valueDivisor) {

                double v = value / ValueDivisor;

                Point p = Transform(new Point(value, value));
                double x = Orientation == RulerOrientation.Horizontal ? p.X : p.Y;

                if (numLines++ >= maxLines)
                    break;

                if ((v % tickInterval) == 0) {
                    ft.Text = v.ToString();
                    Matrix t = Matrix.Identity;
                    if (Orientation == RulerOrientation.Vertical) {
                        t *= Matrix.CreateScale(1, -1);
                        t *= Matrix.CreateTranslation(0, ft.Bounds.Height);
                    }
                    using (context.PushPreTransform(t))
                        context.DrawText(
                            TagBrush,
                            new Point(x + 2, 0), ft);
                }
            }
        }

        /// <summary>
        /// Transforma un punt a coordinades fisiques.
        /// </summary>
        /// <param name="p">El punt a transformar.</param>
        /// <returns>El resultat.</returns>
        /// 
        private Point Transform(Point p) {

            Matrix matrix = ValueMatrix;
            return new Point(
                (p.X * matrix.M11) + (p.Y * matrix.M21) + matrix.M31,
                (p.X * matrix.M12) + (p.Y * matrix.M22) + matrix.M32);
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
