
using MikroPic.EdaTools.v1.Core.Model.Board.IO.Serializers;
using MikroPic.EdaTools.v1.Core.Model.Common;
using NetSerializer.Attributes;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    [NetSerializer(typeof(ComponentSerializer), AliasName = "Component")]
    public sealed partial class EdaComponent: IEdaVisitable<IEdaBoardVisitor>, IEdaName {

        private string _name;
        private string _description;

        /// <inheritdoc/>
        /// 
        public void AcceptVisitor(IEdaBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna el identificador del component.
        /// </summary>
        /// 
        public string Name {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Obte o asigna la descripcio.
        /// </summary>
        /// 
        public string Description {
            get => _description;
            set => _description = value;
        }
    }
}
