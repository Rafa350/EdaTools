namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Xml;
    using System;
    using System.Text;

    /// <summary>
    /// Objecte que representa el identificador unic d'una capa.
    /// </summary>
    /// 
    public struct LayerId {

        private readonly string name;
        private readonly BoardSide side;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom de la capa.</param>
        /// 
        public LayerId(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.side = BoardSide.None;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">No de la capa.</param>
        /// <param name="side">Cara coreresponent de la placa.</param>
        /// 
        public LayerId(string name, BoardSide side) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.side = side;
        }

        /// <summary>
        /// Comprova si dos objectes son iguals.
        /// </summary>
        /// <param name="obj">L'altre objecte.</param>
        /// <returns>True si son iguals, false en cas conmtraria.</returns>
        /// 
        public override bool Equals(object obj) {

            if (obj is LayerId)
                return (name == ((LayerId)obj).name) && (side == ((LayerId)obj).side);
            else
                return false;
        }

        /// <summary>
        /// Obte el hash de l'objecte.
        /// </summary>
        /// <returns>El hash</returns>
        /// 
        public override int GetHashCode() {

            return name.GetHashCode() ^ (side.GetHashCode() * 7);
        }

        /// <summary>
        /// Converteix a text.
        /// </summary>
        /// <returns>El text.</returns>
        /// 
        public override string ToString() {

            return FullName;
        }

        /// <summary>
        /// Converteix un text a un objecte 'LayerId'.
        /// </summary>
        /// <param name="s">El text a procesar.</param>
        /// <returns>L'objecte 'LayerId' obtingut.</returns>
        /// 
        public static LayerId Parse(string s) {

            try {
                if (s.Contains(".")) {
                    string[] ss = s.Split('.');
                    BoardSide side = (BoardSide)Enum.Parse(typeof(BoardSide), ss[0]);
                    return new LayerId(ss[1], side);
                }
                else
                    return new LayerId(s, BoardSide.None);
            }
            catch (Exception ex) {
                throw new InvalidOperationException(
                    String.Format("No se pudo convertir el texto '{0}' a 'LayerId'.", s), 
                    ex);
            }
        }

        /// <summary>
        /// Operador ==
        /// </summary>
        /// <param name="a">Primer operand.</param>
        /// <param name="b">Segon operand.</param>
        /// <returns>Resultat de l'operacio.</returns>
        /// 
        public static bool operator ==(LayerId a, LayerId b) {

            return (a.name == b.name) && (a.side == b.side);
        }

        /// <summary>
        /// Operador !=
        /// </summary>
        /// <param name="a">Primer operand.</param>
        /// <param name="b">Segon operand.</param>
        /// <returns>Resultat de l'operacio.</returns>
        /// 
        public static bool operator !=(LayerId a, LayerId b) {

            return (a.name != b.name) || (a.side != b.side);
        }

        /// <summary>
        /// Obte el nom complert
        /// </summary>
        /// 
        public string FullName {
            get {
                StringBuilder sb = new StringBuilder();

                if (side == BoardSide.Top)
                    sb.Append("Top.");
                else if (side == BoardSide.Bottom)
                    sb.Append("Bottom.");
                sb.Append(name);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Obte el nom.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Obte la cara.
        /// </summary>
        /// 
        public BoardSide Side {
            get {
                return side;
            }
        }
    }

    public static class LayerIdHelper {

        public static void WriteAttribute(this XmlWriterAdapter wr, string name, LayerId layerId) {

            wr.WriteAttribute(name, layerId.FullName);
        }

        public static LayerId AttributeAsLayerId(this XmlReaderAdapter rd, string name) {

            string s = rd.AttributeAsString(name);
            return LayerId.Parse(s);
        }
    }
}
