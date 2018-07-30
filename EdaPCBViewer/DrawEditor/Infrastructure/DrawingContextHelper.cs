namespace MikroPic.EdaTools.v1.Designer.DrawEditor.Infrastructure {

    using System;
    using System.Windows;
    using System.Windows.Media;

    internal static class DrawingContextHelper {

        /// <summary>
        /// Dibuixa un arc.
        /// </summary>
        /// <param name="dc">Contexte de dibuix.</param>
        /// <param name="pen">El pen.</param>
        /// <param name="start">Punt inicial.</param>
        /// <param name="end">Punt final.</param>
        /// <param name="size">Tamany de l'el·lipse.</param>
        /// <param name="angle">Angle de l'arc. El signe indica la direccio del traçat.</param>
        /// 
        public static void DrawArc(this DrawingContext dc, Pen pen, Point start, Point end, Size size, double angle) {

            StreamGeometry g = new StreamGeometry();
            using (StreamGeometryContext gc = g.Open()) {
                gc.BeginFigure(start, false, false);
                gc.ArcTo(end, size, angle, Math.Abs(angle) > 180.0, angle < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise, true, false);
            }
            g.Freeze();

            dc.DrawGeometry(null, pen, g);
        }

        public static void DrawOctagon(this DrawingContext dc, Pen pen, Brush brush, Point center, double radius) {

        }
    }
}
