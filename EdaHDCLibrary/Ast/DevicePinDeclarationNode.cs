namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;

    public sealed class DevicePinDeclarationNode: Node {

        private readonly string name;
        private readonly string identifier;

        public DevicePinDeclarationNode(string name, string identifier) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

//            if (String.IsNullOrEmpty(identifier))
                //throw new ArgumentNullException("identifier");

            this.name = name;
            this.identifier = identifier;
        }

        public override void AcceptVisitor(IVisitor visitor) {
        }

        public string Name {
            get {
                return name;
            }
        }

        public string Identifier {
            get {
                return identifier;
            }
        }
    }
}
