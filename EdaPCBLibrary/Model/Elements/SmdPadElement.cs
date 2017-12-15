namespace MikroPic.EdaTools.v1.Pcb.Model.Elements {

    using System;
    using System.Windows;

    public sealed class SmdPadElement: SingleLayerElement {

        private string name;
        private double rotate;
        private Size size;
        private double roundnes;
        private bool cream = true;
        private bool stop = true;

        public SmdPadElement():
            base() {

        }

        public SmdPadElement(string name, Point position, Layer layer, Size size, double rotate, double roundnes, bool stop, bool cream):
            base(position, layer) {

            this.name = name;
            this.size = size;
            this.rotate = rotate;
            this.roundnes = roundnes;
            this.stop = stop;
            this.cream = cream;
        }

        public override bool InLayer(Layer layer) {

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
        /// Nom del pad.
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
        /// Obte o asigna la orientacio del pad.
        /// </summary>
        public double Rotate {
            get {
                return rotate;
            }
            set {
                rotate = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany del pad.
        /// </summary>
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
