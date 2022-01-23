using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un pad superficial
    /// </summary>
    /// 
    public sealed class EdaSmdPadElement: EdaPadElement {

        public enum SmdPadCornerShape {
            Round,
            Flat
        }

        private EdaSize _size;
        private EdaRatio _cornerRatio = EdaRatio.Zero;
        private SmdPadCornerShape _cornerShape = SmdPadCornerShape.Round;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        public override int GetHashCode() =>
            Position.GetHashCode() +
            Size.GetHashCode() +
            (Rotation.GetHashCode() * 73429) +
            _cornerRatio.GetHashCode();

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            int hash = GetHashCode() + layerId.GetHashCode() * 981;
            EdaPolygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {

                var points = EdaPoints.CreateRectangle(Position, Size, _cornerRatio, true, Rotation);
                polygon = new EdaPolygon(points);

                PolygonCache.Save(hash, polygon);
            }
            return polygon;
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            int hash = GetHashCode() + (layerId.GetHashCode() * 71) + (spacing * 27009);
            EdaPolygon polygon = PolygonCache.Get(hash);
            if (polygon == null) {

                var outlineSize = new EdaSize(_size.Width + spacing + spacing, _size.Height + spacing + spacing);
                var outlineCornerRatio = EdaRatio.FromPercent((double)(CornerSize + spacing) / (Math.Min(outlineSize.Width, outlineSize.Height) / 2));
                var points = EdaPoints.CreateRectangle(Position, outlineSize, outlineCornerRatio, true, Rotation);
                polygon = new EdaPolygon(points);

                PolygonCache.Save(hash, polygon);
            }

            return polygon;
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetThermalPolygon(EdaLayerId layerId, int spacing, int width) {

            EdaPolygon pour = GetOutlinePolygon(layerId, spacing);
            EdaPolygon thermal = new EdaPolygon(
                EdaPoints.CreateCross(
                    Position,
                    new EdaSize(
                        _size.Width + spacing + spacing,
                        _size.Height + spacing + spacing),
                    width,
                    Rotation));

            List<EdaPolygon> childs = new List<EdaPolygon>();
            childs.AddRange(PolygonProcessor.Clip(pour, thermal, PolygonProcessor.ClipOperation.Diference));
            //if (childs.Count != 4)
            //    throw new InvalidProgramException("Thermal generada incorrectamente.");
            return new EdaPolygon(null, childs);
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) {

            double a = Rotation.AsRadiants;

            int w = (int)(_size.Width * Math.Cos(a) + _size.Height * Math.Sin(a));
            int h = (int)(_size.Width * Math.Sin(a) + _size.Height * Math.Cos(a));

            return new EdaRect(Position.X - (w / 2), Position.Y - (h / 2), w, h);
        }

        /// <summary>
        /// El tamany del pad.
        /// </summary>
        /// 
        public EdaSize Size {
            get => _size;
            set => _size = value;
        }

        /// <summary>
        /// El factor d'arrodoniment de les cantonades del pad.
        /// </summary>
        /// 
        public EdaRatio CornerRatio {
            get => _cornerRatio;
            set => _cornerRatio = value;
        }

        /// <summary>
        /// Radi de curvatura de les cantonades.
        /// </summary>
        /// 
        public int CornerSize =>
            (Math.Min(_size.Width, _size.Height) * _cornerRatio) / 2;

        /// <summary>
        /// Forma de les cantonades.
        /// </summary>
        /// 
        public SmdPadCornerShape CornerShape {
            get => _cornerShape;
            set => _cornerShape = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.SmdPad;
    }
}
