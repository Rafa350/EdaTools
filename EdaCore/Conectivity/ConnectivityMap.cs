namespace MikroPic.EdaTools.v1.Core.Conectivity {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System;
    using System.Collections.Generic;

    public sealed class ConnectivityMap {

        private readonly Dictionary<Point, List<Element>> map = new Dictionary<Point, List<Element>>();

        public void Add(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            foreach (var element in board.Elements) {
                Add(element);
            }
        }

        /// <summary>
        /// Afegeix un element.
        /// </summary>
        /// <param name="element">L'element a afeigir.</param>
        /// 
        public void Add(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (element is ViaElement via) 
                AddElement(via.Position, via);

            else if (element is SmdPadElement smd)
                AddElement(smd.Position, smd);

            else if (element is ThPadElement th)
                AddElement(th.Position, th);

            else if (element is LineElement line) {
                if (line.LayerSet.Contains(Layer.TopId) ||
                    line.LayerSet.Contains(Layer.BottomId)) {
                    AddElement(line.StartPosition, line);
                    AddElement(line.EndPosition, line);
                }
            }

            else if (element is ArcElement arc) {
                if (arc.LayerSet.Contains(Layer.TopId) ||
                    arc.LayerSet.Contains(Layer.BottomId)) {
                    AddElement(arc.StartPosition, arc);
                    AddElement(arc.EndPosition, arc);
                }
            }
        }

        /// <summary>
        /// Elimina un element.
        /// </summary>
        /// <param name="element">L'element a eliminar.</param>
        /// 
        public void Remove(Element element) {

        }

        /// <summary>
        /// Obte la pista de la posicio especificada.
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// <returns>La pista com a llista d'elements.</returns>
        /// 
        public IEnumerable<Element> GetChainedElements(Point position) {

            HashSet<Element> chain = new HashSet<Element>();
            GetChainedElements(position, chain);
            return chain;
        }

        private void GetChainedElements(Point position, ICollection<Element> chain) {

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
        }

        /// <summary>
        /// Afegeix un element
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// <param name="element">El element.</param>
        /// 
        private void AddElement(Point position, Element element) {

            List<Element> elements;
            if (!map.TryGetValue(position, out elements)) {
                elements = new List<Element>();
                map.Add(position, elements);
            }
            elements.Add(element);
        }

        /// <summary>
        /// Elimina un element.
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// <param name="element">El element.</param>
        /// 
        private void RemoveElement(Point position, Element element) {

            List<Element> elements = map[position];
            elements.Remove(element);
            if (elements.Count == 0)
                map.Remove(position);
        }

        public void Clear() {

            map.Clear();
        }
    }
}
