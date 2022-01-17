using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Common;
using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public enum PartSide {
        Top,
        Bottom
    }

    public sealed partial class EdaPart: IEdaPosition, IEdaRotation, IEdaName, IEdaVisitable<IEdaBoardVisitor> {

        private string _name;
        private EdaPoint _position;
        private EdaAngle _rotation = EdaAngle.Zero;
        private PartSide _side = PartSide.Top;
        private EdaComponent _component;

        /// <inheritdoc/>
        /// 
        public void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public override string ToString() {

            return String.Format("Part: '{0}'", _name);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public EdaRect GetBoundingBox(BoardSide side) {

            if (HasElements) {

                int left = int.MaxValue;
                int top = int.MaxValue;
                int right = int.MinValue;
                int bottom = int.MinValue;

                foreach (var element in Elements) {
                    EdaRect r = element.GetBoundingBox(side);
                    if (left > r.Left)
                        left = r.Left;
                    if (top > r.Top)
                        top = r.Top;
                    if (right < r.Right)
                        right = r.Right;
                    if (bottom < r.Bottom)
                        bottom = r.Bottom;
                }

                return new EdaRect(left, top, right - left + 1, top - bottom + 1);
            }
            else
                return new EdaRect();
        }

        /// <summary>
        /// Obte un pad pel seu nom.
        /// </summary>
        /// <param name="name">El nom del pad.</param>
        /// <param name="throwOnError">True si dispara una execepcio si no el troba.</param>
        /// <returns>El pad. Null si no el troba.</returns>
        /// 
        public EdaPadElement GetPad(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var pad = _component.GetPad(name, false);
            if (pad != null)
                return pad;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el pad '{0}' en el part '{1}'.", name, _name));

            else
                return null;
        }

        /// <summary>
        /// El nom.
        /// </summary>
        /// 
        public string Name {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// El component
        /// </summary>
        /// 
        public EdaComponent Component {
            get => _component;
            set => _component = value;
        }

        /// <summary>
        /// La posicio.
        /// </summary>
        /// 
        public EdaPoint Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// L'angle de rotacio.
        /// </summary>
        /// 
        public EdaAngle Rotation {
            get => _rotation;
            set => _rotation = value;
        }

        /// <summary>
        /// La cara de la placa on es el component.
        /// </summary>
        /// 
        public PartSide Side {
            get => _side;
            set => _side = value;
        }

        /// <summary>
        /// Indica si el component esta girat.
        /// </summary>
        /// 
        public bool IsFlipped =>
            _side == PartSide.Bottom;

        /// <summary>
        /// Indica si conte elements
        /// </summary>
        /// 
        public bool HasElements =>
            _component.HasElements;

        /// <summary>
        /// Enumera els elements.
        /// </summary>
        /// 
        public IEnumerable<EdaElement> Elements =>
            _component.Elements;

        /// <summary>
        /// Indica si conte pads.
        /// </summary>
        /// 
        public bool HasPads =>
            _component.HasPads();

        /// <summary>
        /// Enumera els pads
        /// </summary>
        /// 
        public IEnumerable<EdaPadElement> Pads =>
            _component.Pads();
    }
}
