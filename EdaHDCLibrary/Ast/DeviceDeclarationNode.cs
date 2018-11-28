namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System.Collections.Generic;

    public sealed class DeviceDeclarationNode: DeclarationNode {

        private readonly IEnumerable<PinDefinitionNode> pins;
        private readonly IEnumerable<AttributeDefinitionNode> attributes;

        public DeviceDeclarationNode(string name, IEnumerable<PinDefinitionNode> pins, IEnumerable<AttributeDefinitionNode> attributes):
            base(name) {

            this.pins = pins;
            this.attributes = attributes;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public IEnumerable<PinDefinitionNode> Pins {
            get {
                return pins;
            }
        }

        public IEnumerable<AttributeDefinitionNode> Attributes {
            get {
                return attributes;
            }
        }
    }
}
