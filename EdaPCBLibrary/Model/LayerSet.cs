namespace MikroPic.EdaTools.v1.Pcb {

    using MikroPic.EdaTools.v1.Xml;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Objecte que representa un conjunt de capes. Es un objecte invariant.
    /// </summary>
    public struct LayerSet: IEnumerable<string> {

        private readonly string[] data;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Identificador de la capa.</param>
        /// 
        public LayerSet(string name) {

            data = new string[1] { name };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name1">Identificador de la primera capa.</param>
        /// <param name="name2">Identificador de la segona capa.</param>
        /// 
        public LayerSet(string name1, string name2) {

            data = new string[2] { name1, name2 };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name1">Identificador de la primera capa.</param>
        /// <param name="name2">Identificador de la segona capa.</param>
        /// <param name="name3">Identificador de la tercera capa.</param>
        /// 
        public LayerSet(string name1, string name2, string name3) {

            data = new string[3] { name1, name2, name3 };
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="names">Llista d'identificadors.</param>
        /// 
        public LayerSet(params string[] names) {

            data = new string[names.Length];
            names.CopyTo(data, 0);
        }

        /// <summary>
        /// Constructor de copia.
        /// </summary>
        /// <param name="other">El conjunt a copiar.</param>
        /// 
        public LayerSet(LayerSet other) {

            data = new string[other.data.Length];
            other.data.CopyTo(data, 0);
        }

        /// <summary>
        /// Comprova si un identificador pertany al conjunt.
        /// </summary>
        /// <param name="name">El identificadord e la capa.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public bool Contains(string name) {

            foreach (var i in data)
                if (name == i)
                    return true;

            return false;
        }

        /// <summary>
        /// Operador suma
        /// </summary>
        /// <param name="a">Primer operand.</param>
        /// <param name="b">Segon operand.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static LayerSet operator +(LayerSet a, LayerSet b) {

            string[] s = new string[a.data.Length + b.data.Length];
            a.data.CopyTo(s, 0);
            b.data.CopyTo(s, a.data.Length);
            return new LayerSet(s);
        }

        /// <summary>
        /// Operador suma
        /// </summary>
        /// <param name="a">Primer operand.</param>
        /// <param name="b">Segon operand.</param>
        /// <returns>El resultat de l'operacio.</returns>
        /// 
        public static LayerSet operator +(LayerSet a, string b) {

            string[] s = new string[a.data.Length + 1];
            a.data.CopyTo(s, 0);
            s[a.data.Length] = b;
            return new LayerSet(s);
        }

        public override string ToString() {

            return ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converteix a string
        /// </summary>
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

            string s = rd.AttributeAsString("layers");
            return LayerSet.Parse(s);
        }
    }
}
