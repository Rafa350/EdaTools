namespace MikroPic.EdaTools.v1.Panel.Model.Items {

    using System;
    using MikroPic.EdaTools.v1.Base.Geometry;

    public sealed class PcbItem : PanelItem {

        private Point _position;
        private Size _size;
        private Angle _rotation;
        private string _fileName;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="fileName">Nom del fitxer de la placa.</param>
        /// 
        public PcbItem(string fileName) :
            base() {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            _fileName = fileName;
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
                throw new ArgumentNullException(nameof(fileName));

            _position = position;
            _size = size;
            _rotation = rotation;
            _fileName = fileName;
        }

        public override void AcceptVisitor(IPanelVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte el nom del fitxer de la placa.
        /// </summary>
        /// 
        public string FileName => _fileName;

        /// <summary>
        /// Obte o asigna la posicio.
        /// </summary>
        /// 
        public Point Position {
            set {
                _position = value;
            }
            get {
                return _position;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany de la envolvent..
        /// </summary>
        /// 
        public Size Size {
            set {
                _size = value;
            }
            get {
                return _size;
            }
        }

        /// <summary>
        /// Obte o asigna la rotacio.
        /// </summary>
        /// 
        public Angle Rotation {
            set {
                _rotation = value;
            }
            get {
                return _rotation;
            }
        }
    }
}
