﻿using System;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un text.
    /// </summary>
    /// 
    public sealed class EdaTextElement : EdaElement, IEdaPosition, IEdaRotation {

        private EdaPoint _position;
        private EdaAngle _rotation;
        private int _height;
        private int _thickness;
        private HorizontalTextAlign _horizontalAlign;
        private VerticalTextAlign _verticalAlign;
        private string _value;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            return null;
        }

        /// <inheritdoc/>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            return null;
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(BoardSide side) {

            throw new NotImplementedException();
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre del cercle.
        /// </summary>
        /// 
        public EdaPoint Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// Obte o asigna l'angle de rotacio.
        /// </summary>
        /// 
        public EdaAngle Rotation {
            get => _rotation;
            set => _rotation = value;
        }

        /// <summary>
        /// Obte o asigna l'alçada de lletra.
        /// </summary>
        /// 
        public int Height {
            get => _height;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Height");

                _height = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'amplada de linia.
        /// </summary>
        /// 
        public int Thickness {
            get => _thickness;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Thickness");

                _thickness = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'aliniacio horitzontal del text.
        /// </summary>
        /// 
        public HorizontalTextAlign HorizontalAlign {
            get => _horizontalAlign;
            set => _horizontalAlign = value;
        }

        /// <summary>
        /// Obte o asigna l'aliniacio vertical del text.
        /// </summary>
        /// 
        public VerticalTextAlign VerticalAlign {
            get => _verticalAlign;
            set => _verticalAlign = value;
        }

        /// <summary>
        /// Obte o asigna el valor del text.
        /// </summary>
        /// 
        public string Value {
            get => _value;
            set => _value = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Text;
    }
}
