namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures;
    using MikroPic.EdaTools.v1.Geometry;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Clase que representa el diccionari d'apertures.
    /// </summary>
    internal sealed class ApertureDictionary {

        // Definicio del macro per l'apertura rectangle arrodonit.
        // $1: Amplada.
        // $2: Alçada.
        // $3: Radi de corvatura.
        // $4: Angle de rotacio.
        //
        private static readonly Macro roundRectangleMacro = new Macro(
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
        private static readonly Macro rectangleMacro = new Macro(
            2,
            "21,1,$1,$2,0,0,$3*");

        // Definicio del macro per l'apertura ovalada.
        // $1: Amplada.
        // $2: Alçada.
        // $3: Angle de rotacio
        //
        //private static readonly Macro oblongApertureMacro = null;

        private readonly IList<Macro> macros = new List<Macro>();
        private readonly IDictionary<int, Aperture> items = new Dictionary<int, Aperture>();
        private int apertureId = 10;

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// 
        public ApertureDictionary() {

            macros.Add(rectangleMacro);
            macros.Add(roundRectangleMacro);
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
        private static int GetRectangleKey(int width, int height, Angle rotation, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "rectangle;{0};{1};{2};{3}", width, height, rotation.Degrees, tag);
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
        private static int GetRoundRectangleKey(int width, int height, int radius, Angle rotation, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "round;{0};{1};{2};{3};{4}", width, height, radius, rotation.Degrees, tag);
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
        private static int GetOctagonKey(int size, Angle rotation, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "octagon;{0};{1};{2}", size, rotation.Degrees, tag);
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
        private static int GetOvalKey(int width, int height, Angle rotation, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "oval;{0};{1};{2};{3}", width, height, rotation.Degrees, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Afegeix una apertura de tipus cercle.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="tag">Etiqueta.</param>
        /// 
        public void DefineCircleAperture(int diameter, string tag = null) {

            if (diameter <= 0)
                throw new ArgumentOutOfRangeException("diameter");

            int key = GetCircleKey(diameter, tag);
            if (!items.ContainsKey(key)) {
                Aperture ap = new CircleAperture(apertureId++, tag, diameter);
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Afegeix una apertura de tipus rectangle.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// 
        public void DefineRectangleAperture(int width, int height, Angle rotation, string tag = null) {

            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");

            int key = GetRectangleKey(width, height, rotation, tag);
            if (!items.ContainsKey(key)) {
                Aperture ap;
                if (rotation.IsHorizontal)
                    ap = new RectangleAperture(apertureId++, tag, width, height);
                else if (rotation.IsVertical)
                    ap = new RectangleAperture(apertureId++, tag, height, width);
                else
                    ap = new MacroAperture(apertureId++, tag, rectangleMacro, width, height, rotation.Degrees);
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Afegeix una apertura de tipus rectangle arrodonit.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="radius">Radi de corvatura.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// 
        public void DefineRoundRectangleAperture(int width, int height, int radius, Angle rotation, string tag = null) {

            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");

            if (radius <= 0)
                throw new ArgumentOutOfRangeException("radius");

            if (width - (2 * radius) <= 0)
                throw new ArgumentOutOfRangeException("radius");

            if (height - (2 * radius) <= 0)
                throw new ArgumentOutOfRangeException("radius");

            int key = GetRoundRectangleKey(width, height, radius, rotation, tag);
            if (!items.ContainsKey(key)) {
                Aperture ap = new MacroAperture(apertureId++, tag, roundRectangleMacro, width, height, radius, rotation.Degrees * 10000);
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Afegeix una apertura de tipus octagon.
        /// </summary>
        /// <param name="size">Diametre exterior.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// 
        public void DefineOctagonAperture(int size, Angle rotation, string tag = null) {

            if (size <= 0)
                throw new ArgumentOutOfRangeException("size");

            int key = GetOctagonKey(size, rotation, tag);
            if (!items.ContainsKey(key)) {
                Aperture ap = new PoligonAperture(apertureId++, tag, 8, size, rotation + Angle.FromDegrees(2250));
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Afegeix una apertura de tipus oval.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// 
        public void DefineOvalAperture(int width, int height, Angle rotation, string tag = null) {

            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");

            if (height <= 0)
                throw new ArgumentOutOfRangeException("height");

            int key = GetOvalKey(width, height, rotation, tag);
            if (!items.ContainsKey(key)) {
                Aperture ap = new ObroundAperture(apertureId++, tag, width, height);
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Obte una aperturew de tipus cercle.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetCircleAperture(int diameter, string tag = null) {

            int key = GetCircleKey(diameter, tag);
            return items[key];
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
        public Aperture GetRectangleAperture(int width, int height, Angle rotation, string tag = null) {

            int key = GetRectangleKey(width, height, rotation, tag);
            return items[key];
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
        public Aperture GetRoundRectangleAperture(int width, int height, int radius, Angle rotation, string tag = null) {

            int key = GetRoundRectangleKey(width, height, radius, rotation, tag);
            return items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus octagon.
        /// </summary>
        /// <param name="size">Diametre extern.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetOctagonAperture(int size, Angle rotation, string tag = null) {

            int key = GetOctagonKey(size, rotation, tag);
            return items[key];
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
        public Aperture GetOvalAperture(int width, int height, Angle rotation, string tag = null) {

            int key = GetOvalKey(width, height, rotation, tag);
            return items[key];
        }

        /// <summary>
        /// Enumera tots els macros definits.
        /// </summary>
        /// 
        public IEnumerable<Macro> Macros {
            get {
                return macros;
            }
        }

        /// <summary>
        /// Enumera totes les apertures definides.
        /// </summary>
        /// 
        public IEnumerable<Aperture> Apertures {
            get {
                return items.Values;
            }
        }
    }
}
