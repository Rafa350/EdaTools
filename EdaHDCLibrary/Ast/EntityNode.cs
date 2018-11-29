namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;
    using System.Collections.Generic;

    public sealed class EntityNode: Node {

        private readonly string prefix;
        private readonly string name;
        private readonly IEnumerable<MemberNode> members;

        public EntityNode(string prefix, string name, IEnumerable<MemberNode> members) {

            if (String.IsNullOrEmpty(prefix))
                throw new ArgumentNullException("prefix");

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.prefix = prefix;
            this.name = name;
            this.members = members;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public string Prefix {
            get {
                return prefix;
            }
        }

        public string Name {
            get {
                return name;
            }
        }

        public IEnumerable<MemberNode> Members {
            get {
                return members;
            }
        }
    }
}
