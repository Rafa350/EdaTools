namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

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
        private readonly IDictionary<string, Aperture> items = new Dictionary<string, Aperture>();
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
        /// <returns>La clau unica.</returns>
        /// 
        private static string GetCircleKey(double diameter) {

            return String.Format(CultureInfo.InvariantCulture, 
                "circle;{0}", diameter);
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus rectangle.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Orientacio.</param>
        /// <returns>La clau unica.</returns>
        /// 
        private static string GetRectangleKey(double width, double height, Angle rotate) {

            return String.Format(CultureInfo.InvariantCulture, 
                "rectangle;{0};{1};{2}", width, height, rotate.Degrees);
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus rectangle arrodonit.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="radius">Radi de corvatura.</param>
        /// <param name="rotate">Orientacio.</param>
        /// <returns>La clau unica.</returns>
        /// 
        private static string GetRoundRectangleKey(double width, double height, double radius, Angle rotate) {

            return String.Format(CultureInfo.InvariantCulture, 
                "round_{0};{1};{2};{3}", width, height, radius, rotate.Degrees);
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus octagon.
        /// </summary>
        /// <param name="size">Diametre extern..</param>
        /// <param name="rotate">Orientacio.</param>
        /// <returns>La clau unica.</returns>
        /// 
        private static string GetOctagonKey(double size, Angle rotate) {

            return String.Format(CultureInfo.InvariantCulture, 
                "octagon;{0};{1}", size, rotate.Degrees);
        }

        /// <summary>
        /// Obte la clau unica per una apertura de tipus oval.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Orientacio.</param>
        /// <returns>la clau unica.</returns>
        /// 
        private static string GetOvalKey(double width, double height,Angle rotate) {

            return String.Format(CultureInfo.InvariantCulture, 
                "oval;{0};{1};{2}", width, height, rotate.Degrees);
        }

        /// <summary>
        /// Afegeix una apertura de tipus cercle.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// 
        public void DefineCircleAperture(double diameter) {

            string key = GetCircleKey(diameter);
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
        /// 
        public void DefineRectangleAperture(double width, double height, Angle rotate) {

            string key = GetRectangleKey(width, height, rotate);
            if (!items.ContainsKey(key)) {
                Aperture ap = new MacroAperture(apertureId++, rectangleMacro, width, height, rotate.Degrees);
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
        /// 
        public void DefineRoundRectangleAperture(double width, double height, double radius, Angle rotate) {

            string key = GetRoundRectangleKey(width, height, radius, rotate);
            if (!items.ContainsKey(key)) {
                Aperture ap = new MacroAperture(apertureId++, roundRectangleMacro, width, height, radius, rotate.Degrees);
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Afegeix una apertura de tipus octagon.
        /// </summary>
        /// <param name="size">Diametre exterior.</param>
        /// <param name="rotate">Orientacio.</param>
        /// 
        public void DefineOctagonAperture(double size, Angle rotate) {

            string key = GetOctagonKey(size, rotate);
            if (!items.ContainsKey(key)) {
                Aperture ap = new PoligonAperture(apertureId++, 8, size, rotate + Angle.FromDegrees(22.5));
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Afegeix una apertura de tipus oval.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Orientacio.</param>
        /// 
        public void DefineOvalAperture(double width, double height, Angle rotate) {

            string key = GetOvalKey(width, height, rotate);
            if (!items.ContainsKey(key)) {
                Aperture ap = new ObroundAperture(apertureId++, width, height);
                items.Add(key, ap);
            }
        }

        /// <summary>
        /// Obte una aperturew de tipus cercle.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetCircleAperture(double diameter) {

            string key = GetCircleKey(diameter);
            return items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus rectangle.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Rotacio.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetRectangleAperture(double width, double height, Angle rotate) {

            string key = GetRectangleKey(width, height, rotate);
            return items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus rectangle arrodonit.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="radius">Radi de corvatura.</param>
        /// <param name="rotate">Rotacio.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetRoundRectangleAperture(double width, double height, double radius, Angle rotate) {

            string key = GetRoundRectangleKey(width, height, radius, rotate);
            return items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus octagon.
        /// </summary>
        /// <param name="size">Diametre extern.</param>
        /// <param name="rotate">Rotacio.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetOctagonAperture(double size, Angle rotate) {

            string key = GetOctagonKey(size, rotate);
            return items[key];
        }

        /// <summary>
        /// Obte una apertura de tipus oval.
        /// </summary>
        /// <param name="width">Amplada.</param>
        /// <param name="height">Alçada.</param>
        /// <param name="rotate">Rotacio.</param>
        /// <returns>L'apertura.</returns>
        /// 
        public Aperture GetOvalAperture(double width, double height, Angle rotate) {

            string key = GetOvalKey(width, height, rotate);
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
