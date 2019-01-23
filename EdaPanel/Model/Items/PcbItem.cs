namespace MikroPic.EdaTools.v1.Panel.Model.Items {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using System;

    public sealed class PcbItem: ProjectItem {

        private Point position;
        private Size size;
        private Angle rotation;
        private string fileName;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la placa.</param>
        /// 
        public PcbItem(string fileName):
            base() { 
            
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            this.fileName = fileName;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la placa.</param>
        /// <param name="position">Posicio de la placa d'ins del panell.</param>
        /// <param name="size">Tamany de la envolvent de la placa.</param>
        /// <param name="rotation">Angle de rotacio de la placa centrat en la posicio.</param>
        /// 
        public PcbItem(string fileName, Point position, Size size, Angle rotation) { 

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            this.position = position;
            this.size = size;
            this.rotation = rotation;
            this.fileName = fileName;
        }

        public override void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el nom del fitxer de la placa.
        /// </summary>
        /// 
        public string FileName {
            get {
                return fileName;
            }
        }

        /// <summary>
        /// Obte o asigna la posicio.
        /// </summary>
        /// 
        public Point Position {
            set {
                position = value;
            }
            get {
                return position;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany de la envolvent..
        /// </summary>
        /// 
        public Size Size {
            set {
                size = value;
            }
            get {
                return size;
            }
        }

        /// <summary>
        /// Obte o asigna la rotacio.
        /// </summary>
        /// 
        public Angle Rotation {
            set {
                rotation = value;
            }
            get {
                return rotation;
            }
        }
    }
}
