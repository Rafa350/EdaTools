namespace MikroPic.EdaTools.v1.Model.Elements {

    using System;
    using System.Windows;

    public class LineElement: ElementBase {

        public enum LineCapStyle {
            Round,
            Flat
        }

        private Point startPosition;
        private Point endPosition;
        private double thickness;
        private LineCapStyle lineCap = LineCapStyle.Round;

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public Point StartPosition {
            get {
                return startPosition;
            }
            set {
                startPosition = value;
            }
        }

        public Point EndPosition {
            get {
                return endPosition;
            }
            set {
                endPosition = value;
            }
        }

        public double Thickness {
            get {
                return thickness;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Thickness");
                thickness = value;
            }
        }

        public LineCapStyle LineCap {
            get {
                return lineCap;
            }
            set {
                lineCap = value;
            }
        }
    }
}
