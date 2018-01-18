﻿namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa un arc.
    /// </summary>
    public sealed class ArcElement: LineElement, IConectable {

        private double angle;

        /// <summary>
        /// Constructor de l'objecte, amb els parametres per defecte.
        /// </summary>
        /// 
        public ArcElement():
            base() {
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="startPosition">Punt inicial.</param>
        /// <param name="endPosition">Punt final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <param name="angle">Angle del arc.</param>
        /// <param name="lineCap">Extrems de linia.</param>
        /// 
        public ArcElement(Point startPosition, Point endPosition, double thickness, double angle, LineCapStyle lineCap) :
            base(startPosition, endPosition, thickness, lineCap) {

            this.angle = angle;
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Crea el poligon del objecte.
        /// </summary>
        /// <param name="inflate">Increment de tamany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(double inflate = 0) {

            return PolygonBuilder.BuildArcSegment(Center, Radius, StartAngle, EndAngle, Thickness + (inflate * 2));
        }

        /// <summary>
        /// Calcula el numero de serie del objecte
        /// </summary>
        /// <returns>El numero de serie.</returns>
        /// 
        protected override int GetSerial() {

            string s = String.Format("{0}${1}${2}${3}${4}${5}${6}${7}", 
                GetType().FullName, 
                StartPosition.X, 
                StartPosition.Y, 
                EndPosition.X, 
                EndPosition.Y, 
                angle, 
                Thickness,
                LineCap);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte o asigna l'angle del arc.
        /// </summary>
        /// 
        public double Angle {
            get {
                return angle;
            }
            set {
                angle = value;
            }
        }

        /// <summary>
        /// Obte el centre de l'arc.
        /// </summary>
        /// 
        public Point Center {
            get {
                double rAngle = angle * Math.PI / 180.0;
                double x1 = StartPosition.X;
                double y1 = EndPosition.Y;
                double x2 = EndPosition.X;
                double y2 = EndPosition.Y;

                // Calcula el punt central
                //
                double mx = (x1 + x2) / 2.0;
                double my = (y1 + y2) / 2.0;

                // Calcula la distancia entre els dos punts.
                //
                double d = Math.Sqrt(Math.Pow(x2 - x1, 2.0) + Math.Pow(y2 - y1, 2.0));

                // Calcula el radi
                //
                double r = Math.Abs((d / 2.0) / Math.Sin(rAngle / 2.0));

                // Calcula el centre
                //
                if (angle > 0)
                    return new Point(
                        mx + Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (y1 - y2) / d,
                        my + Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (x2 - x1) / d);

                else
                    return new Point(
                        mx - Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (y1 - y2) / d,
                        my - Math.Sqrt(Math.Pow(r, 2.0) - Math.Pow((d / 2.0), 2.0)) * (x2 - x1) / d);
            }
        }

        /// <summary>
        /// Obte l'angle inicial del arc.
        /// </summary>
        /// 
        public double StartAngle {
            get {
                Point c = Center;
                double a = Math.Atan((StartPosition.Y - c.Y) / (StartPosition.X - c.X));
                return a * 180.0 / Math.PI;
            }
        }

        /// <summary>
        /// Obte l'angle final de l'arc.
        /// </summary>
        /// 
        public double EndAngle {
            get {
                Point c = Center;
                double a = Math.Atan((EndPosition.Y - c.Y) / (EndPosition.X - c.X));
                return a * 180.0 / Math.PI;
            }
        }

        /// <summary>
        /// Obte el radi de l'arc.
        /// </summary>
        /// 
        public double Radius {
            get {
                double a = angle * Math.PI / 180.0;
                return Math.Abs(Length / 2.0 / Math.Sin(a / 2.0));
            }
        }
    }
}
