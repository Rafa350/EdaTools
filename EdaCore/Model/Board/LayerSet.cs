namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public struct LayerSet : IEnumerable<string> {

        private readonly string[] storage;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">El element a afeigir.</param>
        /// 
        public LayerSet(string element) {

            storage = new string[1] { element };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="element1">Primer element.</param>
        /// <param name="element2">Segon element.</param>
        /// 
        public LayerSet(string element1, string element2) {

            storage = new string[2] { element1, element2 };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="element1">Primer element.</param>
        /// <param name="element2">Segon element.</param>
        /// <param name="element3">Tercer element.</param>
        /// 
        public LayerSet(string element1, string element2, string element3) {

            storage = new string[3] { element1, element2, element3 };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="elements">Elements.</param>
        /// 
        public LayerSet(params string[] elements) {

            if (elements == null)
                throw new ArgumentNullException("elements");

            storage = new string[elements.Length];
            elements.CopyTo(storage, 0);
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="elements">Elements.</param>
        /// 
        public LayerSet(IEnumerable<string> elements) {

            if (elements == null)
                throw new ArgumentNullException("elements");

            if (!(elements is ICollection<string> collection)) {
                List<string> e = new List<string>(elements);
                storage = e.ToArray();
            }
            else {
                storage = new string[collection.Count];
                collection.CopyTo(storage, 0);
            }
        }

        /// <summary>
        /// Constructor de copia.
        /// </summary>
        /// <param name="other">El conjunt a copiar.</param>
        /// 
        public LayerSet(LayerSet other) {

            storage = new string[other.storage.Length];
            other.storage.CopyTo(storage, 0);
        }

        /// <summary>
        /// Comprova si un element pertany al conjunt.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public bool Contains(string element) {

            for (int i = 0; i < storage.Length; i++)
                if (element.Equals(storage[i]))
                    return true;

            return false;
        }

        /// <summary>
        /// Operador +
        /// </summary>
        /// <param name="a">Primer conjunt.</param>
        /// <param name="b">Segon conjunt.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static LayerSet operator +(LayerSet a, LayerSet b) {

            string[] s = new string[a.storage.Length + b.storage.Length];
            a.storage.CopyTo(s, 0);
            b.storage.CopyTo(s, a.storage.Length);
            return new LayerSet(s);
        }

        /// <summary>
        /// Operador +
        /// </summary>
        /// <param name="a">Conjunt.</param>
        /// <param name="b">Element.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static LayerSet operator +(LayerSet a, string b) {

            if (a.storage == null)
                return new LayerSet(b);
            else {
                string[] s = new string[a.storage.Length + 1];
                a.storage.CopyTo(s, 0);
                s[a.storage.Length] = b;
                return new LayerSet(s);
            }
        }

        /// <summary>
        /// Operador -
        /// </summary>
        /// <param name="a">Conjunt.</param>
        /// <param name="b">Element.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static LayerSet operator -(LayerSet a, string b) {

            if (a.storage == null)
                return new LayerSet(b);
            else {
                string[] s = new string[a.storage.Length - 1];
                int i = 0;
                foreach (var aa in a)
                    if (aa != b)
                        s[i++] = aa;
                return new LayerSet(s);
            }
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public override string ToString() {

            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var element in storage) {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(element.ToString());
            }
            return sb.ToString();
        }


        public static LayerSet Parse(string s) {

            if (String.IsNullOrEmpty(s))
                throw new ArgumentNullException("s");

            string[] ss = s.Split(',');
            string[] elements = new string[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                elements[i] = ss[i].Trim();
            return new LayerSet(elements);
        }

        public IEnumerator<string> GetEnumerator() {

            return ((IEnumerable<string>)storage).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {

            return ((IEnumerable<string>)storage).GetEnumerator();
        }

        /// <summary>
        /// Obte l'indicador de conjunt buit.
        /// </summary>
        /// 
        public bool IsEmpty {
            get {
                return (storage == null) || (storage.Length == 0);
            }
        }
    }
}