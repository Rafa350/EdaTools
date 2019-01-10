namespace MikroPic.EdaTools.v1.Panel.Model.Items {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
    using System;

    public sealed class PcbItem: Model.ProjectItem {

        private Point position;
        private Angle rotation;
        private Polygon polygon;
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
        /// <param name="rotation">Angle de rotacio de la placa centrat en la posicio.</param>
        /// <param name="polygon">El poligon de la placa.</param>
        /// 
        public PcbItem(string fileName, Point position, Angle rotation, Polygon polygon) { 

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            this.position = position;
            this.rotation = rotation;
            this.polygon = polygon;
            this.fileName = fileName;
        }

        public override void AcceptVisitor(Model.IVisitor visitor) {
        }

        /// <summary>
        /// Obte o asigna el nom del fitxer de la placa.
        /// </summary>
        /// 
        public string FileName {
            set {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("PlaveElement.FileName");

                fileName = value;
            }
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

        /// <summary>
        ///  Obte el poligon.
        /// </summary>
        /// 
        public Polygon Polygon {
            get {
                return polygon;
            }
        }
    }
}
