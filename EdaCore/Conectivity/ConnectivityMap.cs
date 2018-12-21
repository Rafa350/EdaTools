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

        public void Add(Element element) {

            if (element == null)
                throw new ArgumentNullException("element");

            if (element is ViaElement via) 
                AddElement(via.Position, via);

            else if (element is PadElement pad)
                AddElement(pad.Position, pad);

            else if (element is LineElement line) {
                if (line.LayerSet.Contains(Layer.TopId) ||
                    line.LayerSet.Contains(Layer.BottomId)) {
                    AddElement(line.StartPosition, line);
                    AddElement(line.EndPosition, line);
                }
            }
        }

        public void Remove(Element element) {

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
