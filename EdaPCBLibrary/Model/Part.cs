namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public sealed class Part: IPosition, IRotation, IName {

        private string name;
        private PointInt position;
        private Angle rotation;
        private bool isFlipped;
        private readonly Block block;
        private Dictionary<string, PartAttribute> attributes = new Dictionary<string, PartAttribute>();

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="block">El bloc associat.</param>
        /// 
        public Part(Block block) {

            if (block == null)
                throw new ArgumentNullException("block");

            this.block = block;
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="block">El component associat.</param>
        /// <param name="name">El nom.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Angle de rotacio</param>
        /// <param name="isFlipped">Indica si va girat.</param>
        /// 
        public Part(Block block, string name, PointInt position, Angle rotation, bool isFlipped = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (block == null)
                throw new ArgumentNullException("block");

            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.isFlipped = isFlipped;
            this.block = block;
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public void AddAttribute(PartAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException("attribute");

            attributes.Add(attribute.Name, attribute);
        }

        public void RemoveAttribute(PartAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException("attribute");

            attributes.Remove(attribute.Name);
        }

        public PartAttribute GetAttribute(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            PartAttribute value;
            if (attributes.TryGetValue(name, out value))
                return value;

            return null;
        }

        /// <summary>
        /// Obte la transformacio per aplicar als seus elements.
        /// </summary>
        /// 
        public Transformation Transformation {
            get {
                return new Transformation(position, rotation);
            }
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
        /// Obte el block.
        /// </summary>
        /// 
        public Block Block {
            get {
                return block;
            }
        }

        /// <summary>
        /// Obte o asigna la posicio.
        /// </summary>
        /// 
        public PointInt Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'angle de rotacio.
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

        public bool IsFlipped {
            get {
                return isFlipped;
            }
            set {
                isFlipped = value;
            }
        }

        /// <summary>
        /// Obte la coleccio d'elements.
        /// </summary>
        /// 
        public IEnumerable<Element> Elements {
            get {
                return block.Elements;
            }
        }

        /// <summary>
        /// Obte la llista d'elements que son Pad's
        /// </summary>
        /// 
        public IEnumerable<PadElement> Pads {
            get {
                foreach (Element element in block.Elements) {
                    PadElement pad = element as PadElement;
                    if (pad != null)
                        yield return pad;
                }
            }
        }

        /// <summary>
        /// Obte la llista d'atributs.
        /// </summary>
        /// 
        public IEnumerable<PartAttribute> Attributes {
            get {
                return attributes.Values;
            }
        }
    }
}
