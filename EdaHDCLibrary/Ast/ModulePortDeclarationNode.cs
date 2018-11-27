namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;

    public sealed class ModulePortDeclarationNode: Node {

        private readonly string name;

        public ModulePortDeclarationNode(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
        }

        public override void AcceptVisitor(IVisitor visitor) {
        }

        public string Name {
            get {
                return name;
            }
        }
    }
}
