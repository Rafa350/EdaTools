namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure.Polygons;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa un pad superficial
    /// </summary>
    public sealed class SmdPadElement: PadElement, IRotation {

        private Size size;
        private Angle rotation;
        private Ratio roundness;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom del pad.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="size">Tamany</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="roundness">Percentatge d'arrodoniment de les cantonades.</param>
        /// 
        public SmdPadElement(string name, Point position, Size size, Angle rotation, Ratio roundness) :
            base(name, position) {

            this.size = size;
            this.rotation = rotation;
            this.roundness = roundness;
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
        /// Crea el poligon del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(BoardSide side) {

            Point[] points = PolygonBuilder.BuildRectangle(Position, Size, Radius, rotation);
            return new Polygon(points);
        }

        /// <summary>
        /// Crea el poligon exterior del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetOutlinePolygon(BoardSide side, int spacing) {

            Point[] points = PolygonBuilder.BuildRectangle(
                Position,
                new Size(
                    size.Width + spacing + spacing, 
                    size.Height + spacing + spacing), 
                Radius + spacing, 
                rotation);
            return new Polygon(points);
        }

        /// <summary>
        /// Crea el poligon del thermal.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <param name="spacing">Espaiat.</param>
        /// <param name="width">Amplada dels conductors.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetThermalPolygon(BoardSide side, int spacing, int width) {

            Polygon pour = GetOutlinePolygon(side, spacing);
            Polygon thermal = new Polygon(
                PolygonBuilder.BuildCross(
                    Position,
                    new Size(
                        size.Width + spacing + spacing,
                        size.Height + spacing + spacing),
                    width,
                    rotation));

            List<Polygon> childs = new List<Polygon>();
            childs.AddRange(PolygonProcessor.Clip(pour, thermal, PolygonProcessor.ClipOperation.Diference));
            if (childs.Count != 4)
                throw new InvalidProgramException("Thermal generada incorrectamente.");
            return new Polygon(null, childs.ToArray());
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <param name="side">Cara de la placa.</param>
        /// <returns>El bounding box.</returns>
        /// 
        public override Rect GetBoundingBox(BoardSide side) {

            double a = rotation.Radiants;
            int w = (int) (size.Width * Math.Cos(a) + size.Height * Math.Sin(a));
            int h = (int) (size.Width * Math.Sin(a) + size.Height * Math.Cos(a));

            return new Rect(Position.X - (w >> 1), Position.Y - (h >> 1), w, h);
        }

        /// <summary>
        /// Obte o asigna l'orientacio del pad.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                 rotation = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany del pad.
        /// </summary>
        /// 
        public Size Size {
            get {
                return size;
            }
            set {
                 size = value;
            }
        }

        /// <summary>
        /// Obte o asigna el factor d'arrodoniment de les cantonades del pad.
        /// </summary>
        /// 
        public Ratio Roundness {
            get {
                return roundness;
            }
            set {
                 roundness = value;
            }
        }

        /// <summary>
        /// Obte el radi de curvatura de les cantonades.
        /// </summary>
        /// 
        public int Radius {
            get {
                return (Math.Min(size.Width, size.Height) * roundness) / 2;
            }
        }
    }
}
