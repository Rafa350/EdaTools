namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System.Windows;
    using System.Collections.Generic;

    public sealed class RegionElement: SingleLayerElement {

        public class Segment {

            public Point Position { get; set; }            
            public double Angle { get; set; }
        }

        private List<Segment> segments;
        private double rotate;
        private double thickness;
        private double isolation;

        public RegionElement():
            base() {

        }

        public RegionElement(Point position, Layer layer):
            base(position, layer) {

        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public RegionElement AddLine(Point delta) {

            if (segments == null)
                segments = new List<Segment>();

            segments.Add(new Segment {
                Position = delta,
                Angle = 0
            });

            return this;
        }

        public RegionElement AddArc(Point delta, double angle) {

            if (segments == null)
                segments = new List<Segment>();

            segments.Add(new Segment {
                Position = delta,
                Angle = angle
            });

            return this;
        }

        public double Rotate {
            get {
                return rotate;
            }
            set {
                rotate = value;
            }
        }

        public double Thickness {
            get {
                return thickness;
            }
            set {
                thickness = value;
            }
        }

        public double Isolation {
            get {
                return isolation;
            }
            set {
                isolation = value;
            }
        }

        public bool Filled {
            get {
                return thickness == 0;
            }
            set {
                if (value)
                    thickness = 0;
            }
        }

        public IEnumerable<Segment> Segments {
            get {
                return segments;
            }
        }
    }
}
