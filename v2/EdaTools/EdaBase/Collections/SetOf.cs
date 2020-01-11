namespace MikroPic.EdaTools.v1.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Objecte que representa un conjunt de capes. Es un objecte invariant.
    /// </summary>
    public struct SetOf<T> : IEnumerable<T> {

        private readonly T[] storage;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">El element a afeigir.</param>
        /// 
        public SetOf(T element) {

            storage = new T[1] { element };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="element1">Primer element.</param>
        /// <param name="element2">Segon element.</param>
        /// 
        public SetOf(T element1, T element2) {

            storage = new T[2] { element1, element2 };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="element1">Primer element.</param>
        /// <param name="element2">Segon element.</param>
        /// <param name="element3">Tercer element.</param>
        /// 
        public SetOf(T element1, T element2, T element3) {

            storage = new T[3] { element1, element2, element3 };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="elements">Elements.</param>
        /// 
        public SetOf(params T[] elements) {

            storage = new T[elements.Length];
            elements.CopyTo(storage, 0);
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="elements">Elements.</param>
        /// 
        public SetOf(IEnumerable<T> elements) {

            ICollection<T> collection = elements as ICollection<T>;
            if (collection == null) {
                List<T> e = new List<T>(elements);
                storage = e.ToArray();
            }
            else {
                storage = new T[collection.Count];
                collection.CopyTo(storage, 0);
            }
        }

        /// <summary>
        /// Constructor de copia.
        /// </summary>
        /// <param name="other">El conjunt a copiar.</param>
        /// 
        public SetOf(SetOf<T> other) {

            storage = new T[other.storage.Length];
            other.storage.CopyTo(storage, 0);
        }

        /// <summary>
        /// Comprova si un element pertany al conjunt.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public bool Contains(T element) {

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
        public static SetOf<T> operator +(SetOf<T> a, SetOf<T> b) {

            T[] s = new T[a.storage.Length + b.storage.Length];
            a.storage.CopyTo(s, 0);
            b.storage.CopyTo(s, a.storage.Length);
            return new SetOf<T>(s);
        }

        /// <summary>
        /// Operador +
        /// </summary>
        /// <param name="a">Conjunt.</param>
        /// <param name="b">Element.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static SetOf<T> operator +(SetOf<T> a, T b) {

            T[] s = new T[a.storage.Length + 1];
            a.storage.CopyTo(s, 0);
            s[a.storage.Length] = b;
            return new SetOf<T>(s);
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public override string ToString() {

            return String.Format("SetOf<{0}>: {1}", typeof(T), storage.Length);
        }

        public IEnumerator<T> GetEnumerator() {

            return ((IEnumerable<T>)storage).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {

            return ((IEnumerable<T>)storage).GetEnumerator();
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
