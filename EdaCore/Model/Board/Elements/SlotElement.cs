namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    using System;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

    public sealed class SlotElement : PadElement {

        private int _drill;
        private int _length;
        private int _topSize;
        private int _innerSize;
        private int _bottomSize;

        SlotElement(string name, LayerSet layerSet, Point position, Angle rotation, int topSize,
            int innerSize, int bottomSize, int drill, int length) :
            base(name, layerSet, position, rotation) {

            if (topSize < 0)
                throw new ArgumentOutOfRangeException(nameof(topSize));

            if (innerSize < 0)
                throw new ArgumentOutOfRangeException(nameof(innerSize));

            if (bottomSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bottomSize));

            if (drill <= 0)
                throw new ArgumentOutOfRangeException(nameof(drill));

            _topSize = topSize;
            _innerSize = innerSize;
            _bottomSize = bottomSize;
            _drill = drill;
            _length = length;
        }

        public override Element Clone() {

            return new SlotElement(Name, LayerSet, Position, Rotation, _topSize, _innerSize, _bottomSize, _drill, _length);
        }

        public override void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        public override Rect GetBoundingBox(BoardSide side) {

            double a = Rotation.ToRadiants;

            int size = _topSize;
            switch (side) {
                case BoardSide.Bottom:
                    size = _bottomSize;
                    break;

                case BoardSide.Inner:
                    size = _innerSize;
                    break;
            }

            int w = (int)(_length * Math.Cos(a) + size * Math.Sin(a));
            int h = (int)(_length * Math.Sin(a) + size * Math.Cos(a));

            return new Rect(Position.X - (w / 2), Position.Y - (h / 2), w, h);
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
                return _length;
            }
        }

        public int TopSize {
            get {
                return _topSize;
            }
        }

        public int InnerSize {
            get {
                return _innerSize;
            }
        }

        public int BottomSize {
            get {
                return _bottomSize;
            }
        }

        /// <summary>
        /// Obte o asigna el diametre del forst.
        /// </summary>
        /// 
        public int Drill {
            get {
                return _drill;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("SlotElement.Drill");
                _drill = value;
            }
        }

        /// <summary>
        /// Obte el tipus d'element.
        /// </summary>
        /// 
        public override ElementType ElementType =>
            ElementType.SlotPad;

    }
}
