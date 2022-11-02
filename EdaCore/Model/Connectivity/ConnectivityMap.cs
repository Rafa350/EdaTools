namespace MikroPic.EdaTools.v1.Core.Model.Connectivity {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

    public sealed class ConnectivityMap {

        private sealed class AddElementVisitor: EdaElementVisitor {

            private readonly ConnectivityMap map;

            public AddElementVisitor(ConnectivityMap map) {

                this.map = map;
            }

            public override void Visit(EdaLineElement line) {

                ConnectivityItem item = new ConnectivityItem(line);

                ConnectivityAnchor anchorA = map.GetAnchor(line.StartPosition);
                anchorA.AddItem(item);

                ConnectivityAnchor anchorB = map.GetAnchor(line.StartPosition);
                anchorB.AddItem(item);

                map.DefineEdge(anchorA, anchorB);
            }

            public override void Visit(EdaViaElement via) {

                ConnectivityItem item = new ConnectivityItem(via);

                ConnectivityAnchor anchor = map.GetAnchor(via.Position);
                anchor.AddItem(item);
            }

        }

        private readonly Dictionary<EdaPoint, ConnectivityAnchor> anchors = new Dictionary<EdaPoint, ConnectivityAnchor>();
        private readonly Dictionary<Tuple<ConnectivityAnchor, ConnectivityAnchor>, ConnectivityEdge> edges = new Dictionary<Tuple<ConnectivityAnchor, ConnectivityAnchor>, ConnectivityEdge>();

        /// <summary>
        /// Afegeix una plada al mapa de conectivitat.
        /// </summary>
        /// <param name="board">La placa a afeigir.</param>
        /// 
        public void Add(EdaBoard board) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            var visitor = new AddElementVisitor(this);
            board.AcceptVisitor(visitor);
        }

        /// <summary>
        /// Afegeix un element.
        /// </summary>
        /// <param name="element">L'element a afeigir.</param>
        /// 
        public void Add(EdaElement element) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (element is EdaViaElement via)
                AddViaElement(via);

            else if (element is EdaSmtPadElement smd)
                AddSmdPadElement(smd);

            else if (element is EdaThtPadElement th)
                AddThPadElement(th);

            else if (element is EdaLineElement line)
                AddLineElement(line);

            else if (element is EdaArcElement arc)
                AddLineElement(arc);
        }

        /// <summary>
        /// Elimina un element.
        /// </summary>
        /// <param name="element">L'element a eliminar.</param>
        /// 
        public void Remove(EdaElement element) {

        }

        /// <summary>
        /// Obte la pista de la posicio especificada.
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// <returns>La pista com a llista d'elements.</returns>
        /// 
        public IEnumerable<EdaElement> GetChainedElements(EdaPoint position) {

            HashSet<EdaElement> chain = new HashSet<EdaElement>();
            //            GetChainedElements(position, chain);
            return chain;
        }

        /*        private void GetChainedElements(Point position, ICollection<Element> chain) {

                    if (map.TryGetValue(position, out List<Element> elements)) {
                        foreach (var element in elements) {

                            if (!chain.Contains(element)) {

                                chain.Add(element);

                                // Procesa els elements encadenats (Linies i arcs)
                                //
                                if (element is LineElement line) {
                                    if (line.StartPosition == position)
                                        GetChainedElements(line.EndPosition, chain);
                                    else
                                        GetChainedElements(line.StartPosition, chain);
                                }
                                else if (element is ArcElement arc) {
                                    if (arc.StartPosition == position)
                                        GetChainedElements(arc.EndPosition, chain);
                                    else
                                        GetChainedElements(arc.StartPosition, chain);
                                }
                            }
                        }
                    }
                }*/

        /// <summary>
        /// Obte un anclatge per la posicio especificada.
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// <returns>L'anclatge d'aquesta posicio.</returns>
        /// 
        private ConnectivityAnchor GetAnchor(EdaPoint position) {

            if (!anchors.TryGetValue(position, out ConnectivityAnchor anchor)) {
                anchor = new ConnectivityAnchor(position);
                anchors.Add(position, anchor);
            }

            return anchor;
        }

        /// <summary>
        /// Defineix un item en un anclatge.
        /// </summary>
        /// <param name="anchor">L'anclatge.</param>
        /// <param name="element">El element.</param>
        /// 
        private void DefineItem(ConnectivityAnchor anchor, EdaElement element) {

            ConnectivityItem item = new ConnectivityItem(element);
            anchor.AddItem(item);
        }

        /// <summary>
        /// Defineix una unio entre dos anclatges.
        /// </summary>
        /// <param name="anchorA">Anclatge A.</param>
        /// <param name="anchorB">Anclatge B.</param>
        /// 
        private void DefineEdge(ConnectivityAnchor anchorA, ConnectivityAnchor anchorB) {

            Tuple<ConnectivityAnchor, ConnectivityAnchor> key = new Tuple<ConnectivityAnchor, ConnectivityAnchor>(anchorA, anchorB);

            if (edges.ContainsKey(key))
                throw new InvalidOperationException("Ya existe el anclaje");

            ConnectivityEdge edge = new ConnectivityEdge(anchorA, anchorB);
            edges.Add(key, edge);
        }

        /// <summary>
        /// Afegeix un element 'Via'
        /// </summary>
        /// <param name="via">L'element a afeigir.</param>
        /// 
        private void AddViaElement(EdaViaElement via) {

            ConnectivityItem item = new ConnectivityItem(via);

            ConnectivityAnchor anchor = GetAnchor(via.Position);
            anchor.AddItem(item);
        }

        /// <summary>
        /// Afegeix un element 'ThPad'
        /// </summary>
        /// <param name="pad">L'element a afeigir.</param>
        /// 
        private void AddThPadElement(EdaThtPadElement pad) {

            ConnectivityItem item = new ConnectivityItem(pad);

            ConnectivityAnchor anchor = GetAnchor(pad.Position);
            anchor.AddItem(item);
        }

        /// <summary>
        /// Afegeix un element 'SmdPad'
        /// </summary>
        /// <param name="pad">L'element a afeigir.</param>
        /// 
        private void AddSmdPadElement(EdaSmtPadElement pad) {

            ConnectivityItem item = new ConnectivityItem(pad);

            ConnectivityAnchor anchor = GetAnchor(pad.Position);
            anchor.AddItem(item);
        }

        /// <summary>
        /// Afegeix un element
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// <param name="element">El element.</param>
        /// 
        private void AddLineElement(EdaLineBaseElement line) {

            ConnectivityItem item = new ConnectivityItem(line);

            ConnectivityAnchor anchorA = GetAnchor(line.StartPosition);
            anchorA.AddItem(item);

            ConnectivityAnchor anchorB = GetAnchor(line.StartPosition);
            anchorB.AddItem(item);

            DefineEdge(anchorA, anchorB);
        }

        /// <summary>
        /// Elimina un element.
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// <param name="element">El element.</param>
        /// 
        private void RemoveElement(EdaPoint position, EdaElement element) {
        }

        public void Clear() {

            anchors.Clear();
            edges.Clear();
        }
    }
}
