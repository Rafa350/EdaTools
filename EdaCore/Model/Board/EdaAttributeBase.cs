using System;
using MikroPic.EdaTools.v1.Core.Model.Common;

namespace MikroPic.EdaTools.v1.Core.Model.Board {
    
    public abstract class EdaAttributeBase: IEdaVisitable<IEdaBoardVisitor> {

        private string _name;
        private string _value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <param name="value">El valor.</param>
        /// 
        public EdaAttributeBase(string name, string value = null) {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
            _value = value;
        }

        /// <inheritdoc/>
        /// 
        public abstract void AcceptVisitor(IEdaBoardVisitor visitor);

        /// <summary>
        /// El nom del atribut
        /// </summary>
        /// 
        public string Name {
            get => _name;
            set {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(Name));
                _name = value;
            }
        }

        /// <summary>
        /// Obte o asigna el valor del atribut
        /// </summary>
        /// 
        public string Value {
            get => _value;
            set => _value = value;
        }
    }
}
