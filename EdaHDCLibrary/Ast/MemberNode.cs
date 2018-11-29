namespace MikroPic.EdaTools.v1.Hdc.Ast {

    using System;
    using System.Collections.Generic;

    public sealed class MemberNode: Node {

        private readonly string prefix;
        private readonly string name;
        private readonly IEnumerable<OptionNode> options;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="prefix">Prefix que indica el tipus de membre.</param>
        /// <param name="name">Nom unic del membre de de l'entitat a la que pertany.</param>
        /// <param name="options">Llista d'opcions.</param>
        /// 
        public MemberNode(string prefix, string name, IEnumerable<OptionNode> options) {

            if (String.IsNullOrEmpty(prefix))
                throw new ArgumentNullException("prefix");

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.prefix = prefix;
            this.name = name;
            this.options = options;
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el valor del prefix.
        /// </summary>
        /// 
        public string Prefix {
            get {
                return prefix;
            }
        }

        /// <summary>
        /// Obte el valor del nom.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Obte la llista d'opcions.
        /// </summary>
        /// 
        public IEnumerable<OptionNode> Options {
            get {
                return options;
            }
        }
    }
}
