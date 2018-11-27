namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;

    public sealed class DeviceAttributeDeclarationNode: Node {

        private readonly Type type;
        private readonly string name;

        public DeviceAttributeDeclarationNode(string name, Type type) { 

            this.name = name;
            this.type = type;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public Type Type {
            get {
                return type;
            }
        }

        public string Name {
            get {
                return name;
            }
        }
    }
}
