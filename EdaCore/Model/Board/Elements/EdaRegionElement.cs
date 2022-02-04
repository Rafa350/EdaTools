﻿using System;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Elements {

    /// <summary>
    /// Clase que representa una regio poligonal conductora.
    /// </summary>
    /// 
    public sealed class EdaRegionElement: EdaPolygonElement, IEdaConectable {

        private int _priority;
        private int _clearance;

        /// <inheritdoc/>
        /// 
        public override void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// L'amplada del aillament
        /// </summary>
        /// 
        public int Clearance {
            get => _clearance;
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Clearance");

                _clearance = value;
            }
        }

        /// <summary>
        /// La prioritat
        /// </summary>
        /// 
        public int Priority {
            get => _priority;   
            set => _priority = value;
        }

        /// <inheritdoc/>
        /// 
        public override ElementType ElementType =>
            ElementType.Region;
    }
}
