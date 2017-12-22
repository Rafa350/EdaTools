namespace MikroPic.EdaTools.v1.Pcb.Model.Collections {

    using System;
    using System.Collections.Generic;

    public class BidirectionalMap<First, Second> where First : class where Second : class {

        private readonly Dictionary<First, Second> toSecondDict = new Dictionary<First, Second>();
        private readonly Dictionary<Second, First> toFirstDict = new Dictionary<Second, First>();

        public void Add(First first, Second second) {

            if (first == null)
                throw new ArgumentNullException("first");

            if (second == null)
                throw new ArgumentNullException("second");

            if (toSecondDict.ContainsKey(first) || toFirstDict.ContainsKey(second))
                throw new InvalidOperationException("Ya existe la asociacion.");

            toSecondDict.Add(first, second);
            toFirstDict.Add(second, first);
        }

        public void Remove(First first, Second second) {

            if (first == null)
                throw new ArgumentNullException("first");

            if (second == null)
                throw new ArgumentNullException("second");

            if (!toSecondDict.ContainsKey(first) || !toFirstDict.ContainsKey(second))
                throw new InvalidOperationException("No existe la asociacion.");

            toSecondDict.Remove(first);
            toFirstDict.Remove(second);
        }

        public First GetFirst(Second second) {

            if (second == null)
                throw new ArgumentNullException("child");

            First first;
            if (toFirstDict.TryGetValue(second, out first))
                return first;
            else
                return default(First);
        }

        public Second GetSecond(First first) {

            if (first == null)
                throw new ArgumentNullException("first");

            Second second;
            if (toSecondDict.TryGetValue(first, out second))
                return second;
            else
                return default(Second);
        }
    }
}
