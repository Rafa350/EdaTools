namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model.BoardElements;
    using System;
    using System.Collections.Generic;

    public sealed class Part: IPosition, IRotation, IName, IVisitable {

        private string name;
        private Point position;
        private Angle rotation;
        private BoardSide side = BoardSide.Top;
        private readonly Block block;
        private Dictionary<string, PartAttribute> attributes;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="block">El component associat.</param>
        /// <param name="name">El nom.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Angle de rotacio</param>
        /// <param name="side">Indica la cara de la placa.</param>
        /// 
        public Part(Block block, string name, Point position, Angle rotation, BoardSide side = BoardSide.Top) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (block == null)
                throw new ArgumentNullException("block");

            if ((side != BoardSide.Top) && (side != BoardSide.Bottom))
                throw new ArgumentOutOfRangeException("side");

            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.side = side;
            this.block = block;
        }

        public Part Clone(Block block) {

            return new Part(block, name, position, rotation, side);
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public override string ToString() {

            return String.Format("Part: '{0}'", name);
        }

        /// <summary>
        /// Afegeix un atribut.
        /// </summary>
        /// <param name="attribute">L'atribut a afeigir.</param>
        /// 
        public void AddAttribute(PartAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException("attribute");

            if (attributes == null)
                attributes = new Dictionary<string, PartAttribute>();

            attributes.Add(attribute.Name, attribute);
        }

        /// <summary>
        /// Elimina un atribut.
        /// </summary>
        /// <param name="attribute">L'atribut a eliminar.</param>
        /// 
        public void RemoveAttribute(PartAttribute attribute) {

            if (attribute == null)
                throw new ArgumentNullException("attribute");

            attributes.Remove(attribute.Name);

            if (attributes.Count == 0)
                attributes = null;
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="name">El nom de l'atribut.</param>
        /// <returns>El seu valor. Null si no existeix.</returns>
        /// 
        public PartAttribute GetAttribute(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if ((attributes != null) && attributes.TryGetValue(name, out PartAttribute attribute))
                return attribute;

            return null;
        }

        /// <summary>
        /// Obte un pad pel seu nom.
        /// </summary>
        /// <param name="name">El nom del pad.</param>
        /// <param name="throwOnError">True si dispara una execpcio si no el troba.</param>
        /// <returns>El pad. Null si no el troba.</returns>
        /// 
        public PadElement GetPad(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            PadElement pad = block.GetPad(name, false);
            if (pad != null)
                return pad;

            if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el pad '{0}' en el part '{1}'.", name, this.name));

            return null;
        }

        public Transformation GetLocalTransformation() {

            return new Transformation(position, rotation);
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
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Part.Name");

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
        public Point Position {
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

        public BoardSide Side {
            get {
                return side;
            }
            set {
                if ((value != BoardSide.Top) && (value != BoardSide.Bottom))
                    throw new ArgumentOutOfRangeException("Part.Side");

                side = value;
            }
        }

        /// <summary>
        /// Enumera els elements.
        /// </summary>
        /// 
        public IEnumerable<BoardElement> Elements {
            get {
                return block.Elements;
            }
        }

        /// <summary>
        /// Indica si conte pads.
        /// </summary>
        /// 
        public bool HasPads {
            get {
                return block.HasPads;
            }
        }

        /// <summary>
        /// Enumera els pads
        /// </summary>
        /// 
        public IEnumerable<PadElement> Pads {
            get {
                return block.Pads;
            }
        }

        /// <summary>
        /// Indica si conte atributs.
        /// </summary>
        /// 
        public bool HasAttributes {
            get {
                return attributes != null;
            }
        }

        /// <summary>
        /// Obte la llista d'atributs.
        /// </summary>
        /// 
        public IEnumerable<PartAttribute> Attributes {
            get {
                if (attributes == null)
                    throw new InvalidOperationException("No contiene atributos.");
                return attributes.Values;
            }
        }
    }
}
