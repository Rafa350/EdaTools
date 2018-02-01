namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures;
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
        private static int GetCircleKey(double diameter, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "circle;{0};{1}", diameter, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus rectangle.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Orientacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La clau unica.</returns>
        /// 
        private static int GetRectangleKey(double width, double height, double rotate, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "rectangle;{0};{1};{2};{3}", width, height, rotate, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus rectangle arrodonit.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="radius">Radi de corvatura.</param>
        /// <param name="rotate">Orientacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La clau unica.</returns>
        /// 
        private static int GetRoundRectangleKey(double width, double height, double radius, double rotate, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "round;{0};{1};{2};{3};{4}", width, height, radius, rotate, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus octagon.
        /// </summary>
        /// <param name="size">Diametre extern..</param>
        /// <param name="rotate">Orientacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La clau unica.</returns>
        /// 
        private static int GetOctagonKey(double size, double rotate, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "octagon;{0};{1};{2}", size, rotate, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus oval.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Orientacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>la clau unica.</returns>
        /// 
        private static int GetOvalKey(double width, double height, double rotate, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "oval;{0};{1};{2};{3}", width, height, rotate, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Afegeix una apertura de tipus cercle.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="tag">Etiqueta.</param>
        /// 
        public void DefineCircleAperture(double diameter, string tag = null) {

            int key = GetCircleKey(diameter, tag);
            if (!items.ContainsKey(key)) {
                Aperture ap = new CircleAperture(apertureId++, diameter);
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Afegeix una apertura de tipus rectangle.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Orientacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// 
        public void DefineRectangleAperture(double width, double height, double rotate, string tag = null) {

            int key = GetRectangleKey(width, height, rotate, tag);
            if (!items.ContainsKey(key)) {
                Aperture ap = new MacroAperture(apertureId++, rectangleMacro, width, height, rotate);
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Afegeix una apertura de tipus rectangle arrodonit.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="radius">Radi de corvatura.</param>
        /// <param name="rotate">orientacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// 
        public void DefineRoundRectangleAperture(double width, double height, double radius, double rotate, string tag = null) {

            int key = GetRoundRectangleKey(width, height, radius, rotate, tag);
            if (!items.ContainsKey(key)) {
                Aperture ap = new MacroAperture(apertureId++, roundRectangleMacro, width, height, radius, rotate);
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Afegeix una apertura de tipus octagon.
        /// </summary>
        /// <param name="size">Diametre exterior.</param>
        /// <param name="rotate">Orientacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// 
        public void DefineOctagonAperture(double size, double rotate, string tag = null) {

            int key = GetOctagonKey(size, rotate, tag);
            if (!items.ContainsKey(key)) {
                Aperture ap = new PoligonAperture(apertureId++, 8, size, rotate + 22.5);
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Afegeix una apertura de tipus oval.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Orientacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// 
        public void DefineOvalAperture(double width, double height, double rotate, string tag = null) {

            int key = GetOvalKey(width, height, rotate, tag);
            if (!items.ContainsKey(key)) {
                Aperture ap = new ObroundAperture(apertureId++, width, height);
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
        public Aperture GetCircleAperture(double diameter, string tag = null) {

            int key = GetCircleKey(diameter, tag);
            return items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus rectangle.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetRectangleAperture(double width, double height, double rotate, string tag = null) {

            int key = GetRectangleKey(width, height, rotate, tag);
            return items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus rectangle arrodonit.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="radius">Radi de corvatura.</param>
        /// <param name="rotate">Rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetRoundRectangleAperture(double width, double height, double radius, double rotate, string tag = null) {

            int key = GetRoundRectangleKey(width, height, radius, rotate, tag);
            return items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus octagon.
        /// </summary>
        /// <param name="size">Diametre extern.</param>
        /// <param name="rotate">Rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetOctagonAperture(double size, double rotate, string tag = null) {

            int key = GetOctagonKey(size, rotate, tag);
            return items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus oval.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Rotacio.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetOvalAperture(double width, double height, double rotate, string tag = null) {

            int key = GetOvalKey(width, height, rotate, tag);
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
