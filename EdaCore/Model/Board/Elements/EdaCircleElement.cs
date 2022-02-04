﻿using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using System;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa un cercle.
    /// </summary>
    /// 
    public sealed class EdaCircleElement: EdaElement {

        private EdaPoint _position;
        private int _radius;
        private int _thickness;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetPolygon(EdaLayerId layerId) {

            if (Filled) {
                var points = EdaPoints.CreateCircle(_position, _radius);
                return new EdaPolygon(points);
            }
            else {
                var outerPoints = EdaPoints.CreateCircle(_position, _radius + (_thickness / 2));
                var innerPoints = EdaPoints.CreateCircle(_position, _radius - (_thickness / 2));
                return new EdaPolygon(outerPoints, new EdaPolygon(innerPoints));
            }
        }

        /// <inheritdoc/>
        /// 
        public override EdaPolygon GetOutlinePolygon(EdaLayerId layerId, int spacing) {

            var points = EdaPoints.CreateCircle(_position, _radius + (_thickness / 2) + spacing);
            return new EdaPolygon(points);
        }

        /// <inheritdoc/>
        /// 
        public override EdaRect GetBoundingBox(EdaLayerId layerId) {

            int r = _radius + (_thickness / 2);
            return new EdaRect(_position.X - r, _position.Y - r, r + r, r + r);
        }

        /// <summary>
        ///  La posicio del centre del cercle.
        /// </summary>
        /// 
        public EdaPoint Position {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// El radi del cercle.
        /// </summary>
        /// 
        public int Radius {
            get => _radius;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Radius));

                _radius = value;
            }
        }

        /// <summary>
        /// Diametre del cercle.
        /// </summary>
        /// 
        public int Diameter {
            get => _radius * 2;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Diameter));

                _radius = value / 2;
            }
        }

        /// <summary>
        /// L'amplada de linia.
        /// </summary>
        /// 
        public int Thickness {
            get => _thickness;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Thickness));

                _thickness = value;
            }
        }

        /// <summary>
        /// Indicador de cercle ple. 
        /// </summary>
        /// 
        public bool Filled {
            get => _thickness == 0;
            set {
                if (value)
                    _thickness = 0;
            }
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Circle;
    }
}
