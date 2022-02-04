using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder;
using MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder.Apertures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber {

    /// <summary>
    /// Clase que representa el diccionari d'apertures.
    /// </summary>
    internal sealed class ApertureDictionary {

        // Definicio del macro per l'apertura rectangle arrodonit.
        // $1: Amplada.
        // $2: Alçada.
        // $3: Radi de curvatura.
        // $4: Angle de rotacio.
        //
        private static readonly Macro _roundRectangleMacro = new Macro(
            1,
            "21,1,$1,$2-$3-$3,0,0,$4*\n" +
            "21,1,$1-$3-$3,$2,0,0,$4*\n" +
            "$5=$1/2*\n" +
            "$6=$2/2*\n" +
            "$7=2x$3*\n" +
            "1,1,$7,$5-$3,$6-$3,$4*\n" +
            "1,1,$7,-$5+$3,$6-$3,$4*\n" +
            "1,1,$7,-$5+$3,-$6+$3,$4*\n" +
            "1,1,$7,$5-$3,-$6+$3,$4*");

        // Definicio del macro per l'apertura rectangle.
        // $1: Amplada.
        // $2: Alçada.
        // $3: Angle de rotacio.
        //
        private static readonly Macro _rectangleMacro = new Macro(
            2,
            "21,1,$1,$2,0,0,$3*");

        // Definicio del macro per l'apertura ovalada.
        // $1: Amplada.
        // $2: Alçada.
        // $3: Angle de rotacio
        //
        //private static readonly Macro oblongApertureMacro = null;

        private readonly IList<Macro> _macros = new List<Macro>();
        private readonly IDictionary<int, Aperture> _items = new Dictionary<int, Aperture>();
        private int _apertureId = 10;

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// 
        public ApertureDictionary() {

            _macros.Add(_rectangleMacro);
            _macros.Add(_roundRectangleMacro);
            //macros.Add(oblongApertureMacro);
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus cercle.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La clau unica.</returns>
        /// 
        private static int GetCircleKey(int diameter, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "circle;{0};{1}", diameter, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus rectangle.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La clau unica.</returns>
        /// 
        private static int GetRectangleKey(int width, int height, EdaAngle rotation, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "rectangle;{0};{1};{2};{3}", width, height, rotation.Value, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus rectangle arrodonit.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="radius">Radi de corvatura.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La clau unica.</returns>
        /// 
        private static int GetRoundRectangleKey(int width, int height, int radius, EdaAngle rotation, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "round;{0};{1};{2};{3};{4}", width, height, radius, rotation.Value, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus octagon.
        /// </summary>
        /// <param name="size">Diametre extern..</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La clau unica.</returns>
        /// 
        private static int GetOctagonKey(int size, EdaAngle rotation, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "octagon;{0};{1};{2}", size, rotation.Value, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus oval.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>la clau unica.</returns>
        /// 
        private static int GetOvalKey(int width, int height, EdaAngle rotation, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "oval;{0};{1};{2};{3}", width, height, rotation.Value, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Afegeix una apertura de tipus cercle.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura afeigida.</returns>
        /// 
        public Aperture DefineCircleAperture(int diameter, string tag = null) {

            if (diameter <= 0)
                throw new ArgumentOutOfRangeException(nameof(diameter));

            int key = GetCircleKey(diameter, tag);
            if (!_items.TryGetValue(key, out Aperture ap)) {
                ap = new CircleAperture(_apertureId++, tag, diameter);
                _items.Add(key, ap);
            }

            return ap;
        }

        /// <summary>
        /// Afegeix una apertura de tipus rectangle.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura afeigida.</returns>
        /// 
        public Aperture DefineRectangleAperture(int width, int height, EdaAngle rotation, string tag = null) {

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            int key = GetRectangleKey(width, height, rotation, tag);
            if (!_items.TryGetValue(key, out Aperture ap)) {
                if (rotation.IsHorizontal)
                    ap = new RectangleAperture(_apertureId++, tag, width, height);
                else if (rotation.IsVertical)
                    ap = new RectangleAperture(_apertureId++, tag, height, width);
                else
                    ap = new MacroAperture(_apertureId++, tag, _rectangleMacro, width, height, rotation.Value);
                _items.Add(key, ap);
            }

            return ap;
        }

        /// <summary>
        /// Afegeix una apertura de tipus rectangle arrodonit.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="radius">Radi de corvatura.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura afeigida.</returns>
        /// 
        public Aperture DefineRoundRectangleAperture(int width, int height, int radius, EdaAngle rotation, string tag = null) {

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            if (radius <= 0)
                throw new ArgumentOutOfRangeException(nameof(radius));

            if (width - (2 * radius) <= 0)
                throw new ArgumentOutOfRangeException(nameof(radius));

            if (height - (2 * radius) <= 0)
                throw new ArgumentOutOfRangeException(nameof(radius));

            int key = GetRoundRectangleKey(width, height, radius, rotation, tag);
            if (!_items.TryGetValue(key, out Aperture ap)) {
                ap = new MacroAperture(_apertureId++, tag, _roundRectangleMacro, width, height, radius, rotation.Value * 10000);
                _items.Add(key, ap);
            }

            return ap;
        }

        /// <summary>
        /// Afegeix una apertura de tipus octagon.
        /// </summary>
        /// <param name="size">Diametre exterior.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura afeigida.</returns>
        /// 
        public Aperture DefineOctagonAperture(int size, EdaAngle rotation, string tag = null) {

            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            int key = GetOctagonKey(size, rotation, tag);
            if (!_items.TryGetValue(key, out Aperture ap)) {
                ap = new PoligonAperture(_apertureId++, tag, 8, size, rotation + EdaAngle.FromValue(2250));
                _items.Add(key, ap);
            }

            return ap;
        }

        /// <summary>
        /// Afegeix una apertura de tipus oval.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura afeigida.</returns>
        /// 
        public Aperture DefineOvalAperture(int width, int height, EdaAngle rotation, string tag = null) {

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            int key = GetOvalKey(width, height, rotation, tag);
            if (!_items.TryGetValue(key, out Aperture ap)) {
                if (rotation == EdaAngle.Zero || rotation == EdaAngle.Deg180)
                    ap = new ObroundAperture(_apertureId++, tag, width, height);
                else if (rotation == EdaAngle.Deg90 || rotation == EdaAngle.Deg270)
                    ap = new ObroundAperture(_apertureId++, tag, height, width);
                else
                    throw new NotImplementedException("Angulo de rotacion no implementado para la apertura oval.");

                _items.Add(key, ap);
            }

            return ap;
        }

        /// <summary>
        /// Obte una aperture de tipus cercle.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetCircleAperture(int diameter, string tag = null) {

            int key = GetCircleKey(diameter, tag);
            return _items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus rectangle.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetRectangleAperture(int width, int height, EdaAngle rotation, string tag = null) {

            int key = GetRectangleKey(width, height, rotation, tag);
            return _items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus rectangle arrodonit.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="radius">Radi de corvatura.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetRoundRectangleAperture(int width, int height, int radius, EdaAngle rotation, string tag = null) {

            int key = GetRoundRectangleKey(width, height, radius, rotation, tag);
            return _items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus octagon.
        /// </summary>
        /// <param name="size">Diametre extern.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetOctagonAperture(int size, EdaAngle rotation, string tag = null) {

            int key = GetOctagonKey(size, rotation, tag);
            return _items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus oval.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetOvalAperture(int width, int height, EdaAngle rotation, string tag = null) {

            int key = GetOvalKey(width, height, rotation, tag);
            return _items[key];
        }

        /// <summary>
        /// Enumera tots els macros en us.
        /// </summary>
        /// 
        public IEnumerable<Macro> Macros {
            get {
                List<Macro> macros = null;
                foreach (var aperture in _items.Values.OfType<MacroAperture>()) {

                    var macro = aperture.Macro;

                    if ((macros == null) || !macros.Contains(macro)) {
                        if (macros == null)
                            macros = new List<Macro>();
                        macros.Add(macro);
                    }
                }
                return macros;
            }
        }
        /// <summary>
        /// Enumera totes les apertures definides.
        /// </summary>
        /// 
        public IEnumerable<Aperture> Apertures =>
            _items.Count == 0 ? null : _items.Values;
    }
}
