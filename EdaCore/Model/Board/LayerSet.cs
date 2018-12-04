namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public struct LayerSet : IEnumerable<LayerId> {

        private readonly LayerId[] storage;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">El element a afeigir.</param>
        /// 
        public LayerSet(LayerId element) {

            storage = new LayerId[1] { element };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="element1">Primer element.</param>
        /// <param name="element2">Segon element.</param>
        /// 
        public LayerSet(LayerId element1, LayerId element2) {

            storage = new LayerId[2] { element1, element2 };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="element1">Primer element.</param>
        /// <param name="element2">Segon element.</param>
        /// <param name="element3">Tercer element.</param>
        /// 
        public LayerSet(LayerId element1, LayerId element2, LayerId element3) {

            storage = new LayerId[3] { element1, element2, element3 };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="elements">Elements.</param>
        /// 
        public LayerSet(params LayerId[] elements) {

            storage = new LayerId[elements.Length];
            elements.CopyTo(storage, 0);
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="elements">Elements.</param>
        /// 
        public LayerSet(IEnumerable<LayerId> elements) {

            ICollection<LayerId> collection = elements as ICollection<LayerId>;
            if (collection == null) {
                List<LayerId> e = new List<LayerId>(elements);
                storage = e.ToArray();
            }
            else {
                storage = new LayerId[collection.Count];
                collection.CopyTo(storage, 0);
            }
        }

        /// <summary>
        /// Constructor de copia.
        /// </summary>
        /// <param name="other">El conjunt a copiar.</param>
        /// 
        public LayerSet(LayerSet other) {

            storage = new LayerId[other.storage.Length];
            other.storage.CopyTo(storage, 0);
        }

        /// <summary>
        /// Comprova si un element pertany al conjunt.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public bool Contains(LayerId element) {

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

            LayerId[] s = new LayerId[a.storage.Length + b.storage.Length];
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
        public static LayerSet operator +(LayerSet a, LayerId b) {

            LayerId[] s = new LayerId[a.storage.Length + 1];
            a.storage.CopyTo(s, 0);
            s[a.storage.Length] = b;
            return new LayerSet(s);
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

            string[] ss = s.Split(',');
            LayerId[] elements = new LayerId[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                elements[i] = LayerId.Parse(ss[i]);
            return new LayerSet(elements);
        }

        public IEnumerator<LayerId> GetEnumerator() {

            return ((IEnumerable<LayerId>)storage).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {

            return ((IEnumerable<LayerId>)storage).GetEnumerator();
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