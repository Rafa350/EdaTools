namespace MikroPic.EdaTools.v1.Core.Import.KiCad.Infrastructure {

    /// <summary>
    /// Clase que representa una expressio S-Expression.
    /// </summary>
    /// 
    public sealed class STree {

        private readonly string _source;
        private readonly SNode _root;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="source">String de valors referenciats pels nodes terminal.</param>
        /// <param name="root">Node arrel.</param>
        /// 
        public STree(string source, SNode root) {

            _source = source;
            _root = root;
        }

        /// <summary>
        /// Obte el valor d'un node.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <returns>El seu valor.</returns>
        /// 
        public string GetNodeValue(SLeaf node) {

            return _source.Substring(node.Position, node.Length);
        }

        /// <summary>
        /// Obte el node arrel.
        /// </summary>
        /// 
        public SNode Root => _root;
    }

}
