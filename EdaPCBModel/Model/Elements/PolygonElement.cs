namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System.Windows;
    using System.Collections.Generic;

    public sealed class PolygonElement: SingleLayerElement {

        public class Segment {

            public Point Delta { get; set; }            
            public double Angle { get; set; }
        }

        private List<Segment> nodes;
        private Point position;
        private double rotate;
        private double thickness;

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public PolygonElement AddLine(Point delta) {

            if (nodes == null)
                nodes = new List<Segment>();

            nodes.Add(new Segment {
                Delta = delta,
                Angle = 0
            });

            return this;
        }

        public PolygonElement AddArc(Point delta, double angle) {

            if (nodes == null)
                nodes = new List<Segment>();

            nodes.Add(new Segment {
                Delta = delta,
                Angle = angle
            });

            return this;
        }

        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
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

        public bool Filled {
            get {
                return thickness == 0;
            }
            set {
                if (value)
                    thickness = 0;
            }
        }

        public IEnumerable<Segment> Nodes {
            get {
                return nodes;
            }
        }
    }
}
