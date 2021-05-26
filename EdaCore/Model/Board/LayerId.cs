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
        public static readonly LayerId ViaRestrict;
        public static readonly LayerId TopDocument;
        public static readonly LayerId BottomDocument;
        public static readonly LayerId TopKeepout;
        public static readonly LayerId BottomKeepout;
        public static readonly LayerId Profile;
        public static readonly LayerId Dimensions;
        public static readonly LayerId Milling;
        public static readonly LayerId Unrouted;

        private static readonly Dictionary<string, int> _fwDict;
        private static readonly Dictionary<int, string> _bkDict;
        private static int _idCount = 1000; 

        private const int _valuePOS = 0;
        private const int _valueBITS = 0xFF;
        private const int _valueMASK = _valueBITS << _valuePOS;
        private const int _valueCopper0 = 1 << _valuePOS;
        private const int _valueCopper1 = 2 << _valuePOS;
        private const int _valueCopper2 = 3 << _valuePOS;
        private const int _valueCopper3 = 4 << _valuePOS;
        private const int _valueCopper4 = 5 << _valuePOS;
        private const int _valueCopper5 = 6 << _valuePOS;
        private const int _valueCopper6 = 7 << _valuePOS;
        private const int _valueCopper7 = 8 << _valuePOS;
        private const int _valueCopper8 = 9 << _valuePOS;
        private const int _valueCopper9 = 10 << _valuePOS;
        private const int _valueCopper10 = 11 << _valuePOS;
        private const int _valueCopper11 = 12 << _valuePOS;
        private const int _valueCopper12 = 13 << _valuePOS;
        private const int _valueCopper13 = 14 << _valuePOS;
        private const int _valueCopper14 = 15 << _valuePOS;
        private const int _valueCopper15 = 16 << _valuePOS;
        private const int _valueTopNames = 17 << _valuePOS;
        private const int _valueBottomNames = 18 << _valuePOS;
        private const int _valueTopValues = 19 << _valuePOS;
        private const int _valueBottomValues = 20 << _valuePOS;
        private const int _valueTopStop = 21 << _valuePOS;
        private const int _valueBottomStop = 22 << _valuePOS;
        private const int _valueTopCream = 23 << _valuePOS;
        private const int _valueBottomCream = 24 << _valuePOS;
        private const int _valueTopGlue = 25 << _valuePOS;
        private const int _valueBottomGlue = 26 << _valuePOS;
        private const int _valueTopPlace = 27 << _valuePOS;
        private const int _valueBottomPlace = 28 << _valuePOS;
        private const int _valueTopDocument = 29 << _valuePOS;
        private const int _valueBottomDocument = 30 << _valuePOS;
        private const int _valueDrills = 31;
        private const int _valueHoles = 32;
        private const int _valueVias = 33;
        private const int _valuePads = 34;
        private const int _valueProfile = 35;
        private const int _valueDimensions = 36;
        private const int _valueMilling = 37;
        private const int _valueTopRestrict = 38;
        private const int _valueBottomRestrict = 39;
        private const int _valueInnerRestrict = 40;
        private const int _valueViaRestrict = 41;
        private const int _valueTopKeepout = 42;
        private const int _valueBottomKeepout = 43;
        private const int _valueUnrouted = 44;

        private const int _sidePOS = 16;
        private const int _sideBITS = 0b11;
        private const int _sideMASK = _sideBITS << _sidePOS;
        private const int _sideTop = 1 << _sidePOS;
        private const int _sideBottom = 2 << _sidePOS;
        private const int _sideInner = 3 << _sidePOS;

        private const int _typePOS = 18;
        private const int _typeBITS = 0b111;
        private const int _typeMASK = _typeBITS << _typePOS;
        private const int _typeUnknown = 0 << _typePOS;
        private const int _typeSignal = 1 << _typePOS;
        private const int _typeOutline = 2 << _typePOS;
        private const int _typeUser = 3 << _typePOS;
        private const int _typeRestrict = 4 << _typePOS;

        private readonly int _value;

        /// <summary>
        /// Constructor estatic
        /// </summary>
        /// 
        static LayerId() {

            _fwDict = new Dictionary<string, int>();
            _bkDict = new Dictionary<int, string>();

            TopCopper = Create(_valueCopper0 | _sideTop | _typeSignal, "Top.Copper");
            InnerCopper1 = Create(_valueCopper1 | _sideInner | _typeSignal, "Inner.Copper1");
            InnerCopper2 = Create(_valueCopper2 | _sideInner | _typeSignal, "Inner.Copper2");
            InnerCopper3 = Create(_valueCopper3 | _sideInner | _typeSignal, "Inner.Copper3");
            InnerCopper4 = Create(_valueCopper4 | _sideInner | _typeSignal, "Inner.Copper4");
            InnerCopper5 = Create(_valueCopper5 | _sideInner | _typeSignal, "Inner.Copper5");
            InnerCopper6 = Create(_valueCopper6 | _sideInner | _typeSignal, "Inner.Copper6");
            InnerCopper7 = Create(_valueCopper7 | _sideInner | _typeSignal, "Inner.Copper7");
            InnerCopper8 = Create(_valueCopper8 | _sideInner | _typeSignal, "Inner.Copper8");
            InnerCopper9 = Create(_valueCopper9 | _sideInner | _typeSignal, "Inner.Copper9");
            InnerCopper10 = Create(_valueCopper10 | _sideInner | _typeSignal, "Inner.Copper10");
            InnerCopper11 = Create(_valueCopper11 | _sideInner | _typeSignal, "Inner.Copper11");
            InnerCopper12 = Create(_valueCopper12 | _sideInner | _typeSignal, "Inner.Copper12");
            InnerCopper13 = Create(_valueCopper13 | _sideInner | _typeSignal, "Inner.Copper13");
            InnerCopper14 = Create(_valueCopper14 | _sideInner | _typeSignal, "Inner.Copper14");
            BottomCopper = Create(_valueCopper15 | _sideBottom | _typeSignal, "Bottom.Copper");
            Pads = Create(_valuePads, "Pads");
            Vias = Create(_valueVias, "Vias");
            Drills = Create(_valueDrills, "Drills");
            Holes = Create(_valueHoles, "Holes");
            TopNames = Create(_valueTopNames | _sideTop, "Top.Names");
            BottomNames = Create(_valueBottomNames | _sideTop, "Bottom.Names");
            TopValues = Create(_valueTopValues, "Top.Values");
            BottomValues = Create(_valueBottomValues, "Bottom.Values");
            TopDocument = Create(_valueTopDocument, "Top.Document");
            BottomDocument = Create(_valueBottomDocument, "Bottom.Document");
            TopPlace = Create(_valueTopPlace | _sideTop, "Top.Place");
            BottomPlace = Create(_valueBottomPlace | _sideBottom, "Bottom.Place");
            TopStop = Create(_valueTopStop | _sideTop, "Top.Stop");
            BottomStop = Create(_valueBottomStop | _sideBottom, "Bottom.Stop");
            TopCream = Create(_valueTopCream | _sideTop, "Top.Cream");
            BottomCream = Create(_valueBottomCream | _sideBottom, "Bottom.Cream");
            TopGlue = Create(_valueTopGlue | _sideTop, "Top.Glue");
            BottomGlue = Create(_valueBottomGlue | _sideBottom, "Bottom.Glue");
            TopRestrict = Create(_valueTopRestrict | _sideTop, "Top.Restrict");
            BottomRestrict = Create(_valueBottomRestrict | _sideBottom | _typeRestrict, "Bottom.Restrict");
            InnerRestrict = Create(_valueInnerRestrict | _sideInner | _typeRestrict, "Inner.Restrict");
            ViaRestrict = Create(_valueViaRestrict | _typeRestrict, "Via.Restrict");
            TopKeepout = Create(_valueTopKeepout | _sideTop, "Top.Keepout");
            BottomKeepout = Create(_valueBottomKeepout | _sideBottom, "Bottom.Keepout");
            Profile = Create(_valueProfile | _typeOutline, "Profile");
            Dimensions = Create(_valueDimensions, "Dimensions");
            Milling = Create(_valueMilling, "Milling");
            Unrouted = Create(_valueUnrouted, "Unrouted");
        }

        /// <summary>
        /// Constructor privat.
        /// </summary>
        /// <param name="value">Valor del identificador.</param>
        /// 
        private LayerId(int value) {

            _value = value;
        }

        /// <summary>
        /// Obte un identificador o crea un de nou si no existeix.
        /// </summary>
        /// <param name="name">Nom.</param>
        /// <returns>El identificador.</returns>
        /// 
        private static LayerId Create(int value, string name) {

            _fwDict.Add(name, value);
            _bkDict.Add(value, name);

            return new LayerId(value);
        }

        /// <summary>
        /// Obte un identificador o crea un de nou si no existeix.
        /// </summary>
        /// <param name="name">Nom.</param>
        /// <returns>El identificador.</returns>
        /// 
        public static LayerId Get(string name) {

            if (!_fwDict.TryGetValue(name, out int value)) {
                value = _idCount++ | _typeUser;
                _fwDict.Add(name, value);
                _bkDict.Add(value, name);
            }

            return new LayerId(value);
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
        /// Representa el identificador com a text.
        /// </summary>
        /// <returns>El resultat.</returns>
        /// 
        public override string ToString() {

            return _bkDict[_value];
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
            _value;

        /// <summary>
        /// Comprova si son iguals
        /// </summary>
        /// <param name="other">L'objecte a comparar.</param>
        /// <returns>TRue si son iguals.</returns>
        /// 
        public bool Equals(LayerId other) =>
            _value == other._value;

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

        public LayerId Flip() {
            
            switch (_value & _sideMASK) {
                case _sideTop:
                    return new LayerId((_value & _sideMASK) | _sideBottom);
            
                case _sideBottom:
                    return new LayerId((_value & _sideMASK) | _sideTop);
                
                default:
                    return new LayerId(_value);
            }
        }

        public bool IsTopCopper =>
            (_value & _valueMASK) == _valueCopper0;

        public bool IsBottomCopper =>
            (_value & _valueMASK) == _valueCopper15;

        public bool IsTop =>
            (_value & _sideMASK) == _sideTop;

        public bool IsBottom =>
            (_value & _sideMASK) == _sideBottom;

        public bool IsInner =>
            (_value & _sideMASK) == _sideInner;

        public bool IsSignal =>
            (_value & _typeMASK) == _typeSignal;

        public bool IsOutline =>
            (_value & _typeMASK) == _typeOutline;

        public bool IsUser =>
            (_value & _typeMASK) == _typeUser;

        public BoardSide Side {
            get {
                switch (_value & _sideMASK) {
                    case _sideTop:
                        return BoardSide.Top;

                    case _sideBottom:
                        return BoardSide.Bottom;

                    case _sideInner:
                        return BoardSide.Inner;

                    default:
                        return BoardSide.None;
                }
            }
        }
    }
}
