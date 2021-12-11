namespace MikroPic.EdaTools.v1.Core.Import.KiCad.Infrastructure {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class SBranch : SNode, IEnumerable<SNode> {

        private readonly List<SNode> _nodes;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="nodes">El nodes a afeigir.</param>
        /// 
        public SBranch(List<SNode> nodes) {

            _nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
        }

        /// <summary>
        /// Obte un enumerador.
        /// </summary>
        /// <returns>El enumerador.</returns>
        /// 
        public IEnumerator<SNode> GetEnumerator() {

            return _nodes.GetEnumerator();
        }

        /// <summary>
        /// Obte un enumerador.
        /// </summary>
        /// <returns>El enumerador.</returns>
        /// 
        IEnumerator IEnumerable.GetEnumerator() {

            return _nodes.GetEnumerator();
        }

        /// <summary>
        /// Obte els nodes.
        /// </summary>
        /// 
        public IEnumerable<SNode> Nodes =>
            _nodes;

        /// <summary>
        /// Indica si la branca es buida.
        /// </summary>
        /// 
        public bool IsEmpty =>
            _nodes.Count == 0;

        /// <summary>
        /// Obte el numero de nodes.
        /// </summary>
        /// 
        public int Count =>
            _nodes.Count;

        /// <summary>
        /// Obte un node.
        /// </summary>
        /// <param name="index">Index del node.</param>
        /// <returns>El node.</returns>
        /// 
        public SNode this[int index] =>
            _nodes[index];

        /// <summary>
        /// Obte el primer node.
        /// </summary>
        /// 
        public SNode First =>
            _nodes[0];
    }


}
