namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Xml;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Objecte que representa un conjunt de capes. Es un objecte invariant.
    /// </summary>
    public struct LayerSet: IEnumerable<LayerId> {

        private readonly LayerId[] data;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Identificador de la capa.</param>
        /// 
        public LayerSet(LayerId id) {

            data = new LayerId[1] { id };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="id1">Identificador de la primera capa.</param>
        /// <param name="id2">Identificador de la segona capa.</param>
        /// 
        public LayerSet(LayerId id1, LayerId id2) {

            data = new LayerId[2] { id1, id2 };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="id1">Identificador de la primera capa.</param>
        /// <param name="id2">Identificador de la segona capa.</param>
        /// <param name="id3">Identificador de la tercera capa.</param>
        /// 
        public LayerSet(LayerId id1, LayerId id2, LayerId id3) {

            data = new LayerId[3] { id1, id2, id3 };
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
        /// Operador +
        /// </summary>
        /// <param name="a">Primer operand.</param>
        /// <param name="b">Segon operand.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static LayerSet operator +(LayerSet a, LayerSet b) {

            LayerId[] s = new LayerId[a.data.Length + b.data.Length];
            a.data.CopyTo(s, 0);
            b.data.CopyTo(s, a.data.Length);
            return new LayerSet(s);
        }

        /// <summary>
        /// Operador +
        /// </summary>
        /// <param name="a">Primer operand.</param>
        /// <param name="b">Segon operand.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static LayerSet operator +(LayerSet a, LayerId b) {

            LayerId[] s = new LayerId[a.data.Length + 1];
            a.data.CopyTo(s, 0);
            s[a.data.Length] = b;
            return new LayerSet(s);
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public override string ToString() {

            return ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
        /// <param name="provider">Objecte proveidor de format.</param>
        /// <returns>El resultat de la conversio.</returns>
        /// 
        public string ToString(IFormatProvider provider) {

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

            try {
                string[] ss = s.Split(',');
                LayerId[] ids = new LayerId[ss.Length];
                for (int i = 0; i < ss.Length; i++)
                    ids[i] = LayerId.Parse(ss[i]);
                return new LayerSet(ids);
            }
            catch (Exception ex) {
                throw new InvalidOperationException(
                    String.Format("No se pudo convertir el texto '{0}' a 'LayerSet'", s),
                    ex);
            }
        }

        public IEnumerator<LayerId> GetEnumerator() {

            return ((IEnumerable<LayerId>)data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {

            return ((IEnumerable<LayerId>)data).GetEnumerator();
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

            string s = rd.AttributeAsString("layers");
            return LayerSet.Parse(s);
        }
    }
}
