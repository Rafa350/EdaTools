namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;

    public abstract class DeclarationNode: Node {

        private readonly string name;

        public DeclarationNode(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
        }

        public string Name {
            get {
                return name;
            }
        }
    }
}
