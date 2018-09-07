namespace MikroPic.EdaTools.v1.Pcb {

    using MikroPic.EdaTools.v1.Xml;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    using LayerId = System.String;

    /// <summary>
    /// Objecte que representa un conjunt de capes. Es un objecte invariant.
    /// </summary>
    public struct LayerSet: IEnumerable<LayerId> {

        private readonly LayerId[] data;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="id0">Identificador de la capa.</param>
        /// 
        public LayerSet(LayerId id0) {

            data = new LayerId[1];
            data[0] = id0;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="id0">Identificador de la primera capa.</param>
        /// <param name="id1">Identificador de la segona capa.</param>
        /// 
        public LayerSet(LayerId id0, LayerId id1) {

            data = new LayerId[2];
            data[0] = id0;
            data[1] = id1;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="id0">Identificador de la primera capa.</param>
        /// <param name="id1">Identificador de la segona capa.</param>
        /// <param name="id2">Identificador de la tercera capa.</param>
        /// 
        public LayerSet(LayerId id0, LayerId id1, LayerId id2) {

            data = new LayerId[3];
            data[0] = id0;
            data[1] = id1;
            data[2] = id2;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="ids">Llista d'identificadors.</param>
        /// 
        public LayerSet(params LayerId[] ids) {

            data = new LayerId[ids.Length];
            ids.CopyTo(data, 0);
        }

        /// <summary>
        /// Constructor de copia.
        /// </summary>
        /// <param name="other">El conjunt a copiar.</param>
        /// 
        public LayerSet(LayerSet other) {

            data = new LayerId[other.data.Length];
            other.data.CopyTo(data, 0);
        }

        /// <summary>
        /// Comprova si un identificador pertany al conjunt.
        /// </summary>
        /// <param name="id">El identificadord e la capa.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public bool Contains(LayerId id) {

            foreach (var i in data)
                if (id == i)
                    return true;

            return false;
        }

        /// <summary>
        /// Unio de dos conjunts.
        /// </summary>
        /// <param name="other">L'altre conjunt</param>
        /// <returns>El resultat de la unio.</returns>
        /// 
        public LayerSet Union(LayerSet other) {

            LayerId[] a = new LayerId[data.Length + other.data.Length];
            data.CopyTo(a, 0);
            other.data.CopyTo(a, data.Length);
            return new LayerSet(a);
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public override string ToString() {

            if ((data != null) && (data.Length > 0)) {

                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (var id in data) {
                    if (first)
                        first = false;
                    else
                        sb.Append(", ");
                    sb.Append(id.ToString());
                }
                return sb.ToString();

            }
            else
                return null;
        }

        /// <summary>
        /// Converteix una cadena en un objecte 'LayerSet'
        /// </summary>
        /// <param name="s">La cadena a procesar.</param>
        /// <returns>L'objecte obtingut.</returns>
        /// 
        public static LayerSet Parse(string s) {

            string[] ss = s.Split(',');
            return new LayerSet(ss);
        }

        public IEnumerator<string> GetEnumerator() {

            return ((IEnumerable<string>)data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<string>)data).GetEnumerator();
        }
    }

    /// <summary>
    /// Clase que implememta els metodes d'extensio.
    /// </summary>
    public static class LayerSetHelper {

        /// <summary>
        /// Escriu un atribut de tipus 'LayerSet'
        /// </summary>
        /// <param name="wr">L'objecte 'XmlWriterAdapter'</param>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="layerSet">El valor a escriure.</param>
        /// 
        public static void WriteAttribute(this XmlWriterAdapter wr, string name, LayerSet layerSet) {

            wr.WriteAttribute(name, layerSet.ToString());
        }

        /// <summary>
        /// Llegeix un atribut de tipus 'LayerSet'
        /// </summary>
        /// <param name="rd">L'objecte 'XmlReaderAdapter'</param>
        /// <param name="name">El nom de l'atribut.</param>
        /// <returns>El valor obtingut.</returns>
        /// 
        public static LayerSet AttributeAsLayerSet(this XmlReaderAdapter rd, string name) {

            string[] s = rd.AttributeAsStrings("layers");
            return new LayerSet(s);
        }
    }
}
