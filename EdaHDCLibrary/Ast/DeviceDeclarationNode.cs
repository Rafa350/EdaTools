namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System.Collections.Generic;

    public sealed class DeviceDeclarationNode: Node {

        private readonly string name;
        private readonly IEnumerable<DevicePinDeclarationNode> pins;
        private readonly IEnumerable<DeviceAttributeDeclarationNode> attributes;

        public DeviceDeclarationNode(string name, IEnumerable<DevicePinDeclarationNode> pins, IEnumerable<DeviceAttributeDeclarationNode> attributes) {

            this.name = name;
            this.pins = pins;
            this.attributes = attributes;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public string Name {
            get {
                return name;
            }
        }

        public IEnumerable<DevicePinDeclarationNode> Pins {
            get {
                return pins;
            }
        }

        public IEnumerable<DeviceAttributeDeclarationNode> Attributes {
            get {
                return attributes;
            }
        }
    }
}
