using System;
using MikroPic.EdaTools.v1.Base.Geometry;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un pad SMT
    /// </summary>
    /// 
    public sealed class EdaSmtPadElement: EdaPadElement {

        public enum SmdPadCornerShape {
            Round,
            Flat
        }

        private EdaSize _size;
        private EdaRatio _cornerRatio = EdaRatio.Zero;
        private SmdPadCornerShape _cornerShape = SmdPadCornerShape.Round;
        private EdaRatio _pasteReductionRatio = EdaRatio.Zero;
        private bool _pasteEnabled = true;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override int GetHashCode() =>
            Position.GetHashCode() +
            Size.GetHashCode() +
            (Rotation.GetHashCode() * 73429) +
            _cornerRatio.GetHashCode();

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            var points = EdaPointFactory.CreateRectangle(Position, Size, _cornerRatio, true, Rotation);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            var outlineSize = new EdaSize(_size.Width + spacing + spacing, _size.Height + spacing + spacing);
            var cornerSize = (Math.Min(_size.Width, _size.Height) * _cornerRatio) / 2;
            var outlineCornerRatio = EdaRatio.FromPercent((double)(cornerSize + spacing) / (Math.Min(outlineSize.Width, outlineSize.Height) / 2));
            var points = EdaPointFactory.CreateRectangle(Position, outlineSize, outlineCornerRatio, true, Rotation);
            return new EdaPolygon(points);

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
        /// El percentatge d'arrodoniment de les cantonades del pad.
        /// </summary>
        /// 
        public EdaRatio CornerRatio {
            get => _cornerRatio;
            set => _cornerRatio = value;
        }

        /// <summary>
        /// Forma de les cantonades.
        /// </summary>
        /// 
        public SmdPadCornerShape CornerShape {
            get => _cornerShape;
            set => _cornerShape = value;
        }

        /// <summary>
        /// Espai entre el pad i la pasta de soldadura.
        /// </summary>
        /// 
        public EdaRatio PasteReductionRatio {
            get => _pasteReductionRatio;
            set => _pasteReductionRatio = value;
        }

        /// <summary>
        /// Activa o desactiva la pasta de soldadura.
        /// </summary>
        /// 
        public bool PasteEnabled {
            get => _pasteEnabled;
            set => _pasteEnabled = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.SmtPad;
    }
}
