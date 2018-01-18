namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class Part: IPosition, IRotation, IName {

        private string name;
        private Point position;
        private double rotation;
        private bool isFlipped;
        private readonly Block block;
        private Dictionary<string, Parameter> parameters;

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
        public Part(Block block, string name, Point position, double rotation, bool isFlipped) {

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

        public void AddParameter(Parameter parameter) {

            if (parameter == null)
                throw new ArgumentNullException("parameter");

            if (parameters == null)
                parameters = new Dictionary<string, Parameter>();

            parameters.Add(parameter.Name, parameter);
        }

        public void RemoveParameter(Parameter parameter) {

            if (parameter == null)
                throw new ArgumentNullException("parameter");

            parameters.Remove(parameter.Name);
            if (parameters.Count == 0)
                parameters = null;
        }

        public Parameter GetParameter(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if ((parameters != null) && (parameters.ContainsKey(name)))
                return parameters[name];
            else
                return null;
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
        public double Rotation {
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
        /// Obte la matriu de transformacio per aplicar als elements.
        /// </summary>
        /// 
        public Matrix Transformation {
            get {
                Matrix m = new Matrix();
                m.Translate(position.X, position.Y);
                m.RotateAt(rotation, position.X, position.Y);
                return m;
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
                return block.Elements.OfType<PadElement>();
            }
        }

        /// <summary>
        /// Obte la llista de parametres.
        /// </summary>
        /// 
        public IEnumerable<Parameter> Parameters {
            get {
                return parameters == null ? null : parameters.Values;
            }
        }
    }
}
