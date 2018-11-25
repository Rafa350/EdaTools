namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;

    public sealed class TypeNode: Node {

        private readonly string name;

        public TypeNode(string name) {

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
