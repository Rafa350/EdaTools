using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace TestGraphics.Views.Controls {

    internal class RenderControl: Control {

        public RenderControl() {

        }

        public override void Render(DrawingContext context) {

            var size = Bounds.Size;

            double radius = 50;
            var p1 = new Point(100, 500);
            var p2 = new Point(500, 500);
            var pCorner = new Point(200, 100);

            Point p1Cross, p2Cross, pCenter;
            RoundCorner(p1, pCorner, p2, radius, out p1Cross, out pCenter, out p2Cross);

            var blackPen = new Pen(Brushes.Black, 2, null, PenLineCap.Round, PenLineJoin.Round);
            var redPen = new Pen(Brushes.Red, 2, null, PenLineCap.Round, PenLineJoin.Round);

            context.DrawLine(blackPen, p1, pCorner);
            context.DrawLine(blackPen, pCorner, p2);

            context.DrawEllipse(Brushes.Green, null, p1, 5, 5);
            context.DrawEllipse(Brushes.Green, null, p2, 5, 5);
            context.DrawEllipse(Brushes.Green, null, pCorner, 5, 5);
            context.DrawEllipse(Brushes.Green, null, p1Cross, 5, 5);
            context.DrawEllipse(Brushes.Green, null, p2Cross, 5, 5);
            context.DrawEllipse(Brushes.Green, null, pCenter, 5, 5);
        }

        private static void RoundCorner(Point p1, Point corner, Point p2, double radius, out Point cross1, out Point center, out Point cross2) {

            double Length(double dx, double dy) =>
                Math.Sqrt(dx * dx + dy * dy);

            Point ProportionPoint(Point point, double factor, double dx, double dy) =>
                new(point.X - (dx * factor), point.Y - (dy * factor));

            // Vector 1
            //
            double dx1 = corner.X - p1.X;
            double dy1 = corner.Y - p1.Y;

            // Vector 2
            //
            double dx2 = corner.X - p2.X;
            double dy2 = corner.Y - p2.Y;

            // Angle between vector 1 and vector 2 divided by 2
            //
            double angle = (Math.Atan2(dy1, dx1) - Math.Atan2(dy2, dx2)) / 2.0;

            // The length of segment between angular point and the
            // points of intersection with the circle of a given radius
            //
            double tan = Math.Abs(Math.Tan(angle));
            double segment = radius / tan;

            // Check the segment
            //
            double length1 = Length(dx1, dy1);
            double length2 = Length(dx2, dy2);

            double length = Math.Min(length1, length2);

            if (segment > length) {
                segment = length;
                radius = length * tan;
            }

            // Points of intersection are calculated by the proportion between 
            // the coordinates of the vector, length of vector and the length of the segment.
            //
            cross1 = ProportionPoint(corner, segment / length1, dx1, dy1);
            cross2 = ProportionPoint(corner, segment / length2, dx2, dy2);

            // Calculation of the coordinates of the circle 
            // center by the addition of angular vectors.
            //
            double dx = (corner.X * 2.0) - cross1.X - cross2.X;
            double dy = (corner.Y * 2.0) - cross1.Y - cross2.Y;

            double l = Length(dx, dy);
            double d = Length(segment, radius);
            center = ProportionPoint(corner, d / l, dx, dy);
        }
    }
}
