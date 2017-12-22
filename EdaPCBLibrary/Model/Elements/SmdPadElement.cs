namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class SmdPadElement: SingleLayerElement, IPosition, IRotation {

        private string name;
        private Point position;
        private Size size;
        private double rotation;
        private double roundnes;
        private bool cream = true;
        private bool stop = true;

        /// <summary>
        /// Constructor per defecte de l'objecte.
        /// </summary>
        /// 
        public SmdPadElement():
            base() {
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="layer"></param>
        /// <param name="size"></param>
        /// <param name="rotation"></param>
        /// <param name="roundnes"></param>
        /// <param name="stop"></param>
        /// <param name="cream"></param>
        /// 
        public SmdPadElement(string name, Point position, Layer layer, Size size, double rotation, double roundnes, bool stop, bool cream):
            base(layer) {

            this.name = name;
            this.position = position;
            this.size = size;
            this.rotation = rotation;
            this.roundnes = roundnes;
            this.stop = stop;
            this.cream = cream;
        }

        public override bool IsOnLayer(Layer layer) {

            if (Layer == layer)
                return true;
            else if ((Layer.Id == LayerId.Top) && (layer.Id == LayerId.TopStop) && stop)
                return true;
            else if ((Layer.Id == LayerId.Bottom) && (layer.Id == LayerId.BottomStop) && stop)
                return true;
            else if ((Layer.Id == LayerId.Top) && (layer.Id == LayerId.TopCream) && cream)
                return true;
            else if ((Layer.Id == LayerId.Bottom) && (layer.Id == LayerId.BottomCream) && cream)
                return true;
            else
                return false;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna el nom.
        /// </summary>
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        /// <summary>
        ///  Obte o asigna la posicio del centre geometric del rectangle.
        /// </summary>
        /// 
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
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
        public double Roundnes {
            get {
                return roundnes;
            }
            set {
                if (value < 0 || roundnes > 1)
                    throw new ArgumentOutOfRangeException("Roundness");
                roundnes = value;
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
