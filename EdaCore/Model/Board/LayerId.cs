using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Represente el identificador d'una capa.
    /// </summary>
    /// 
    public readonly struct LayerId {

        public static readonly LayerId TopCopper;
        public static readonly LayerId BottomCopper;
        public static readonly LayerId InnerCopper1;
        public static readonly LayerId InnerCopper2;
        public static readonly LayerId InnerCopper3;
        public static readonly LayerId InnerCopper4;
        public static readonly LayerId InnerCopper5;
        public static readonly LayerId InnerCopper6;
        public static readonly LayerId InnerCopper7;
        public static readonly LayerId InnerCopper8;
        public static readonly LayerId InnerCopper9;
        public static readonly LayerId InnerCopper10;
        public static readonly LayerId InnerCopper11;
        public static readonly LayerId InnerCopper12;
        public static readonly LayerId InnerCopper13;
        public static readonly LayerId InnerCopper14;
        public static readonly LayerId Pads;
        public static readonly LayerId Vias;
        public static readonly LayerId Drills;
        public static readonly LayerId Holes;
        public static readonly LayerId TopStop;
        public static readonly LayerId BottomStop;
        public static readonly LayerId TopCream;
        public static readonly LayerId BottomCream;
        public static readonly LayerId TopGlue;
        public static readonly LayerId BottomGlue;
        public static readonly LayerId TopNames;
        public static readonly LayerId BottomNames;
        public static readonly LayerId TopValues;
        public static readonly LayerId BottomValues;
        public static readonly LayerId TopPlace;
        public static readonly LayerId BottomPlace;
        public static readonly LayerId TopRestrict;
        public static readonly LayerId BottomRestrict;
        public static readonly LayerId InnerRestrict;
        public static readonly LayerId Profile;
        public static readonly LayerId Dimensions;
        public static readonly LayerId Milling;

        private static readonly Dictionary<string, int> _fwDict;
        private static readonly Dictionary<int, string> _bkDict;
        private static int _idCount = 0;
        
        private readonly int _id;

        /// <summary>
        /// Constructor estatic
        /// </summary>
        /// 
        static LayerId() {

            _fwDict = new Dictionary<string, int>();
            _bkDict = new Dictionary<int, string>();

            TopCopper        = Get("Top.Copper");
            BottomCopper     = Get("Bottom.Copper");
            InnerCopper1     = Get("Inner.Copper1");
            InnerCopper2     = Get("Inner.Copper2");
            InnerCopper3     = Get("Inner.Copper3");
            InnerCopper4     = Get("Inner.Copper4");
            InnerCopper5     = Get("Inner.Copper5");
            InnerCopper6     = Get("Inner.Copper6");
            InnerCopper7     = Get("Inner.Copper7");
            InnerCopper8     = Get("Inner.Copper8");
            InnerCopper9     = Get("Inner.Copper9");
            InnerCopper10    = Get("Inner.Copper10");
            InnerCopper11    = Get("Inner.Copper11");
            InnerCopper12    = Get("Inner.Copper12");
            InnerCopper13    = Get("Inner.Copper13");
            InnerCopper14    = Get("Inner.Copper14");
            Pads             = Get("Pads");
            Vias             = Get("Vias");
            Drills           = Get("Drills");
            Holes            = Get("Holes");
            TopNames         = Get("Top.Names");
            BottomNames      = Get("Bottom.Names");
            TopValues        = Get("Top.Values");
            BottomValues     = Get("Bottom.Values");
            TopPlace         = Get("Top.Place");
            BottomPlace      = Get("Bottom.Place");
            TopStop          = Get("Top.Stop");
            BottomStop       = Get("Bottom.Stop");
            TopCream         = Get("Top.Cream");
            BottomCream      = Get("Bottom.Cream");
            TopGlue          = Get("Top.Glue");
            BottomGlue       = Get("Bottom.Glue");
            TopRestrict      = Get("Top.Restrict");
            BottomRestrict   = Get("Bottom.Restrict");
            InnerRestrict    = Get("Inner.Restrict");
            Profile          = Get("Profile");
            Dimensions       = Get("Dimensions");
            Milling          = Get("Milling");
        }

        /// <summary>
        /// Constructor privat.
        /// </summary>
        /// <param name="id">Valor del identificador.</param>
        /// 
        private LayerId(int id) {

            _id = id;
        }

        /// <summary>
        /// Comprova si el identificador existeix.
        /// </summary>
        /// <param name="name">Nom.</param>
        /// <returns>True si existeix.</returns>
        /// 
        public static bool Exists(string name) {

            return _fwDict.ContainsKey(name);
        }

        /// <summary>
        /// Obte un identificador o crea un de nou si no existeix.
        /// </summary>
        /// <param name="name">Nom.</param>
        /// <returns>El identificador.</returns>
        /// 
        public static LayerId Get(string name) {

            if (!_fwDict.TryGetValue(name, out int id)) {
                id = _idCount++;
                _fwDict.Add(name, id);
                _bkDict.Add(id, name);
            }

            return new LayerId(id);
        }

        /// <summary>
        /// Representa el identificador com a text.
        /// </summary>
        /// <returns>El resultat.</returns>
        /// 
        public override string ToString() {

            return _bkDict[_id];
        }

        /// <summary>
        /// Converteis el valor desde un text.
        /// </summary>
        /// <param name="s">El text.</param>
        /// <returns>El resultat.</returns>
        /// 
        public static LayerId Parse(string s) {

            return Get(s);
        }

        /// <summary>
        /// Calcula el hash
        /// </summary>
        /// <returns>El resulta.</returns>
        /// 
        public override int GetHashCode() =>
            _id;

        /// <summary>
        /// Comprova si son iguals
        /// </summary>
        /// <param name="other">L'objecte a comparar.</param>
        /// <returns>TRue si son iguals.</returns>
        /// 
        public bool Equals(LayerId other) =>
            _id == other._id;

        /// <summary>
        /// Comprova si son iguals
        /// </summary>
        /// <param name="other">L'objecte a comparar.</param>
        /// <returns>TRue si son iguals.</returns>
        /// 
        public override bool Equals(object obj) =>
            (obj is LayerId other) && Equals(other);

        public static bool operator ==(LayerId a, LayerId b) =>
            a.Equals(b);

        public static bool operator !=(LayerId a, LayerId b) =>
            !a.Equals(b);
    }
}
