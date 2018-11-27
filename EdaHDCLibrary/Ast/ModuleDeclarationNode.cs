namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;

    public sealed class ModuleDeclarationNode: Node {

        private readonly string name;

        public ModuleDeclarationNode(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
        }

        public override void AcceptVisitor(IVisitor visitor) {
        }
    }
}
