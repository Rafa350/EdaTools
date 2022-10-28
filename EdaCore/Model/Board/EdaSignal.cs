﻿using System;
using MikroPic.EdaTools.v1.Core.Model.Common;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa una senyal.
    /// </summary>
    /// 
    public sealed class EdaSignal: IEdaVisitable<IEdaBoardVisitor> {

        private string _name;
        private int _clearance = 150000;

        /// <inheritdoc/>
        /// 
        public void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        /// 
        public override string ToString() =>
            String.Format("Signal: {0}", _name);

        /// <summary>
        /// Nom de la senyal.
        /// </summary>
        /// 
        public string Name {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Distancia d'aillament minima de la senyal.
        /// </summary>
        /// 
        public int Clearance {
            get => _clearance;
            set {
                if (_clearance < 0)
                    throw new ArgumentOutOfRangeException("Clearance");
                _clearance = value;
            }
        }
    }
}
