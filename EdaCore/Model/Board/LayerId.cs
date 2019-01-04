namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Objecte que representa el identificador unic d'una capa.
    /// </summary>
    /// 
    public readonly struct LayerId {

        private static readonly Regex rs;
        private static readonly Regex rc;
        private readonly string name;
        private readonly BoardSide side;

        static LayerId() {

            rs = new Regex(@"^[a-z0-9]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            rc = new Regex(@"^[a-z0-9]+(\.[a-z0-9]+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom de la capa.</param>
        /// 
        public LayerId(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (!rc.IsMatch(name))
                throw new InvalidOperationException(
                    String.Format("El formato del nombre '{0}', es incorrecto", name));

            if (name.Contains(".")) {
                string[] s = name.Split('.');
                this.name = s[1];
                side = (BoardSide)Enum.Parse(typeof(BoardSide), s[0], true);
            }
            else {
                this.name = name;
                side = BoardSide.None;
            }
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

            if (!rs.IsMatch(name))
                throw new InvalidOperationException(
                    String.Format("El formato del nombre '{0}', es incorrecto", name));

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

            if (String.IsNullOrEmpty(s))
                throw new ArgumentNullException("s");

            if (!rc.IsMatch(s))
                throw new InvalidOperationException(
                    String.Format("El formato de entrada '{0}', es incorrecto", s));

            if (s.Contains(".")) {
                string[] ss = s.Split('.');
                BoardSide side = (BoardSide)Enum.Parse(typeof(BoardSide), ss[0], true);
                return new LayerId(ss[1], side);
            }
            else
                return new LayerId(s, BoardSide.None);
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

            return !(a == b);
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

        /// <summary>
        /// Obte la cara oposada.
        /// </summary>
        /// 
        public BoardSide ReverseSide {
            get {
                if (side == BoardSide.Top)
                    return BoardSide.Bottom;
                else if (side == BoardSide.Bottom)
                    return BoardSide.Top;
                else
                    return side;
            }
        }
    }
}
