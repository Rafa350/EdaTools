namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using System;

    public sealed class SlotElement: PadElement {

        private int drill;
        private int length;
        private int topSize;
        private int innerSize;
        private int bottomSize;

        SlotElement(string name, LayerSet layerSet, Point position, Angle rotation, int topSize, 
            int innerSize, int bottomSize, int drill, int length):
            base(name, layerSet, position, rotation) {

            if (topSize < 0)
                throw new ArgumentOutOfRangeException("topSize");

            if (innerSize < 0)
                throw new ArgumentOutOfRangeException("innerSize");

            if (bottomSize < 0)
                throw new ArgumentOutOfRangeException("bottomSize");

            if (drill <= 0)
                throw new ArgumentOutOfRangeException("drill");

            this.topSize = topSize;
            this.innerSize = innerSize;
            this.bottomSize = bottomSize;
            this.drill = drill;
            this.length = length;
        }

        public override Element Clone() {

            return new SlotElement(Name, LayerSet, Position, Rotation, topSize, innerSize, bottomSize, drill, length);
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public override Rect GetBoundingBox(BoardSide side) {
            throw new System.NotImplementedException();
        }

        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {
            throw new System.NotImplementedException();
        }

        public override Polygon GetPolygon(BoardSide side) {
            throw new System.NotImplementedException();
        }

        public override Polygon GetThermalPolygon(BoardSide side, int spacing, int width) {
            throw new System.NotImplementedException();
        }

        public int Length {
            get {
                return length;
            }
        }

        public int TopSize {
            get {
                return topSize;
            }
        }

        public int InnerSize {
            get {
                return innerSize;
            }
        }

        public int BottomSize {
            get {
                return bottomSize;
            }
        }

        /// <summary>
        /// Obte o asigna el diametre del forst.
        /// </summary>
        /// 
        public int Drill {
            get {
                return drill;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("SlotElement.Drill");
                drill = value;
            }
        }

        /// <summary>
        /// Obte el tipus d'element.
        /// </summary>
        /// 
        public override ElementType ElementType {
            get {
                return ElementType.SlotPad;
            }
        }
    }
}
