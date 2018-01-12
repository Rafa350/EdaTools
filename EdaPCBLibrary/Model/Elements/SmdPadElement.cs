namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using System;
    using System.Windows;

    /// <summary>
    /// Clase que representa un pad superficial
    /// </summary>
    public sealed class SmdPadElement: Element, IPosition, IRotation, IName, IConectable {

        private string name;
        private Point position;
        private LayerId layerId;
        private Size size;
        private double rotation;
        private double roundnes;
        private bool cream = true;
        private bool stop = true;

        /// <summary>
        /// Constructor de l'objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public SmdPadElement():
            base() {
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom del pad.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="layerId">Identificador de la capa.</param>
        /// <param name="size">Tamany</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <param name="roundnes">Percentatge d'arrodoniment de les cantonades.</param>
        /// <param name="stop">Genera mascara.</param>
        /// <param name="cream">Genera encolat.</param>
        /// 
        public SmdPadElement(
            string name, 
            Point position, 
            LayerId layerId, 
            Size size, 
            double rotation, 
            double roundnes, 
            bool stop = true, 
            bool cream = true):
            
            base() {

            this.name = name;
            this.position = position;
            this.layerId = layerId;
            this.size = size;
            this.rotation = rotation;
            this.roundnes = roundnes;
            this.stop = stop;
            this.cream = cream;
        }

        /// <summary>
        /// Comprova si l'objecte pertany a la capa especificada.
        /// </summary>
        /// <param name="layerId">Identificador de la capa.</param>
        /// <returns>True si pertany, false en cas contrari.</returns>
        /// 
        public override bool IsOnLayer(
            LayerId layerId) {

            if (this.layerId == layerId)
                return true;
            else if ((this.layerId == LayerId.Top) && (layerId == LayerId.TopStop) && stop)
                return true;
            else if ((this.layerId == LayerId.Bottom) && (layerId == LayerId.BottomStop) && stop)
                return true;
            else if ((this.layerId == LayerId.Top) && (layerId == LayerId.TopCream) && cream)
                return true;
            else if ((this.layerId == LayerId.Bottom) && (layerId == LayerId.BottomCream) && cream)
                return true;
            else
                return false;
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
        /// <param name="inflate">Increment de tamany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public override Polygon GetPolygon(
            double inflate = 0) {

            return PolygonBuilder.BuildRectangle(position,
                new Size(size.Width + (inflate * 2), size.Height + (inflate * 2)), Radius + inflate, rotation);
        }

        /// <summary>
        /// Calcula el bounding box del element.
        /// </summary>
        /// <returns>El bounding box.</returns>
        /// 
        protected override Rect GetBoundingBox() {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Obte o asigna el nom.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre geometric del pad.
        /// </summary>
        /// 
        public Point Position {
            get {
                return position;
            }
            set {
                if (position != value) {
                    position = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Obte o asigna l'orientacio del pad.
        /// </summary>
        /// 
        public double Rotation {
            get {
                return rotation;
            }
            set {
                if (rotation != value) {
                    rotation = value;
                    Invalidate();
                }
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
                if (size != value) {
                    size = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Obte o asigna el factor d'arrodoniment de les cantonades del pad.
        /// </summary>
        /// 
        public double Roundnes {
            get {
                return roundnes;
            }
            set {
                if (value < 0 || roundnes > 1)
                    throw new ArgumentOutOfRangeException("Roundness");

                if (roundnes != value) {
                    roundnes = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Obte el radi de curvatura de les cantonades.
        /// </summary>
        /// 
        public double Radius {
            get {
                return Math.Min(size.Width, size.Height) * Roundnes / 2;
            }
        }

        /// <summary>
        /// Obte o asigna l'indicador de mascara de soldadura.
        /// </summary>
        /// 
        public bool Stop {
            get {
                return stop;
            }
            set {
                stop = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'indicador de pasta de soldar.
        /// </summary>
        /// 
        public bool Cream {
            get {
                return cream;
            }
            set {
                cream = value;
            }
        }
    }
}
